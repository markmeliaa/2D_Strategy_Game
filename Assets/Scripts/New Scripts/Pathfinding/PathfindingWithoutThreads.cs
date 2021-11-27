using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using System;

public class PathfindingWithoutThreads : MonoBehaviour
{
    PathRequestManagerWithoutThreads requestManager;

    // Grid script and this script must be in the same gameObject for this to work
    public static FieldGrid grid;

    void Awake()
    {
        grid = GetComponent<FieldGrid>();
        //requestManager = GetComponent<PathRequestManagerWithoutThreads>(); // WITHOUT THREADS
    }

    // WITHOUT THREADS
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        //StartCoroutine(FindPath(startPos, targetPos));
    }

    // WITHOUT THREADS
    // Apply A*
    public static Queue<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // Get positions as nodes
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // Initialize the open and closed set
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        // Start the open set with the first node
        openSet.Add(startNode);

        Node currentNode = null;

        // While there are nodes on the open set, keep looking for a route
        while (openSet.Count > 0)
        {
            // Find node with less distance in the open set
            currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // If the current node is the end node, a path has been found (return it)
            if (currentNode == targetNode) return RetracePath(startNode, currentNode);

            // Get neighbours from current node and add them if they're not on the open set or has a better distance value
            foreach (Node neighbour in grid.GetNeighours(currentNode))
            {
                if (neighbour != targetNode)
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;
                }

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
            }
        }

        // No path found, return empty list
        return new Queue<Vector3>();

    }

    public static Queue<Vector3> RetracePath(Node startNode, Node endNode)
    {
        Queue<Vector3> path = new Queue<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Enqueue(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }

        // As path is a route starting at the end node, reverse it
        path = new Queue<Vector3>(path.Reverse());

        return path;
    }

    public static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }
}
