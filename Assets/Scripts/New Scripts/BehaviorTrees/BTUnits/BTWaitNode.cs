using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTWaitNode : BTNode
{
    bool start = false;
    public BTWaitNode(BehaviorTree t) : base(t)
    {

    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f);
    }

    public override Result Execute()
    {
        if (!start)
            return Result.Running;
        else
            return Result.Success;
    }
}
