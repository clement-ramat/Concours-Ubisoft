using UnityEngine;
using NodeCanvas.Framework;


public class CheckIsFlashed : ConditionTask
{
    protected override bool OnCheck()
    {
        if(agent.GetComponent<GhostEnemy>().IsFlashed) Debug.Log("IsFlashed");
        return agent.GetComponent<GhostEnemy>().IsFlashed;
    }
}