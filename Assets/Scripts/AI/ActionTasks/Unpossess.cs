using UnityEngine;
using NodeCanvas.Framework;
using UnityEngine.AI;

/*
 * Doc
 * http://www.paradoxnotion.com/files/nodecanvas/NodeCanvas%20Documentation.pdf
 */

public class Unpossess : ActionTask<NavMeshAgent>
{

    protected override void OnExecute()
    {
        GhostEnemy ghost = agent.GetComponent<GhostEnemy>();
        if(ghost.IsPossessing())
        {
            ghost.UseGhostMeshAgent();
            if (ghost.UnPossess())
            {
                //Debug.Log("unpossess success");
                // Unpossession successful
                EndAction(true);
            }
            else
            {
                //Debug.Log("unpossess fail");
                // Fail
                EndAction(false);
            }
        }
        else 
        {
            // Was not possessing anything
            EndAction(true);
        }
    }
}



