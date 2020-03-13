using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NodeCanvas.BehaviourTrees;

public class NetworkedAI : NetworkBehaviour
{
    private NavMeshAgent myNavMeshAgent;
    private BehaviourTreeOwner myBTO;

    public override void OnStartClient()
    {
        base.OnStartClient();

        myBTO = GetComponent<BehaviourTreeOwner>();
        myNavMeshAgent = GetComponent<NavMeshAgent>();

        if (myBTO != null) myBTO.enabled = !isClientOnly;
        if (myNavMeshAgent != null) myNavMeshAgent.enabled = !isClientOnly;
    }

}
