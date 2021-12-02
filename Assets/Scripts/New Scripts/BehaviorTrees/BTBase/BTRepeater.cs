using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRepeater : BTDecorator
{
    public BTRepeater(BehaviorTree t, BTNode c) : base(t, c)
    {

    }

    public override Result Execute()
    {
        //Debug.Log("Child returned: " + Child.Execute());
        return Child.Execute();
    }
}
