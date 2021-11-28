using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehavior : MonoBehaviour
{
    [SerializeField] GM gm;
    List<Unit> enemyUnits = new List<Unit>();

    public IEnumerator AITurn()
    {
        yield return new WaitForSeconds(.1f);

        enemyUnits.Clear();

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (unit.playerNumber == 2)
                enemyUnits.Add(unit);
        }

        // First check to buy troops
        BuyTroop(Random.Range(0, 3));
        yield return new WaitForSeconds(0.75f);

        // Then move the troops
        foreach (Unit unit in enemyUnits)
        {
            StartCoroutine(unit.Act());
            yield return new WaitForSeconds(1.5f);
        }

        gm.EndTurn();
    }

    // Decides whether to buy or not
    // TODO: Place the units and villages in the map
    public void BuyTroop(int spend)
    {
        if (spend == 0)
            return;

        // Spend just in one troop, the best you can afford
        if (spend == 1 && gm.player2Gold > 40 && enemyUnits.Count < 11)
        {
            if (gm.player2Gold > 100 && enemyUnits.Count > 3)
            {
                Debug.Log("Can buy village");
                gm.player2Gold -= 100;
            }

            else if (gm.player2Gold > 90)
            {
                Debug.Log("Can buy archer");
                gm.player2Gold -= 90;
            }

            else if (gm.player2Gold > 70)
            {
                Debug.Log("Can buy dragon");
                gm.player2Gold -= 70;
            }

            else
            {
                Debug.Log("Can buy knight");
                gm.player2Gold -= 40;
            }

            gm.UpdateGoldText();
        }

        // Spend all the money
        if (spend == 2 && enemyUnits.Count < 7)
        {
            while (gm.player2Gold > 40)
            {
                if (gm.player2Gold > 100 && enemyUnits.Count > 3)
                {
                    Debug.Log("Can buy village");
                    gm.player2Gold -= 100;
                }

                else if (gm.player2Gold > 90)
                {
                    Debug.Log("Can buy archer");
                    gm.player2Gold -= 90;
                }

                else if (gm.player2Gold > 70)
                {
                    Debug.Log("Can buy dragon");
                    gm.player2Gold -= 70;
                }

                else
                {
                    Debug.Log("Can buy knight");
                    gm.player2Gold -= 40;
                }

                gm.UpdateGoldText();
            }
        }
    }
}
