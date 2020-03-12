using UnityEngine;
using NodeCanvas.Framework;


public class HumanIsClosest : ConditionTask
{
    protected override bool OnCheck()
    {
        GhostEnemy ghostEnemy = agent.GetComponent<GhostEnemy>();

        //On vérifie s'il y a un humain dans la scène.
        if (ghostEnemy.HumanExistInScene())
        {
            Human h = ghostEnemy.HumanPlayerGameObject.GetComponent<Human>();

            if (ghostEnemy.GhostExistInScene())
            {
                GhostPlayer gp = ghostEnemy.GhostPlayerGameObject.GetComponent<GhostPlayer>();

                float distanceHuman = Vector3.Distance(h.transform.position, agent.transform.position);
                float distanceGhost = Vector3.Distance(gp.transform.position, agent.transform.position);

                if (distanceHuman >= distanceGhost) return false; // human is farther
                else return true; // human is closest

            }
            else
            {
                // Only a human in the scene
                return true;
            }
        }

        return false;
    }
}



