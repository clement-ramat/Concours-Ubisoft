using UnityEngine;
using System.Collections.Generic;
using Rewired;
using Doozy.Engine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.Events;

[AddComponentMenu("")]
public class SelectionMenuHandler : NetworkBehaviour
{


    public GameObject verticalLayoutRight;
    public GameObject verticalLayoutLeft;
    public GameObject verticalLayoutCenter;
    public GameObject manette1;
    public GameObject manette2;


    private int numberPlayersReady = 0;
    private bool enableJoinGame = true;
    private List<int> listPlayerID = new List<int>();

    [SyncVar(hook = nameof(UpdateP1Join))]
    private bool p1joined = false;
    [SyncVar(hook = nameof(UpdateP2Join))]
    private bool p2joined = false;

    [SyncVar(hook = nameof(UpdateP1Chosen))]
    private bool p1HasChosen = false;
    [SyncVar(hook = nameof(UpdateP2Chosen))]
    private bool p2HasChosen = false;

    [SyncVar(hook = nameof(UpdateP1Position))]
    private int p1placement = 1; //0 for left, 1 for center, 2 for right
    [SyncVar(hook = nameof(UpdateP2Position))]
    private int p2placement = 1;


    public int maxPlayers = 2;

    private List<PlayerMap> playerMap; // Maps Rewired Player ids to game player ids

    private int gamePlayerIdCounter = 0;
    public CharacterSelectionNetworkManager networkManager;



    public override void OnStartServer()
    {
        base.OnStartServer();

        p1joined = false;
        p2joined = false;
    }

        
    public void UpdateP1Position(int oldValue, int newValue)
    {
        if (oldValue != newValue)
        {
            if (newValue == 0)
            {
                manette1.transform.parent = verticalLayoutLeft.transform;
            }
            else if (newValue == 1)
            {
                manette1.transform.parent = verticalLayoutCenter.transform;
            }
            else if (newValue == 2)
            {
                manette1.transform.parent = verticalLayoutRight.transform;
            }
        }
    }

    public void UpdateP2Position(int oldValue, int newValue)
    {
        if (oldValue != newValue)
        {
            if (newValue == 0)
            {
                manette2.transform.parent = verticalLayoutLeft.transform;
            }
            else if (newValue == 1)
            {
                manette2.transform.parent = verticalLayoutCenter.transform;
            }
            else if (newValue == 2)
            {
                manette2.transform.parent = verticalLayoutRight.transform;
            }
        }
    }

    public void UpdateP1Join(bool oldValue, bool newValue)
    {
        manette1.SetActive(newValue);
    }

    public void UpdateP2Join(bool oldValue, bool newValue)
    {
        manette2.SetActive(newValue);
    }


    public void UpdateP1Chosen(bool oldValue, bool newValue)
    {
        if(oldValue != newValue)
        {
            if(newValue == true)
            {
                manette1.GetComponent<UIButton>().ExecuteClick();
                manette1.GetComponent<Button>().interactable = false;
            }
            else
            {
                manette1.GetComponent<Button>().interactable = true;
            }
        }
    }


    public void UpdateP2Chosen(bool oldValue, bool newValue)
    {
        if (oldValue != newValue)
        {
            if (newValue == true)
            {
                manette2.GetComponent<UIButton>().ExecuteClick();
                manette2.GetComponent<Button>().interactable = false;
            }
            else
            {
                manette2.GetComponent<Button>().interactable = true;
            }
        }
    }

    void Awake()
    {
        playerMap = new List<PlayerMap>();
    }

    private void Start()
    {
        networkManager = NetworkManager.singleton as CharacterSelectionNetworkManager;
    }

    //Lorsqu'on bouge le joystick à gauche
    public void MoveControllerLeft(int i)
    {
        if (i == 1 && p1joined && !p1HasChosen)
        {
            if (p1placement != 0)
            {
                if (!(p2placement == 0 && p2HasChosen)) //On peut choisir son perso seulement si l'autre a pas encore choisi, sinon t'es attribué d'office
                {
                    p1placement = 0;
                }
            }
        }

        if (i == 2 && p2joined && !p2HasChosen)
        {
            if (p2placement != 0)
            {
                if (!(p1placement == 0 && p1HasChosen))
                {
                    p2placement = 0;
                }
            }
        }
    }

