using UnityEngine;
using NodeCanvas.Framework;



public class ForceUngrab: ActionTask
{
    protected override void OnExecute()
    {
        Human h;
        GhostEnemy ghostEnemy = agent.GetComponent<GhostEnemy>();
        if (ghostEnemy.HumanExistInScene())
        {
            h = ghostEnemy.HumanPlayerGameObject.GetComponent<Human>();
               
        }else
        {
            EndAction(false);
            return;
        }

        float distance = Vector3.Distance(h.transform.position, agent.transform.position);
        if (distance < 5f) 
        {
            // human.unGrab
            // THEN
            // ghostEnemy.Possess
            if (h.Grabber.IsGrabbingObject())
            {
                //GameObject go = h.GrabbedObject.gameObject;
                h.Grabber.ManageGrab(h.interactionArea);

                //PossessInteraction pi = go.GetComponent<PossessInteraction>();
                //if (pi)
                //{
                //    if (ghostEnemy.Possess())
                //    {
                //        //Debug.Log("possess success");
                //        // Possession successful
                //        ghostEnemy.UseObjectMeshAgent();
                //        EndAction(true);
                //    }
                //}
            }
        }
        else
        {
            EndAction(false);
            return;
        }

        EndAction(true);
    }
}

