using UnityEngine;
using NodeCanvas.Framework;


public class PossessedIsLight : ConditionTask
{
    protected override bool OnCheck()
    {
        return agent.GetComponent<GhostEnemy>().IsPossessedLight();
    }
}



