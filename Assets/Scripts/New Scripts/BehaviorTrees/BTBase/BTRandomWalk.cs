using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRandomWalk : BTNode
{
    protected Vector3 NextDestination { get; set; }
    public float speed = 3.0f;
    public BTRandomWalk(BehaviorTree t) : base(t)
    {
        NextDestination = Vector3.zero;
        FindNextDestination();
    }

    public bool FindNextDestination()
    {
        object o;
        bool found = false;
        found = Tree.Blackboard.TryGetValue("WorldBounds", out o);
        if (found)
        {
            Rect bounds = (Rect)o;
            float x = UnityEngine.Random.value * bounds.width;
            float y = UnityEngine.Random.value * bounds.height;
            NextDestination = new Vector3(x, y, NextDestination.z);
        }

        return found;
    }

    public override Result Execute()
    {
        // If we've arrived at the point, then find the next destination
        if (Tree.gameObject.transform.position == NextDestination)
        {
            if (!FindNextDestination())
                return Result.Failure;
            else
                return Result.Success;
        }

        else
        {
            Tree.gameObject.transform.position = 
                Vector3.MoveTowards(Tree.gameObject.transform.position, NextDestination, Time.deltaTime * speed);

            return Result.Running;
        }
    }
}
