using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehavior : MonoBehaviour
{
    [SerializeField] GM gm;
    [SerializeField] BehaviorTree behaviorTree;

    public List<Unit> enemyUnits = new List<Unit>();

    public IEnumerator AITurn()
    {
        yield return new WaitForSeconds(.1f);

        enemyUnits.Clear();

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (unit.playerNumber == 2)
            {
                unit.hasMoved = false;
                unit.hasAttacked = false;
                enemyUnits.Add(unit);
            }  
        }

        StartCoroutine(behaviorTree.RunBehavior(behaviorTree.MRoot));

        yield return new WaitForSeconds(.5f);

        StopAllCoroutines();

        // Then move the troops
        foreach (Unit unit in enemyUnits)
        {
            /*
            if (!unit.isBlueKing)
            {
                unit.uRoot = new BTRepeater(behaviorTree, new BTSelector(behaviorTree, new BTNode[] {
                new BTRepeater(behaviorTree, new BTSequencer(behaviorTree, new BTNode [] { new BTCheckRange(behaviorTree, unit), new BTAttackUnit(behaviorTree, unit)})),
                new BTRepeater(behaviorTree, new BTSequencer(behaviorTree, new BTNode[] { new BTMoveUnit(behaviorTree, unit), new BTWaitNode(behaviorTree), new BTCheckRange(behaviorTree, unit), new BTAttackUnit(behaviorTree, unit) })) }));
            }

            else
            {
                unit.uRoot = new BTRepeater(behaviorTree, new BTSelector(behaviorTree, new BTNode[] {
                new BTRepeater(behaviorTree, new BTSequencer(behaviorTree, new BTNode [] { new BTCheckRange(behaviorTree, unit), new BTAttackUnit(behaviorTree, unit)})),
                new BTRepeater(behaviorTree, new BTSequencer(behaviorTree, new BTNode[] { new BTFleeKing(behaviorTree, unit), new BTWaitNode(behaviorTree), new BTCheckRange(behaviorTree, unit), new BTAttackUnit(behaviorTree, unit) })) }));
            }

            StartCoroutine(behaviorTree.RunBehavior(unit.uRoot));
            */

            StartCoroutine(unit.Act());
            yield return new WaitForSeconds(1.75f);
        }

        gm.EndTurn();
    }
}
