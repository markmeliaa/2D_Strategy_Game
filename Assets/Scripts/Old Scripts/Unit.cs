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
    List<Vector3> path;

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
            if (Mathf.Abs(transform.position.x - enemy.transform.position.x) + Mathf.Abs(transform.position.y - enemy.transform.position.y) <= 1.25) // check is the enemy is near enough to attack
            {
                if (unitDamage >= 1)
                {
                    health -= unitDamage;
                    UpdateHealthDisplay();
                    DamageIcon d = Instantiate(damageIcon, transform.position, Quaternion.identity);
                    d.Setup(unitDamage);
                }
            }
        } 
        
        else 
        {
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
                enemy.health = 0;
                UpdateHealthDisplay();
                gm.ShowVictoryPanel(enemy.playerNumber);
            }

            GetWalkableTiles(); // check for new walkable tiles (if enemy has died we can now walk on his tile)
            gm.RemoveInfoPanel(enemy);
            PathfindingWithoutThreads.grid.NodeFromWorldPoint(enemy.transform.position).walkable = true;
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
                health = 0;
                UpdateHealthDisplay();
                gm.ShowVictoryPanel(playerNumber);
            }

            gm.ResetTiles(); // reset tiles when we die
            gm.RemoveInfoPanel(this);
            PathfindingWithoutThreads.grid.NodeFromWorldPoint(this.transform.position).walkable = true;
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

        //Debug.Log("Moving " + this.gameObject.name);

        Node lastNode = null;
        if (path.Count > 0)
        {
            int steps = 0;
            currentNode.walkable = true;
            currentNode.hasUnit = false;

            while (path.Count > 0 && steps < tileSpeed)
            {
                transform.position = Vector3.MoveTowards(transform.position, path[0], moveSpeed * Time.deltaTime);
                gm.MoveInfoPanel(this);

                if (transform.position == path[0])
                {
                    steps++;
                    lastNode = PathfindingWithoutThreads.grid.NodeFromWorldPoint(path[0]);
                    path.RemoveAt(0);
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

        if (lastNode != null)
        {
            lastNode.walkable = false;
            lastNode.hasUnit = true;
        }

        currentNode = lastNode;
        hasMoved = true;
        ResetWeaponIcon();
        GetEnemies();
    }

    IEnumerator StartMovementArcher(Node moveTo)
    { // Moves the character to his new position.

        path = PathfindingWithoutThreads.FindPath(transform.position, moveTo.worldPosition);

        Node lastNode = null;
        if (path.Count > 0)
        {
            int steps = 0;
            currentNode.walkable = true;
            currentNode.hasUnit = false;

            Vector3 last = new Vector3();
            Vector3 lastlast = new Vector3();

            foreach (Vector3 node in path)
            {
                lastlast = last;
                last = node;
            }

            List<Node> neighbors = PathfindingWithoutThreads.grid.GetNeighours(PathfindingWithoutThreads.grid.NodeFromWorldPoint(last));

            foreach (Node node in neighbors)
            {
                if (PathfindingWithoutThreads.grid.NodeFromWorldPoint(lastlast).walkable && node.hasUnit)
                {
                    path.Remove(last);
                    break;
                }
            }

            while (path.Count > 0 && steps < tileSpeed)
            {
                transform.position = Vector3.MoveTowards(transform.position, path[0], moveSpeed * Time.deltaTime);
                gm.MoveInfoPanel(this);

                if (transform.position == path[0])
                {
                    steps++;
                    lastNode = PathfindingWithoutThreads.grid.NodeFromWorldPoint(path[0]);
                    path.RemoveAt(0);
                }

                yield return null;
            }

            /*
            neighbors = PathfindingWithoutThreads.grid.GetNeighours(PathfindingWithoutThreads.grid.NodeFromWorldPoint(path[0]));

            foreach (Node node in neighbors)
            {
                if (node.hasUnit)
                {
                    transform.position = Vector3.MoveTowards(transform.position, path[0], moveSpeed * Time.deltaTime);
                    gm.MoveInfoPanel(this);

                    lastNode = PathfindingWithoutThreads.grid.NodeFromWorldPoint(path[0]);
                    path.RemoveAt(0);
                }
                    
            }
            */
        }

        if (lastNode != null)
        {
            lastNode.walkable = false;
            lastNode.hasUnit = true;
        }

        currentNode = lastNode;
        hasMoved = true;
        ResetWeaponIcon();
        GetEnemies();
    }

    public IEnumerator Act()
    {
        if (!hasMoved)
        {
            if (isBlueKing)
            {
                Flee();
                yield return new WaitForSeconds(1.5f);
                Attack();
            }

            else if (transform.tag == "Archer")
            {
                MoveArcher();
                yield return new WaitForSeconds(1.5f);
                Attack();
            }

            else
            {
                Move();
                yield return new WaitForSeconds(1.5f);
                Attack();
            }
        }
    }

    void Flee()
    {
        Vector3 moveTo = InfluenceMapControl.influenceMap.GetPositionWithLessInfluence();
        
        if (moveTo != new Vector3(-99, -99, -99)) StartCoroutine(StartMovement(PathfindingWithoutThreads.grid.NodeFromWorldPoint(moveTo)));
    }

    void Move()
    {
        Vector3 moveTo = InfluenceMapControl.influenceMap.GetPositionWithMoreInfluence();

        if (moveTo != new Vector3(-99, -99, -99)) StartCoroutine(StartMovement(PathfindingWithoutThreads.grid.NodeFromWorldPoint(moveTo)));
    }

    void MoveArcher()
    {
        Vector3 moveTo = InfluenceMapControl.influenceMap.GetPositionWithMoreInfluence();

        if (moveTo != new Vector3(-99, -99, -99)) StartCoroutine(StartMovementArcher(PathfindingWithoutThreads.grid.NodeFromWorldPoint(moveTo)));
    }

    void Attack()
    {
        if (enemiesInRange.Count > 0)
        {
            foreach (Unit enemy in enemiesInRange)
            {
                if (enemy.isRedKing)
                {
                    Attack(enemy);
                    return;
                }
            }

            Attack(enemiesInRange[Random.Range(0, enemiesInRange.Count)]);
        }
    }
}
