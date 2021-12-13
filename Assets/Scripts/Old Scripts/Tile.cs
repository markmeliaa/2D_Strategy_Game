using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer rend;
    public Color highlightedColor;
    public Color creatableColor;

    public LayerMask obstacles;

    public bool isWalkable;
    public bool isCreatable;

    private GM gm;

    public float amount;
    private bool sizeIncrease;

	private AudioSource source;

    public Node node;

    private void Start()
    {
		source = GetComponent<AudioSource>();
        gm = FindObjectOfType<GM>();
        rend = GetComponent<SpriteRenderer>();

        isWalkable = false;
        node = PathfindingWithoutThreads.grid.NodeFromWorldPoint(transform.position);
    }

    public bool isClear() // does this tile have an obstacle on it. Yes or No?
    {
        return node.walkable && !node.hasTree;
    }

    public bool isClearKnight()
    {
        return node.walkable;
    }

    public void Highlight() {
		
        rend.color = highlightedColor;
        isWalkable = true;
    }

    public void Reset()
    {
        rend.color = Color.white;
        isWalkable = false;
        isCreatable = false;
    }

    public void SetCreatable() {
        rend.color = creatableColor;
        isCreatable = true;
    }

    private void OnMouseDown()
    {
        if (gm.playerTurn == 2) return;

        if (isCreatable == true && gm.createdUnit != null) 
        {
            Unit unit = Instantiate(gm.createdUnit, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            PathfindingWithoutThreads.grid.NodeFromWorldPoint(unit.transform.position).walkable = false;
            PathfindingWithoutThreads.grid.NodeFromWorldPoint(unit.transform.position).hasUnit = true;
            unit.hasMoved = true;
            unit.hasAttacked = true;
            unit.transform.parent = GameObject.FindGameObjectWithTag("unitParent").gameObject.transform;
            gm.ResetTiles();
            gm.createdUnit = null;
        } 
        
        else if (isCreatable == true && gm.createdVillage != null) 
        {
            Village village = Instantiate(gm.createdVillage, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            village.transform.parent = GameObject.FindGameObjectWithTag("unitParent").gameObject.transform;
            PathfindingWithoutThreads.grid.NodeFromWorldPoint(village.transform.position).walkable = false;
            gm.ResetTiles();
            gm.createdVillage = null;
        }

        else if (gm.selectedUnit != null && isWalkable && node.walkable)
        {
            gm.selectedUnit.Move(node);
        }
    }


    private void OnMouseEnter()
    {
        if (isClear() == true) {
			source.Play();
			sizeIncrease = true;
            transform.localScale += new Vector3(amount, amount, amount);
        }
        
    }

    private void OnMouseExit()
    {
        if (isClear() == true)
        {
            sizeIncrease = false;
            transform.localScale -= new Vector3(amount, amount, amount);
        }

        if (isClear() == false && sizeIncrease == true) {
            sizeIncrease = false;
            transform.localScale -= new Vector3(amount, amount, amount);
        }
    }
}
