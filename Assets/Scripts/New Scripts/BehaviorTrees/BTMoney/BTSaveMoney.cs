using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSaveMoney : BTNode
{
    int random;
    public BTSaveMoney(BehaviorTree t, int _random) : base(t)
    {
        random = _random;
    }

    public override Result Execute()
    {
        Debug.Log(random + " global");

        // If the random is 0, return Success
        if (random < 1)
            return Result.Success;

        else
            return Result.Failure;
    }
}
