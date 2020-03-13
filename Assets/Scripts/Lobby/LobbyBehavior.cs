using Mirror;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Represents the sync object that clients used to disable their doors.
/// </summary>
[System.Serializable]
public struct PaintingIndexDoorIndex
{
    public int paintingIndex;
    public int doorIndex;
}

[System.Serializable]
public class SyncListPaintingsIndexDoorIndex : SyncList<PaintingIndexDoorIndex> { }

public class LobbyBehavior : NetworkBehaviour
{

    [Title("Spawning Settings")]
    [SerializeField] 
    private List<Transform> firstTimeSpawnTransforms = new List<Transform>();

    [Title("Painting Obtained Settings")]
    [SerializeField]
    private PaintingsObtained paintingsObtained;

    [SerializeField]
    private List<LobbyDoorBehavior> lobbyDoorBehaviors = new List<LobbyDoorBehavior>();

    [SerializeField][Required]
    private DoorBehavior finalDoorBehavior;

    [Title("Next Scene Settings")]
    [SerializeField]
    private GlobalSceneLoaderManager sceneLoaderManager;

    [SerializeField]
    private List<SceneHolder> levels = new List<SceneHolder>();

    [SerializeField]
    private SceneHolder finalScene;

    [Title("Number of Paintings Obtained Events")]
    [SerializeField]
    private List<UnityEvent> numberPaintingsObtainedEvents = new List<UnityEvent>();

    private bool cheatUsed = false;

    [SyncVar]
    private int nbPaintingsAcquired = 0;

    public override void OnStartServer()
    {
        base.OnStartServer();

        InitNbPaintingsFromData();
    }

    private void InitNbPaintingsFromData()
    {
        for(int i = 0; i < paintingsObtained.paintings.Count; i++)
        {
            PaintingObtained paintingObtained = paintingsObtained.paintings[i];
            if (paintingObtained.isPaintingAcquired.value)
            {
                nbPaintingsAcquired++;
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        ManageLobbyDoors();
    }

    private void ManageLobbyDoors()
    {
        // We are at the end of the game, unlock the last door 
        if(lobbyDoorBehaviors.Count == nbPaintingsAcquired)
        {
            if (isServer)
            {
                finalDoorBehavior.UnlockMorePlank();
            }
        }

        // Disable every except the one we need to go (same index as nb paintings obtained)
        for(int i = 0; i < lobbyDoorBehaviors.Count; i++)
        {
            if(i != nbPaintingsAcquired)
            {
                lobbyDoorBehaviors[i].DisableLobbyDoor();
            }
        }

        // We only call one event for the nb of paintings obtained
        if(nbPaintingsAcquired < numberPaintingsObtainedEvents.Count)
        {
            if (numberPaintingsObtainedEvents[nbPaintingsAcquired] != null)
            {
                numberPaintingsObtainedEvents[nbPaintingsAcquired].Invoke();
            }
        }
    }


    void Update()
    {
        if (isServer)
        {
            // --- CHEAT CODES --- 
            if (Input.GetKeyDown(KeyCode.P) && !cheatUsed)
            {
                LoadNextScene();
            }
            // ---------------------
        }
    }

    [Server]
    public void LoadNextScene()
    {
        if (!isServer)
        {
            return;
        }

        if(nbPaintingsAcquired < levels.Count) { 

            if (nbPaintingsAcquired < paintingsObtained.paintings.Count){
                paintingsObtained.paintings[nbPaintingsAcquired].isPaintingAcquired.value = true;
            }

            LoadScene(levels[nbPaintingsAcquired].scene);
        }else
        {
            LoadScene(finalScene.scene);
        }
    }

    private void LoadScene(string nextSceneName)
    {
        if (sceneLoaderManager.componentCache != null && nextSceneName != null)
        {
            sceneLoaderManager.componentCache.LoadScene(nextSceneName);
        }
        else if (nextSceneName != null)
        {
            NetworkManager networkManager = NetworkManager.singleton as NetworkManager;
            if (networkManager != null)
            {
                networkManager.ServerChangeScene(nextSceneName);
            }
        }
    }

    /// <summary>
    /// Returns the spawn points for the room. 
    /// If no painting is acquired, spawns in front of the room.
    /// If painting acquired spawn in front of the door.
    /// </summary>
    /// <returns> The spawn points. </returns>
    public List<Transform> GetSpawnsTranforms()
    {
        for (int i = paintingsObtained.paintings.Count - 1; i >= 0; i--)
        {
            PaintingObtained paintingObtained = paintingsObtained.paintings[i];
            if (paintingObtained.isPaintingAcquired.value)
            {
                if (i < 0 || i >= lobbyDoorBehaviors.Count)
                {
                    return firstTimeSpawnTransforms;
                }
                else
                {
                    return lobbyDoorBehaviors[i].spawnTransforms;
                }
            }
        }

        return firstTimeSpawnTransforms;
    }
}
