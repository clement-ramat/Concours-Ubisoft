using UnityEngine;
using NodeCanvas.Framework;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/*
 * Doc
 * http://www.paradoxnotion.com/files/nodecanvas/NodeCanvas%20Documentation.pdf
 */

public class Possess: ActionTask<NavMeshAgent>
{
    [SerializeField]
    private BBParameter<GameObject> nearestPossessable;

    [SerializeField]
    public BBParameter<bool> nearestLightweight;

    protected override void OnExecute()
    {
        GhostEnemy ghost = agent.GetComponent<GhostEnemy>();
       
        if (ghost.Possess())
        {
            //Debug.Log("possess success");
            // Possession successful
            ghost.UseObjectMeshAgent();
            EndAction(true);
        }
        else
        {
            //Debug.Log("possess fail");
            // Fail
            FindNearestPossessable();
            ghost.nearestPossessable = nearestPossessable.value;
            //Debug.Log("nearest: " + ghost.nearestPossessable);
            EndAction(false);
        }
    }

    private void FindNearestPossessable()
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = agent.transform.position;

        // Finding all possessable objects
        List<PossessInteraction> possessables = new List<PossessInteraction>();
        if (nearestLightweight.value)
        {
            possessables.AddRange(Object.FindObjectsOfType<LightPossessInteraction>());
        }
        else
        {
            possessables.AddRange(Object.FindObjectsOfType<PossessInteraction>());
        }

        // Looking for the nearest possessable object
        foreach (PossessInteraction p in possessables)
        {
            Transform t = p.transform;

            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }

        if(tMin != null) nearestPossessable.value = tMin.gameObject;
    }
}
