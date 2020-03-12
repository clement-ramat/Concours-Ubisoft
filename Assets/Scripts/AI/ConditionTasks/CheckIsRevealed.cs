using UnityEngine;
using NodeCanvas.Framework;


public class CheckIsRevealed : ConditionTask
{
    protected override bool OnCheck()
    {
        return agent.GetComponent<GhostEnemy>().IsRevealed;
    }
}