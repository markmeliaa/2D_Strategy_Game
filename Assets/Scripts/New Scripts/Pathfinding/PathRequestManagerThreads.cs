using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class PathRequestManagerThreads : MonoBehaviour
{
    Queue<PathResultThreads> results = new Queue<PathResultThreads>(); // WITH THREADS

    static PathRequestManagerThreads instance;
    PathfindingThreads pathfinding;

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<PathfindingThreads>();
    }

    // WITH THREADS
    void Update()
    {
        if (results.Count > 0)
        {
            int itemsInQueue = results.Count;
            lock (results)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    PathResultThreads result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    // WITH THREADS
    public static void RequestPath(PathRequestThreads request)
    {
        ThreadStart threadStart = delegate
        {
            instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
        };

        threadStart.Invoke();
    }

    // WITH THREADS
    public void FinishedProcessingPath(PathResultThreads result)
    {
        lock (results)
        {
            results.Enqueue(result);
        }
    }
}

// WITH THREADS
public struct PathRequestThreads
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequestThreads(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }
}

// WITH THREADS
public struct PathResultThreads
{
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResultThreads(Vector3[] _path, bool _success, Action<Vector3[], bool> _callback)
    {
        path = _path;
        success = _success;
        callback = _callback;
    }
}
