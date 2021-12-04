using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTFleeKing : BTNode
{
    Unit unit;
    bool flee = false;

    public BTFleeKing(BehaviorTree t, Unit _unit) : base(t)
    {
        unit = _unit;
    }

    bool Flee()
    {
        Vector3 moveTo = InfluenceMapControl.influenceMap.GetPositionWithLessInfluence();

        //Debug.Log(moveTo);

        if (moveTo != new Vector3(-99, -99, -99))
        {
            unit.Flee();
            return true;
        }

        return false;
    }

    public override Result Execute()
    {
        flee = Flee();

        if (flee)
            return Result.Success;
        else
            return Result.Failure;
    }
}
