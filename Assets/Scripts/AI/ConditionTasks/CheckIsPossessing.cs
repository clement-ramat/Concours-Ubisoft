using UnityEngine;
using NodeCanvas.Framework;


public class CheckIsPossessing : ConditionTask
{
    protected override bool OnCheck()
    {
        return agent.GetComponent<Ghost>().IsPossessing();
    }
}



