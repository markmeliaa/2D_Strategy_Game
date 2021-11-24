using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitThreads : MonoBehaviour
{
    public Transform target;
    float speed = 1.0f;
    Vector3[] path;
    int targetIndex;

    void Start()
    {
        // WITH THREADS
        PathRequestManagerThreads.RequestPath(new PathRequestThreads(transform.position, target.position, OnPathFound));
    }

    private void Update()
    {
        // WITH THREADS
        PathRequestManagerThreads.RequestPath(new PathRequestThreads(transform.position, target.position, OnPathFound));
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            transform.LookAt(currentWaypoint);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(path[i], new Vector3(0.3f, 0.3f, 0.3f));

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }

                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
