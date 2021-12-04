using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTMoveUnit : BTNode
{
    Unit unit;
    bool move = false;

    public BTMoveUnit(BehaviorTree t, Unit _unit) : base(t)
    {
        unit = _unit;
    }

    bool Move()
    {
        Vector3 moveTo = InfluenceMapControl.influenceMap.GetPositionWithMoreInfluence();

        //Debug.Log(moveTo);

        if (moveTo != new Vector3(-99, -99, -99))
        {
            if (unit.transform.tag == "Archer")
            {
                unit.MoveArcher();
                //StartCoroutine(unit.StartMovementArcher(PathfindingWithoutThreads.grid.NodeFromWorldPoint(moveTo)));
            }

            else
            {
                unit.Move();
                //StartCoroutine(unit.StartMovement(PathfindingWithoutThreads.grid.NodeFromWorldPoint(moveTo)));
            }

            return true;
        }

        return false;
    }

    public override Result Execute()
    {
        move = Move();

        if (move)
            return Result.Success;
        else
            return Result.Failure;
    }
}
