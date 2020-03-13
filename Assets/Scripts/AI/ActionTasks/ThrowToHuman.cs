using UnityEngine;
using NodeCanvas.Framework;


 
public class ThrowToHuman : ActionTask
{
    protected override void OnExecute()
    {
        GhostEnemy ghostEnemy = agent.GetComponent<GhostEnemy>();

        //On vérifie s'il y a un humain dans la scène.
        if (ghostEnemy.HumanExistInScene())
        {
            Human h = ghostEnemy.HumanPlayerGameObject.GetComponent<Human>();
            
            Vector3 targetDirection = h.transform.position - ghostEnemy.transform.position;
            Vector3 oppositeDirection = ghostEnemy.transform.position - h.transform.position;

            Vector3 dir = Vector3.SlerpUnclamped(targetDirection, oppositeDirection, Random.Range(0.25f, 0.75f));
            
            //Debug.Log("*****");
            //Debug.Log(targetDirection);
            //Debug.Log(oppositeDirection);
            //Debug.Log(dir);
            // launches possessed object in a straight line
            ghostEnemy.UseGhostMeshAgent();

            ghostEnemy.LaunchPossessedObject(dir);

            EndAction(true);

        }
        else
        {
            // TEST PURPOSES ONLY
            //ghostEnemy.UseGhostMeshAgent();
            //if (ghostEnemy.UnPossess())
            //{
            //    //Debug.Log("unpossess success");
            //    // Unpossession successful
            //    EndAction(true);
            //}
            // TEST PURPOSES ONLY

            EndAction(false);
        };
    }
}




