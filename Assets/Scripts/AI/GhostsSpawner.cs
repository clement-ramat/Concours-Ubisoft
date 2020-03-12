using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirteenPixels.Soda;
using UnityEngine.Events;

public class GhostsSpawner : NetworkBehaviour
{

    [SerializeField]
    private List<Transform> spawnTransforms = new List<Transform>();

    [SerializeField]
    private List<GameObject> ghostToSpawnPrefabs = new List<GameObject>();

    [SerializeField]
    private RuntimeSetGameObject ghostsRuntimeSet;

    [SerializeField]
    private UnityEvent OnGhostsDefeated;

    public override void OnStartServer()
    {
        base.OnStartServer();

        ghostsRuntimeSet.onElementCountChange.AddResponse(OnEnemiesGhostCountChange);
    }

    public void SpawnGhosts()
    {
        if (isServer)
        {
            GhostRespawner gr = GetComponent<GhostRespawner>();

            for (int i = 0; i < ghostToSpawnPrefabs.Count; i++)
            {
                GameObject newGhost = Instantiate(ghostToSpawnPrefabs[i], spawnTransforms[i % spawnTransforms.Count].position, Quaternion.identity);

                if(gr != null)
                {
                    newGhost.GetComponent<GhostEnemy>().GhostRespawner = gr;
                }

                RuntimeSetElement runtimeSetElement = newGhost.AddComponent<RuntimeSetElement>();
                runtimeSetElement.runtimeSet = ghostsRuntimeSet;

                runtimeSetElement.enabled = true;

                NetworkServer.Spawn(newGhost);
            }
        }

        // If we don t still have ghosts, something is wrong sowe invoke the event like the ghosts has been defeated
        if (ghostToSpawnPrefabs.Count == 0)
            {
                if (OnGhostsDefeated != null)
                {
                    OnGhostsDefeated.Invoke();
                }
            }
    }

    private void OnEnemiesGhostCountChange(int newCount)
    {
        if(newCount == 0)
        {
            if(OnGhostsDefeated != null)
            {
                OnGhostsDefeated.Invoke();
            }
        }
    }
}
