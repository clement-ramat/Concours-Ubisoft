using System.Collections;
using UnityEngine;
using Mirror;
using NodeCanvas.BehaviourTrees;

public class GhostRespawner : NetworkBehaviour
{
    private GameObject g;

    public void ReviveGhost(GameObject ghost, float reviveTime)
    {
        g = ghost;
        if (isServer) StartCoroutine(ReviveGhost_Coroutine(reviveTime));
    }

    private IEnumerator ReviveGhost_Coroutine(float reviveTime)
    {
        RpcReviveDisable(g);
        yield return new WaitForSeconds(reviveTime);
        RpcReviveEnable(g);
    }


    private void ReviveDisable(GameObject ghost)
    {
        ghost.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        ghost.SetActive(false);
    }


    private void ReviveEnable(GameObject ghost)
    {
        ghost.SetActive(true);

        GhostEnemy ge = ghost.GetComponent<GhostEnemy>();
        ge.RpcResetParticles();
        ghost.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        ge.GetComponent<BehaviourTreeOwner>().StartBehaviour();
    }


    [ClientRpc]
    private void RpcReviveDisable(GameObject ghost)
    {
        ReviveDisable(ghost);
    }


    [ClientRpc]
    private void RpcReviveEnable(GameObject ghost)
    {
        ReviveEnable(ghost);
    }


    private IEnumerator WaitFor(float s)
    {
        yield return new WaitForSeconds(s);
    }
}
