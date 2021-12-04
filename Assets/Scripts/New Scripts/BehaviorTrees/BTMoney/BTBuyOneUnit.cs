using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTBuyOneUnit : BTNode
{
    GM gm;
    CharacterCreation characterCreation;
    Unit selectedUnit = null;

    public BTBuyOneUnit(BehaviorTree t) : base(t)
    {
        gm = GameObject.Find("GameMaster").GetComponent<GM>();
        characterCreation = GameObject.Find("GameMaster").GetComponent<CharacterCreation>();
    }

    public Unit BuyUnit()
    {
        if (gm.player2Gold >= 100)
        {
            selectedUnit = gm.blueVillage.GetComponent<Unit>();
            characterCreation.BuyVillage(gm.blueVillage.GetComponent<Village>());
            //Debug.Log("Buying village");
        }

        else if (gm.player2Gold >= 90)
        {
            selectedUnit = gm.blueArcher.GetComponent<Unit>();
            characterCreation.BuyUnit(gm.blueArcher.GetComponent<Unit>());
            //Debug.Log("Buying archer");
        }

        else if (gm.player2Gold >= 70)
        {
            selectedUnit = gm.blueDragon.GetComponent<Unit>();
            characterCreation.BuyUnit(gm.blueDragon.GetComponent<Unit>());
            //Debug.Log("Buying dragon");
        }

        else
        {
            selectedUnit = gm.blueKnight.GetComponent<Unit>();
            characterCreation.BuyUnit(gm.blueKnight.GetComponent<Unit>());
            //Debug.Log("Buying knight");
        }

        return selectedUnit;
    }

    public override Result Execute()
    {
        selectedUnit = BuyUnit();
        //Debug.Log("Buy one");

        // If a unit can be bought and placed, return Success
        if (selectedUnit != null)
        {
            return Result.Success;
        }
            

        else
            return Result.Failure;
    }
}
