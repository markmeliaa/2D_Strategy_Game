using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCheckMoney : BTNode
{
    GM gm;

    public BTCheckMoney(BehaviorTree t) : base(t)
    {
        gm = GameObject.Find("GameMaster").GetComponent<GM>();
    }

    public override Result Execute()
    {
        // If the AI has more than 40 coins, return Success
        if (gm.player2Gold > 40)
            return Result.Success;

        else
            return Result.Failure;
    }
}
