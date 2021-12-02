using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTLess5Units : BTNode
{
    AIBehavior ai;

    public BTLess5Units(BehaviorTree t) : base(t)
    {
        ai = GameObject.Find("GameMaster").GetComponent<AIBehavior>();
    }

    public override Result Execute()
    {
        // If the AI has more than 5 troops, return Success
        if (ai.enemyUnits.Count <= 5)
            return Result.Success;

        else
            return Result.Failure;
    }
}
