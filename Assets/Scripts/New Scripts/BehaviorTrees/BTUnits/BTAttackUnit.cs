using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTAttackUnit : BTNode
{
    Unit unit;
    bool attack = false;

    public BTAttackUnit(BehaviorTree t, Unit _unit) : base (t)
    {
        unit = _unit;
    }

    bool Attack()
    {
        if (unit.enemiesInRange.Count == 0)
            return false;

        foreach (Unit enemy in unit.enemiesInRange)
        {
            if (enemy.isRedKing)
            {
                unit.Attack(enemy);
                return true;
            }
        }

        unit.Attack(unit.enemiesInRange[Random.Range(0, unit.enemiesInRange.Count)]);
        return true;
    }

    public override Result Execute()
    {
        //Debug.Log("attack");
        attack = Attack();

        if (attack)
            return Result.Success;
        else
            return Result.Failure;
    }
}
