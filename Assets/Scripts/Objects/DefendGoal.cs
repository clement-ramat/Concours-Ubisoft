using UnityEngine;
using Mirror;

public class DefendGoal : NetworkBehaviour
{
    private bool itemSpawned = false;

    [SerializeField]
    private GameObject[] particleEffects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Football 1")
        {
            foreach(GameObject go in particleEffects)
            {
                go.GetComponent<ParticleSystem>().Play();
            }

            if(!itemSpawned)
            {
                SpawnItem();
                itemSpawned = true;
            }
            
        }
    }


    void SpawnItem()
    {
        if (isServer) GetComponent<EnigmeReward>().DropReward();
    }
}
