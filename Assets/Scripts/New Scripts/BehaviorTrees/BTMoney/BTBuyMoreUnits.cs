using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTBuyMoreUnits : BTNode
{
    GM gm;
    CharacterCreation characterCreation;
    List<Unit> selectedUnits = new List<Unit>();

    public BTBuyMoreUnits(BehaviorTree t) : base(t)
    {
        gm = GameObject.Find("GameMaster").GetComponent<GM>();
        characterCreation = GameObject.Find("GameMaster").GetComponent<CharacterCreation>();
    }

    public List<Unit> BuyUnits()
    {
        while (gm.player2Gold > 40)
        {
            if (gm.player2Gold > 100)
            {
                selectedUnits.Add(gm.blueVillage.GetComponent<Unit>());
                characterCreation.BuyVillage(gm.blueVillage.GetComponent<Village>());
                //Debug.Log("Buying village");
            }

            else if (gm.player2Gold > 90)
            {
                selectedUnits.Add(gm.blueArcher.GetComponent<Unit>());
                characterCreation.BuyUnit(gm.blueArcher.GetComponent<Unit>());
                //Debug.Log("Buying archer");
            }

            else if (gm.player2Gold > 70)
            {
                selectedUnits.Add(gm.blueDragon.GetComponent<Unit>());
                characterCreation.BuyUnit(gm.blueDragon.GetComponent<Unit>());
                //Debug.Log("Buying dragon");
            }

            else
            {
                selectedUnits.Add(gm.blueKnight.GetComponent<Unit>());
                characterCreation.BuyUnit(gm.blueKnight.GetComponent<Unit>());
                //Debug.Log("Buying knight");
            }
        }
        
        return selectedUnits;
    }

    public override Result Execute()
    {
        selectedUnits = BuyUnits();
        Debug.Log("Buy more");

        // If a unit can be bought and placed, return Success
        if (selectedUnits.Count > 0)
        {
            return Result.Success;
        }


        else
            return Result.Failure;
    }
}
