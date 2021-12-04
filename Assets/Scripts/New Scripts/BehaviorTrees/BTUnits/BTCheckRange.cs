using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCheckRange : BTNode
{
    Unit unit;

    public BTCheckRange(BehaviorTree t, Unit _unit) : base(t)
    {
        unit = _unit;
    }

    public override Result Execute()
    {
        List<Unit> enemiesInRange = unit.enemiesInRange;

        //Debug.Log(enemiesInRange.Count);

        if (enemiesInRange.Count <= 0)
            return Result.Failure;

        else
            return Result.Success;
    }
}
