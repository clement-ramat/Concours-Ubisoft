using UnityEngine;
using NodeCanvas.Framework;


public class GhostPlayerIsPossessing : ConditionTask
{
    protected override bool OnCheck()
    {
        GhostEnemy ghostEnemy = agent.GetComponent<GhostEnemy>();

        if (ghostEnemy.GhostExistInScene())
            return ghostEnemy.GhostPlayerGameObject.GetComponent<GhostPlayer>().IsPossessing();
        else
            return false;
    }
}



