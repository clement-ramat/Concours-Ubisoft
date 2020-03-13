using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Sirenix.OdinInspector;
using Michsky.UI.Dark;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.EventSystems;

/// <summary>
/// Temporary script used for CharacterSelection Scene Only
/// </summary>
public class CharacterSelectionNetworkManager : NetworkManager
{
    [Header("In Game Settings")]
    public GameObject playerControllerPrefab;
    public GameObject ghostPrefab;
    public GameObject humanPrefab;

    public CharacterType p1Character;
    public CharacterType p2Character;

    [SerializeField]
    private SceneHolder gameScene;

    [Header("Managers")]
    [SerializeField]
    private GlobalSceneLoaderManager globalSceneLoaderManager;

    [Header("In Game Data")]
    [SerializeField]
    private PlayerAbilities playerAbilities;

    [SerializeField]
    private PaintingsObtained paintingsObtained;

    private int playerSpawned = 0;

    public override void OnServerConnect(NetworkConnection conn)
    {
        NetworkMenuHandler networkMenuHandler = FindObjectOfType<NetworkMenuHandler>();
        if (networkMenuHandler == null)
        {
            return;
        }

        SelectionMenuHandler selectionMenuHandler = FindObjectOfType<SelectionMenuHandler>();
        if (selectionMenuHandler == null)
        {
            return;
        }

        base.OnServerConnect(conn);


        GameObject player = Instantiate(playerPrefab);


        NetworkServer.Spawn(player);

        NetworkServer.AddPlayerForConnection(conn, player);


        player.GetComponent<PlayerMenuController>().playerId = numPlayers;
        player.GetComponent<PlayerMenuController>().selectionMenuHandler = selectionMenuHandler;

        player.GetComponent<PlayerMenuController>().TargetSetupController(conn, selectionMenuHandler.gameObject);

        networkMenuHandler.selectionMenuHandler.PlayerJoin(numPlayers);
        //networkMenuHandler.selectionMenuHandler.RpcUpdateControllers();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        NetworkMenuHandler networkMenuHandler = FindObjectOfType<NetworkMenuHandler>();

        Debug.Log(networkMenuHandler);
        if (networkMenuHandler == null)
        {
            return;
        }
        networkMenuHandler.menuManager.GetComponent<MainPanelManager>().PanelAnim(5);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        if (networkSceneName == offlineScene)
        {
            NetworkMenuHandler networkMenuHandler = FindObjectOfType<NetworkMenuHandler>();
            if (networkMenuHandler == null)
            {
                return;
            }

            SelectionMenuHandler selectionMenuHandler = FindObjectOfType<SelectionMenuHandler>();
            if (selectionMenuHandler == null)
            {
                return;
            }

            if (conn.connectionId != 0)
                selectionMenuHandler.PlayerJoin(2, false);
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        NetworkMenuHandler networkMenuHandler = FindObjectOfType<NetworkMenuHandler>();
        if (networkMenuHandler == null)
        {
            return;
        }

        networkMenuHandler.menuManager.GetComponent<MainPanelManager>().PanelAnim(0);
        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);

    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        if (!autoCreatePlayer)
        {
            if (networkSceneName != offlineScene)
            {
                if (conn.connectionId == 0)
                {
                    SpawnCharacterForPlayer(p1Character, conn);
                    playerSpawned++;
                }
                else
                {
                    SpawnCharacterForPlayer(p2Character, conn);
                    playerSpawned++;
                }
            }
        }

        if (playerSpawned >= numPlayers && globalSceneLoaderManager.componentCache != null)
        {
            globalSceneLoaderManager.componentCache.RpcHideLoader(true);//IsLoadingScreenHidden = true;
        }
    }


    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        ClientScene.AddPlayer();
    }

    /// <summary>
    /// Spawns the in-game players.
    /// </summary>
    /// <param name="characterType"></param>
    /// <param name="conn"></param>
    public void SpawnCharacterForPlayer(CharacterType characterType, NetworkConnection conn)
    {
        NetworkServer.DestroyPlayerForConnection(conn);

        GameObject playerController = Instantiate(playerControllerPrefab, GetStartPosition().position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, playerController);

        Vector3 playerPosition = GetPlayerSpawnPoint();
        if (characterType == CharacterType.Ghost)
        {
            playerController.GetComponent<PlayerController>().TargetSetCharacterType(conn, CharacterType.Ghost);

            GameObject ghost = Instantiate(ghostPrefab, playerPosition, Quaternion.identity);

            NetworkServer.Spawn(ghost);

            playerController.GetComponent<PlayerController>().myCharacter = ghost;
            playerController.GetComponent<PlayerController>().characterType = CharacterType.Ghost;

        }
        else
        {

            playerController.GetComponent<PlayerController>().TargetSetCharacterType(conn, CharacterType.Human);

            GameObject human = Instantiate(humanPrefab, playerPosition, Quaternion.identity);

            NetworkServer.Spawn(human);

            playerController.GetComponent<PlayerController>().myCharacter = human;
            playerController.GetComponent<PlayerController>().characterType = CharacterType.Human;
        }
    }

    private Vector3 GetPlayerSpawnPoint()
    {
        RoomsManager roomsManager = FindObjectOfType<RoomsManager>();
        if (roomsManager != null)
        {
            List<Transform> transforms = roomsManager.GetCurrentRoom().GetSpawnPoints();
            return transforms[Random.Range(0, transforms.Count)].position;
        }

        LobbyBehavior lobbyManager = FindObjectOfType<LobbyBehavior>();
        if (lobbyManager != null)
        {
            List<Transform> transforms = lobbyManager.GetSpawnsTranforms();
            return transforms[Random.Range(0, transforms.Count)].position;
        }

        return Vector3.zero;
    }

    /// <summary>
    /// Load the game scene.
    /// </summary>
    [Button]
    public void StartGame()
    {
        playerAbilities.Reset();
        paintingsObtained.Reset();

        if (globalSceneLoaderManager.componentCache != null)
        {
            globalSceneLoaderManager.componentCache.LoadScene(gameScene.scene.SceneName);
        }
    }
}
