using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSaveMoney : BTNode
{
    int random;
    public BTSaveMoney(BehaviorTree t) : base(t)
    {
        
    }

    public override Result Execute()
    {
        int random = Random.Range(0, 3);

        // If the random is 0, return Success
        if (random < 1)
            return Result.Success;

        else
            return Result.Failure;
    }
}