    //Lorsqu'on bouge le joystrick à droite
    public void MoveControllerRight(int i)
    {
        if (i == 1 && p1joined && !p1HasChosen)
        {
            if (p1placement != 2)
            {
                if (!(p2placement == 2 && p2HasChosen))
                {
                    //manette1.transform.SetParent(verticalLayoutRight.transform);
                    p1placement = 2;
                }

            }
        }

        if (i == 2 && p2joined && !p2HasChosen)
        {
            if (p2placement != 2)
            {
                if (!(p1placement == 2 && p1HasChosen))
                {
                    //manette2.transform.SetParent(verticalLayoutRight.transform);
                    p2placement = 2;
                }

            }
        }
    }



    public void ChooseCharacter(int i)
    {
        if (i == 1 && p1placement != 1 && !p1HasChosen)
        {
            if (p1placement == 0)
            {
                if (p2joined && p2placement == 0)
                {
                    p2placement = 2;
                }
            }
            else if (p1placement == 2)
            {
                if (p2joined && p2placement == 2)
                {
                    p2placement = 0;
                }
            }
            p1HasChosen = true;
        }


        else if (i == 2 && p2placement != 1 && !p2HasChosen)
        {
            if (p2placement == 0)
            {
                if (p1joined && p1placement == 0)
                {
                    manette1.transform.SetParent(verticalLayoutRight.transform);
                    p1placement = 2;
                }

            }
            else if (p2placement == 2)
            {
                if (p1joined && p1placement == 2)
                {
                    p1placement = 0;
                }

            }
            p2HasChosen = true;
        }
    }


    public void CancelSelection(int playerId)
    {
        if (playerId == 1 && p1joined && p1HasChosen)
        {
            p1HasChosen = false;
        }
        else if (playerId == 2 && p2joined && p2HasChosen)
        {
            p2HasChosen = false;
        }
    }

    public bool CanStartGame()
    {
        return (p2HasChosen && p1HasChosen);

    }

    public void StartGame()
    {
        if (CanStartGame())
        {


            if (p1placement == 0)
                networkManager.p1Character = CharacterType.Human;
            else if (p1placement == 2)
                networkManager.p1Character = CharacterType.Ghost;

            if (p2placement == 0)
                networkManager.p2Character = CharacterType.Human;
            else if (p2placement == 2)
                networkManager.p2Character = CharacterType.Ghost;


            networkManager.StartGame();
        }
    }

 

    public void PlayerJoin(int playerId, bool joined = true)
    {
        if (playerId == 1)
        {
            p1joined = joined;
            ResetController(1);
        }
        else if (playerId == 2)
        {
            p2joined = joined;
            ResetController(2);
        }
    }

    public void ResetController(int playerId)
    {
        if (playerId == 1)
        {
            p1HasChosen = false;
            p1placement = 1;
        }
        else if (playerId == 2)
        {
            p2HasChosen = false;
            p2placement = 1;
        }
    }




    public void EnableJoinGame()
    {
        enableJoinGame = true;
    }

    void AssignNextPlayer(int rewiredPlayerId)
    {
        if (playerMap.Count >= maxPlayers)
        {
            Debug.LogError("Max player limit already reached!");
            return;
        }

        int gamePlayerId = GetNextGamePlayerId();

        // Add the Rewired Player as the next open game player slot
        playerMap.Add(new PlayerMap(rewiredPlayerId, gamePlayerId));

        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        // Disable the Assignment map category in Player so no more JoinGame Actions return
        //rewiredPlayer.controllers.maps.SetMapsEnabled(false, "Assignment");

        // Enable UI control for this Player now that he has joined
        //rewiredPlayer.controllers.maps.SetMapsEnabled(true, "Default");

        Debug.Log("Added Rewired Player id " + rewiredPlayerId + " to game player " + gamePlayerId);
    }

    private int GetNextGamePlayerId()
    {
        return gamePlayerIdCounter++;
    }

    // This class is used to map the Rewired Player Id to your game player id
    private class PlayerMap
    {
        public int rewiredPlayerId;
        public int gamePlayerId;

        public PlayerMap(int rewiredPlayerId, int gamePlayerId)
        {
            this.rewiredPlayerId = rewiredPlayerId;
            this.gamePlayerId = gamePlayerId;

        }
    }




}
