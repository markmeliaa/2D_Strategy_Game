using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : MonoBehaviour
{
    private BTNode mRoot;
    private BTNode uRoot;

    public Dictionary<string, object> Blackboard { get; set; }
    public BTNode MRoot { get { return mRoot; } }
    public BTNode URoot { get { return uRoot; } }

    // Start is called before the first frame update
    void Awake()
    {
        Blackboard = new Dictionary<string, object>();
        Blackboard.Add("WorldBounds", new Rect(0, 0, 5, 5));

        mRoot = new BTRepeater(this, new BTSequencer(this, new BTNode[] { new BTCheckMoney(this),
            new BTRepeater(this, new BTSelector(this, new BTNode[] { new BTSaveMoney(this),
                new BTRepeater(this, new BTSelector(this, new BTNode[] {
                    new BTRepeater(this, new BTSequencer(this, new BTNode[] { new BTMore5Units(this), new BTBuyOneUnit(this) })),
                    new BTRepeater(this, new BTSequencer(this, new BTNode[] { new BTLess5Units(this), new BTBuyMoreUnits(this) })) })) })) }));
    }

    public IEnumerator RunBehavior(BTNode root)
    {  
        BTNode.Result result = root.Execute();

        while (result == BTNode.Result.Running)
        {
            //Debug.Log("Root result: " + result);
            yield return null;
            result = root.Execute();
        }

        Debug.Log("Behavior has finished with: " + result);
    }
}
