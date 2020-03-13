using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;
using Sirenix.OdinInspector;

public class RoomsManager : NetworkBehaviour
{


    [Title("Global Players Settings")]
    [SerializeField]
    private GlobalPlayerGhost globalPlayerGhost;

    [SerializeField]
    private GlobalPlayerHuman globalPlayerHuman;

    [Title("Rooms Settings")]
    [SerializeField]
    private List<RoomBehavior> roomBehaviors = new List<RoomBehavior>();

    [SerializeField]
    private float playersTransitionTime = 1.75f;

    [Title("Next Scene Settings")]

    [SerializeField]
    private GlobalSceneLoaderManager sceneLoaderManager;

    [SerializeField]
    private SceneHolder nextScene;

    [SyncVar (hook = nameof(OnClientCurrentRoomChanged))]
    private int currentRoomIndex = 0;


    private void Start()
    {
        // We deactivate every rooms expect the first one
        for (int i = 0; i < roomBehaviors.Count; i++)
        {
            roomBehaviors[i].Reset();
        }

        if(currentRoomIndex >= 0 && currentRoomIndex < roomBehaviors.Count)
        {
            roomBehaviors[currentRoomIndex].ActivateRoom();
        }
    }

    private void Update()
    {
        if (!isServer)
        {
            return;
        }

        // CHEAT TO OPEN THE DOOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            GetCurrentRoom().finishDoor.Open();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            EndLevel();
        }
    }

    public RoomBehavior GetCurrentRoom()
    {
        return roomBehaviors[currentRoomIndex];
    }

    [Server]
    public void GoNextRoom()
    {
        if (currentRoomIndex + 1 < roomBehaviors.Count)
        {
            // We go on the next room

            // We disable the current room (the room is over)
            roomBehaviors[currentRoomIndex].CloseRoom(isServer);

            currentRoomIndex++;

            // We enable the next room
            roomBehaviors[currentRoomIndex].ActivateRoom();

            // We move the two players in the next room
            if (globalPlayerGhost.componentCache != null)
            {
                MovePlayerIntoNextRoom(globalPlayerGhost.componentCache, roomBehaviors[currentRoomIndex - 1], roomBehaviors[currentRoomIndex]);
            }
            if (globalPlayerHuman.componentCache != null)
            {
                MovePlayerIntoNextRoom(globalPlayerHuman.componentCache, roomBehaviors[currentRoomIndex - 1], roomBehaviors[currentRoomIndex]);
            }
        }
        else
        {
            // We cannot go on the next room, but we fake the movement anyway.

            // We move the two players in the next room
            if (globalPlayerGhost.componentCache != null)
            {
                MovePlayerIntoNextRoom(globalPlayerGhost.componentCache, roomBehaviors[currentRoomIndex]);
            }
            if (globalPlayerHuman.componentCache != null)
            {
                MovePlayerIntoNextRoom(globalPlayerHuman.componentCache, roomBehaviors[currentRoomIndex]);
            }

            // At the end of the movement, load the next scene.
            Sequence mySequence = DOTween.Sequence();
            mySequence.AppendInterval(playersTransitionTime).AppendCallback(() =>
            {
                EndLevel();
            });
        }
    }

    
    [Server]
    private void MovePlayerIntoNextRoom(Character character, RoomBehavior room1, RoomBehavior room2 = null)
    {

        List<Transform> transforms = room1.GetNextRoomPoints();
        if (transforms.Count == 0)
        {
            if(room2 == null)
            {
                Debug.LogError("HAVE NO WHERE TO GO, CANNOT FAKE MOVEMENT");

                // We have no where to go, cancel the function
                return;
            }

            transforms = room2.GetSpawnPoints();
            if (transforms.Count == 0)
            {
                Debug.LogError("HAVE NO WHERE TO GO, CANNOT MOVE NEXT ROOM SPAWN POINTS");
                // We have no where to go, cancel the function
                return;
            }
        }

        // Reset the character
        character.ResetCharacter();

        // Stop him from mouving
        CharacterMovement characterMovement = character.GetComponent<CharacterMovement>();
        if (characterMovement != null)
        {
            characterMovement.CanMove = false;
        }

        // Find the position in the next room and make him go there
        Transform transformToGo = transforms[Random.Range(0, transforms.Count)];

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(character.transform.DOMove(transformToGo.position, playersTransitionTime)).AppendCallback(() => {
            if (characterMovement != null)
            {
                characterMovement.CanMove = true;
            }

            character.transform.position = transformToGo.position;
        });
    }

    /// <summary>
    /// Hook function called on client when currentRoomIndex changed.
    /// Close old room and activate the new one.
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnClientCurrentRoomChanged(int oldValue, int newValue)
    {
        if (!isClientOnly)
        {
            return;
        }

        roomBehaviors[oldValue].CloseRoom(false);
        roomBehaviors[newValue].ActivateRoom();
    }

    /// <summary>
    /// Simply the load the next scene via the scene loader manager.
    /// Called via the end of level event or when we fake the movement to the next room.
    /// </summary>
    public void EndLevel()
    {
        if(sceneLoaderManager.componentCache != null && nextScene != null)
        {
            sceneLoaderManager.componentCache.LoadScene(nextScene.scene);
        }else if (nextScene != null)
        {
            NetworkManager networkManager = NetworkManager.singleton as NetworkManager;
            if (networkManager != null)
            {
                networkManager.ServerChangeScene(nextScene.scene);
            }
        }
    }
}
