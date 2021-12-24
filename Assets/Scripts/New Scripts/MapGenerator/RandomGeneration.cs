using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGeneration : MonoBehaviour
{
    [SerializeField] public GameObject king;
    [SerializeField] public GameObject knight;
    [SerializeField] public GameObject archer;
    [SerializeField] public GameObject dragon;
    [SerializeField] public GameObject village;
    [SerializeField] public GameObject[] trees;

    [SerializeField] public List<GameObject> spawnpoints;

    Node currentNode;

    private GameObject unitParent;

    private FieldGrid pathfindingGrid;

    private int mapHalf;

    // Start is called before the first frame update
    void Start()
    {
        unitParent = GameObject.FindGameObjectWithTag("unitParent").gameObject;
        pathfindingGrid = GameObject.Find("PathfindingGrid").GetComponent<FieldGrid>();
        mapHalf = pathfindingGrid.gridSizeX / 2;

        //PlaceUnits();

        int rand = Random.Range(0, spawnpoints.Count);
        Instantiate(king, spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);
        //Debug.Log(spawnpoints[rand].transform.position);
        currentNode = PathfindingWithoutThreads.grid.NodeFromWorldPoint(spawnpoints[rand].transform.position);
        //Debug.Log(currentNode);
        currentNode.walkable = false;
        currentNode.hasUnit = true;
        spawnpoints.RemoveAt(rand);

        for (int i = 0; i < 3; i++)
        {
            rand = Random.Range(0, spawnpoints.Count);
            Instantiate(knight, spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);
            currentNode = PathfindingWithoutThreads.grid.NodeFromWorldPoint(spawnpoints[rand].transform.position);
            currentNode.walkable = false;
            currentNode.hasUnit = true;
            spawnpoints.RemoveAt(rand);
        }
       
        for (int i = 0; i < 2; i++)
        {
            rand = Random.Range(0, spawnpoints.Count);
            Instantiate(archer, spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);
            currentNode = PathfindingWithoutThreads.grid.NodeFromWorldPoint(spawnpoints[rand].transform.position);
            currentNode.walkable = false;
            currentNode.hasUnit = true;
            spawnpoints.RemoveAt(rand);
        }

        for (int i = 0; i < 2; i++)
        {
            rand = Random.Range(0, spawnpoints.Count);
            Instantiate(dragon, spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);
            currentNode = PathfindingWithoutThreads.grid.NodeFromWorldPoint(spawnpoints[rand].transform.position);
            currentNode.walkable = false;
            currentNode.hasUnit = true;
            spawnpoints.RemoveAt(rand);
        }
        

        for (int i = 0; i < 3; i++)
        {
            rand = Random.Range(0, spawnpoints.Count);
            Instantiate(village, spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);
            currentNode = PathfindingWithoutThreads.grid.NodeFromWorldPoint(spawnpoints[rand].transform.position);
            currentNode.walkable = false;
            spawnpoints.RemoveAt(rand);
        }

        while (spawnpoints.Count > 0)
        {
            rand = Random.Range(0, spawnpoints.Count);
            int rand2 = Random.Range(0, trees.Length);
            Instantiate(trees[rand2], spawnpoints[rand].transform.position, Quaternion.identity, unitParent.transform);

            if (rand2 != 3 && rand2 != 4 && rand2 != 5)
            {
                currentNode = PathfindingWithoutThreads.grid.NodeFromWorldPoint(spawnpoints[rand].transform.position);
                //currentNode.walkable = false;
                currentNode.hasTree = true;
                currentNode.tacticalCost = 10;            }

            else
            { 
                currentNode = PathfindingWithoutThreads.grid.NodeFromWorldPoint(spawnpoints[rand].transform.position);
                currentNode.tacticalCost = 0f;
            }

            spawnpoints.RemoveAt(rand);
        }
    }

    void PlaceUnits()
    {
        int randX = Random.Range(0, pathfindingGrid.gridSizeX);
        while (randX > mapHalf)
        {
            randX = Random.Range(0, pathfindingGrid.gridSizeX);
        }
        int randY = Random.Range(0, pathfindingGrid.gridSizeY);

        if (!pathfindingGrid.grid[randX, randY].hasUnit && !pathfindingGrid.grid[randX, randY].hasTree && pathfindingGrid.grid[randX, randY].walkable)
        {
            Instantiate(king, pathfindingGrid.grid[randX, randY].worldPosition, Quaternion.identity, unitParent.transform);
            pathfindingGrid.grid[randX, randY].hasUnit = true;
            pathfindingGrid.grid[randX, randY].walkable = false;
        }

        for (int i = 0; i < 3; i++)
        {
            randX = Random.Range(0, pathfindingGrid.gridSizeX);
            while (randX > mapHalf)
            {
                randX = Random.Range(0, pathfindingGrid.gridSizeX);
            }
            randY = Random.Range(0, pathfindingGrid.gridSizeY);

            if (!pathfindingGrid.grid[randX, randY].hasUnit && !pathfindingGrid.grid[randX, randY].hasTree && pathfindingGrid.grid[randX, randY].walkable)
            {
                Instantiate(knight, pathfindingGrid.grid[randX, randY].worldPosition, Quaternion.identity, unitParent.transform);
                pathfindingGrid.grid[randX, randY].hasUnit = true;
                pathfindingGrid.grid[randX, randY].walkable = false;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            randX = Random.Range(0, pathfindingGrid.gridSizeX);
            while (randX > mapHalf)
            {
                randX = Random.Range(0, pathfindingGrid.gridSizeX);
            }
            randY = Random.Range(0, pathfindingGrid.gridSizeY);

            if (!pathfindingGrid.grid[randX, randY].hasUnit && !pathfindingGrid.grid[randX, randY].hasTree && pathfindingGrid.grid[randX, randY].walkable)
            {
                Instantiate(archer, pathfindingGrid.grid[randX, randY].worldPosition, Quaternion.identity, unitParent.transform);
                pathfindingGrid.grid[randX, randY].hasUnit = true;
                pathfindingGrid.grid[randX, randY].walkable = false;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            randX = Random.Range(0, pathfindingGrid.gridSizeX);
            while (randX > mapHalf)
            {
                randX = Random.Range(0, pathfindingGrid.gridSizeX);
            }
            randY = Random.Range(0, pathfindingGrid.gridSizeY);

            if (!pathfindingGrid.grid[randX, randY].hasUnit && !pathfindingGrid.grid[randX, randY].hasTree && pathfindingGrid.grid[randX, randY].walkable)
            {
                Instantiate(dragon, pathfindingGrid.grid[randX, randY].worldPosition, Quaternion.identity, unitParent.transform);
                pathfindingGrid.grid[randX, randY].hasUnit = true;
                pathfindingGrid.grid[randX, randY].walkable = false;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            randX = Random.Range(0, pathfindingGrid.gridSizeX);
            while (randX > mapHalf)
            {
                randX = Random.Range(0, pathfindingGrid.gridSizeX);
            }
            randY = Random.Range(0, pathfindingGrid.gridSizeY);

            if (!pathfindingGrid.grid[randX, randY].hasUnit && !pathfindingGrid.grid[randX, randY].hasTree && pathfindingGrid.grid[randX, randY].walkable)
            {
                Instantiate(village, pathfindingGrid.grid[randX, randY].worldPosition, Quaternion.identity, unitParent.transform);
                pathfindingGrid.grid[randX, randY].walkable = false;
            }
        }

        for (int i = 0; i < 15; i++)
        {
            randX = Random.Range(0, pathfindingGrid.gridSizeX);
            while (randX > mapHalf)
            {
                randX = Random.Range(0, pathfindingGrid.gridSizeX);
            }
            randY = Random.Range(0, pathfindingGrid.gridSizeY);
            int treeRand = Random.Range(0, trees.Length);

            if (!pathfindingGrid.grid[randX, randY].hasUnit && !pathfindingGrid.grid[randX, randY].hasTree && pathfindingGrid.grid[randX, randY].walkable)
            {
                Instantiate(trees[treeRand], pathfindingGrid.grid[randX, randY].worldPosition, Quaternion.identity, unitParent.transform);
                if (treeRand != 3 && treeRand != 4 && treeRand != 5)
                {
                    pathfindingGrid.grid[randX, randY].walkable = false;
                    pathfindingGrid.grid[randX, randY].hasTree = true;
                    pathfindingGrid.grid[randX, randY].tacticalCost = 10;
                }
            }
        }
    }
}
