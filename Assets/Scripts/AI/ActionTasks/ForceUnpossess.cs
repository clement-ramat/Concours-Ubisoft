using UnityEngine;
using NodeCanvas.Framework;



public class ForceUnpossess : ActionTask
{
    public BBParameter<float> animationDelay = 0.5f;

    protected override void OnExecute()
    {

        GhostEnemy ghostEnemy = agent.GetComponent<GhostEnemy>();
        ghostEnemy.PlayAttackAnimation();



    }

    protected override void OnUpdate()
    {
        if (elapsedTime >= animationDelay.value)
        {
            GhostEnemy ghostEnemy = agent.GetComponent<GhostEnemy>();
            GhostPlayer g;
            if (ghostEnemy.GhostExistInScene())
            {
                g = ghostEnemy.GhostPlayerGameObject.GetComponent<GhostPlayer>();
            }
            else
            {
                EndAction(false);
                return;
            }

            float distance = Vector3.Distance(g.transform.position, agent.transform.position);
            if (distance < 3f)
            {
                // if player is possessing
                // forceUnpossess

                ghostEnemy.PlayAttackAnimation();

                if (g.IsPossessing())
                {

                    //GameObject go = g.GetPossessedObject();
                    //if(go != null)
                    //{
                    // Force unpossess
                    g.ForceUnPossess();

                    // steal
                    //if (ghostEnemy.Possess())
                    //{
                    //    ghostEnemy.UseObjectMeshAgent();
                    //    EndAction(true);
                    //}
                    //}
                }
            }
            else
            {
                EndAction(false);
                return;
            }


            Debug.Log("Me mad, me StealGhost");
            EndAction(true);
        }
    }

}




