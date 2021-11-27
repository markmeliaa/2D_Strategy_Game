using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManagerWithoutThreads : MonoBehaviour
{
    Queue<PathRequestWithoutThreads> pathRequestQueue = new Queue<PathRequestWithoutThreads>(); // WITHOUT THREADS
    PathRequestWithoutThreads currentPathRequest; // WITHOUT THREADS 

    static PathRequestManagerWithoutThreads instance;
    PathfindingWithoutThreads pathfinding;

    bool isProcessingPath; // WITHOUT THREADS

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<PathfindingWithoutThreads>();
    }

    // WITHOUT THREADS
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequestWithoutThreads newRequest = new PathRequestWithoutThreads(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    // WITHOUT THREADS
    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    // WITHOUT THREADS
    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    // WITHOUT THREADS
    struct PathRequestWithoutThreads
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequestWithoutThreads(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
