using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public bool isSelected;
    public bool hasMoved;

    public int tileSpeed;
    public float moveSpeed;

    private GM gm;

    public float attackRadius;
    public bool hasAttacked;
    public List<Unit> enemiesInRange = new List<Unit>();

    public int playerNumber;

    public GameObject weaponIcon;

    // Attack Stats
    public int health = 10;
    public int attackDamage;
    public int defenseDamage;
    public int armor;

    public DamageIcon damageIcon;

    public int cost;

	public GameObject deathEffect;

	private Animator camAnim;

    public bool isBlueKing;

    public bool isRedKing;

	private AudioSource source;

    public Text displayedText;

    // Pathfinding stuff
    Node currentNode;
    Queue<Vector3> path;

    private void Start()
    {
		source = GetComponent<AudioSource>();
		camAnim = Camera.main.GetComponent<Animator>();
        gm = FindObjectOfType<GM>();

        if (isBlueKing)
            displayedText = GameObject.FindGameObjectWithTag("blueTextLife").gameObject.GetComponent<Text>();
        else if (isRedKing)
            displayedText = GameObject.FindGameObjectWithTag("redTextLife").gameObject.GetComponent<Text>();

        UpdateHealthDisplay();
        currentNode = PathfindingWithoutThreads.grid.NodeFromWorldPoint(transform.position);
    }

    private void UpdateHealthDisplay ()
    {
        if (isRedKing || isBlueKing)
        {
            displayedText.text = health.ToString();
        }
    }

    private void OnMouseDown() // select character or deselect if already selected
    {
        if (gm.playerTurn == 1)
        {
            ResetWeaponIcon();

            if (isSelected == true)
            {

                isSelected = false;
                gm.selectedUnit = null;
                gm.ResetTiles();

            }
            else
            {
                if (playerNumber == gm.playerTurn)
                { // select unit only if it's his turn
                    if (gm.selectedUnit != null)
                    { // deselect the unit that is currently selected, so there's only one isSelected unit at a time
                        gm.selectedUnit.isSelected = false;
                    }
                    gm.ResetTiles();

                    gm.selectedUnit = this;

                    isSelected = true;
                    if (source != null)
                    {
                        source.Play();
                    }

                    GetWalkableTiles();
                    GetEnemies();
                }

                Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.15f);
                if (col != null)
                {
                    Unit unit = col.GetComponent<Unit>(); // double check that what we clicked on is a unit
                    if (unit != null && gm.selectedUnit != null)
                    {
                        if (gm.selectedUnit.enemiesInRange.Contains(unit) && !gm.selectedUnit.hasAttacked)
                        { // does the currently selected unit have in his list the enemy we just clicked on
                            gm.selectedUnit.Attack(unit);

                        }
                    }
                }
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            gm.UpdateInfoPanel(this);
        }
    }



    void GetWalkableTiles() { // Looks for the tiles the unit can walk on
        if (hasMoved == true) {
            return;
        }

        Tile[] tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles) {
            if (Mathf.Abs(transform.position.x - tile.transform.position.x) + Mathf.Abs(transform.position.y - tile.transform.position.y) <= tileSpeed)
            { // how far he can move
                if (tile.isClear() == true)
                { // is the tile clear from any obstacles
                    tile.Highlight();
                }

            }          
        }
    }

    void GetEnemies() {
    
        enemiesInRange.Clear();

        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= attackRadius) // check is the enemy is near enough to attack
            {
                if (enemy.playerNumber != gm.playerTurn && !hasAttacked) { // make sure you don't attack your allies
                    enemiesInRange.Add(enemy);
                    enemy.weaponIcon.SetActive(true);
                }

            }
        }
    }

    public void Move(Node n)
    {
        gm.ResetTiles();
        StartCoroutine(StartMovement(n));
    }

    void Attack(Unit enemy) {
        hasAttacked = true;

        int enemyDamege = attackDamage - enemy.armor;
        int unitDamage = enemy.defenseDamage - armor;

        if (enemyDamege >= 1)
        {
            enemy.health -= enemyDamege;
            enemy.UpdateHealthDisplay();
            DamageIcon d = Instantiate(damageIcon, enemy.transform.position, Quaternion.identity);
            d.Setup(enemyDamege);
        }

        if (transform.tag == "Archer" && enemy.tag != "Archer")
        {
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= 2) // check is the enemy is near enough to attack
            {
                if (unitDamage >= 1)
                {
                    health -= unitDamage;
                    UpdateHealthDisplay();
                    DamageIcon d = Instantiate(damageIcon, transform.position, Quaternion.identity);
                    d.Setup(unitDamage);
                }
            }
        } else {

            if (unitDamage >= 1)
            {
                health -= unitDamage;
                UpdateHealthDisplay();
                DamageIcon d = Instantiate(damageIcon, transform.position, Quaternion.identity);
                d.Setup(unitDamage);
            }
        }

        if (enemy.health <= 0)
        {
         
            if (deathEffect != null){
				Instantiate(deathEffect, enemy.transform.position, Quaternion.identity);
				camAnim.SetTrigger("shake");
			}

            if (enemy.isRedKing || enemy.isBlueKing)
            {
                gm.ShowVictoryPanel(enemy.playerNumber);
            }

            GetWalkableTiles(); // check for new walkable tiles (if enemy has died we can now walk on his tile)
            gm.RemoveInfoPanel(enemy);
            Destroy(enemy.gameObject);
        }

        if (health <= 0)
        {

            if (deathEffect != null)
			{
				Instantiate(deathEffect, enemy.transform.position, Quaternion.identity);
				camAnim.SetTrigger("shake");
			}

			if (isRedKing || isBlueKing)
            {
                gm.ShowVictoryPanel(playerNumber);
            }

            gm.ResetTiles(); // reset tiles when we die
            gm.RemoveInfoPanel(this);
            Destroy(gameObject);
        }

        gm.UpdateInfoStats();
    }   

    public void ResetWeaponIcon() {
        Unit[] enemies = FindObjectsOfType<Unit>();
        foreach (Unit enemy in enemies)
        {
            enemy.weaponIcon.SetActive(false);
        }
    }

    IEnumerator StartMovement(Node moveTo) { // Moves the character to his new position.

        path = PathfindingWithoutThreads.FindPath(transform.position, moveTo.worldPosition);

        Node lastNode = null;
        if (path.Count > 0)
        {
            int steps = 0;
            currentNode.walkable = true;

            while (path.Count > 0 && steps < moveSpeed)
            {
                transform.position = Vector3.MoveTowards(transform.position, path.Peek(), Time.deltaTime);
                gm.MoveInfoPanel(this);

                if (transform.position == path.Peek())
                {
                    steps++;
                    lastNode = PathfindingWithoutThreads.grid.NodeFromWorldPoint(path.Peek());
                    path.Dequeue();
                }

                yield return null;
            }
        }

        /*
        while (transform.position.x != movePos.position.x) { // first aligns him with the new tile's x pos
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(movePos.position.x, transform.position.y), moveSpeed * Time.deltaTime);
            yield return null;
        }
        while (transform.position.y != movePos.position.y) // then y
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, movePos.position.y), moveSpeed * Time.deltaTime);
            yield return null;
        }
        */
        Debug.Log(lastNode.worldPosition);
        lastNode.walkable = false;
        currentNode = lastNode;
        hasMoved = true;
        ResetWeaponIcon();
        GetEnemies();
    }

    public void Act()
    {
        if (hasMoved) return;

        if (isBlueKing)
        {
            Flee();
            return;
        }

        // Move();
        // Attack();
    }

    void Flee()
    {
        Vector3 moveTo = InfluenceMapControl.influenceMap.GetPositionWithLessInfluence();
        Debug.Log(moveTo);
        if (moveTo != new Vector3(-99, -99, -99)) StartCoroutine(StartMovement(PathfindingWithoutThreads.grid.NodeFromWorldPoint(moveTo)));
    }
}
