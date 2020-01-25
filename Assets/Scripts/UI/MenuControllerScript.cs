using UnityEngine;
using System.Collections.Generic;
using Rewired;
using Doozy.Engine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[AddComponentMenu("")]
public class MenuControllerScript : MonoBehaviour
{
  public GameObject verticalLayoutRight;
  public GameObject verticalLayoutLeft;
  public GameObject verticalLayoutCenter;
  public GameObject manette1;
  public GameObject manette2;
  private int numberPlayersReady = 0;
  private bool enableJoinGame = true;
  private List<int> listPlayerID = new List<int>();
  private static MenuControllerScript instance;

  private bool p1joined = false;
  private bool p2joined = false;
  private bool p1HasChosen = false;
  private bool p2HasChosen = false;
  private int p1placement = 1; //0 for left, 1 for center, 2 for right
  private int p2placement = 1;


  public int maxPlayers = 2;

  private List<PlayerMap> playerMap; // Maps Rewired Player ids to game player ids
  private int gamePlayerIdCounter = 0;

  public static Rewired.Player GetRewiredPlayer(int gamePlayerId)
  {
    if (!Rewired.ReInput.isReady) return null;
    if (instance == null)
    {
      Debug.LogError("Not initialized. Do you have a PressStartToJoinPlayerSelector in your scene?");
      return null;
    }
    for (int i = 0; i < instance.playerMap.Count; i++)
    {
      if (instance.playerMap[i].gamePlayerId == gamePlayerId) return ReInput.players.GetPlayer(instance.playerMap[i].rewiredPlayerId);
    }
    return null;
  }


  void Awake()
  {
    playerMap = new List<PlayerMap>();
    instance = this; // set up the singleton
  }

  //Lorsqu'on bouge le joystick à gauche
  void MoveControllerLeft(int i)
  {
    if (i == 0 && p1joined && !p1HasChosen)
    {
      if (p1placement != 0)
      {
        if(!(p2placement == 0 && p2HasChosen)) //On peut choisir son perso seulement si l'autre a pas encore choisi, sinon t'es attribué d'office
        {
          manette1.transform.SetParent(verticalLayoutLeft.transform);
          p1placement = 0;
        }
      }
    }

    if (i == 1 && p2joined && !p2HasChosen)
    {
      if (p2placement != 0)
      {
        if (!(p1placement == 0 && p1HasChosen))
        {
          manette2.transform.SetParent(verticalLayoutLeft.transform);
          p2placement = 0;
        }
      }
    }
  }

  //Lorsqu'on bouge le joystrick à droite
  void MoveControllerRight(int i)
  {
    if (i == 0 && p1joined && !p1HasChosen)
    {
      if (p1placement != 2)
      {
        if (!(p2placement == 2 && p2HasChosen))
        {
          manette1.transform.SetParent(verticalLayoutRight.transform);
          p1placement = 2;
        }

      }
    }

    if (i == 1 && p2joined && !p2HasChosen)
    {
      if (p2placement != 2)
      {
        if (!(p1placement == 2 && p1HasChosen))
        {
          manette2.transform.SetParent(verticalLayoutRight.transform);
          p2placement = 2;
        }

      }
    }
  }


  //Lorsqu'on appuie sur A pour valider son choix (ca bouge l'autre joueur si il était sur le même choix)
  void ChooseCharacter(int i)
  {
    if (i == 0 && p1placement != 1 && !p1HasChosen)
    {
      if (p1placement == 0)
      {
        if (p2joined && p2placement == 0)
        {
          manette2.transform.SetParent(verticalLayoutRight.transform);
          p2placement = 2;
        }
        manette1.GetComponent<UIButton>().ExecuteClick();
        manette1.GetComponent<Button>().interactable = false;
      }
      else if (p1placement == 2)
      {
        if (p2joined && p2placement == 2)
        {
          manette2.transform.SetParent(verticalLayoutLeft.transform);
          p2placement = 0;
        }
        manette1.GetComponent<UIButton>().ExecuteClick();
        manette1.GetComponent<Button>().interactable = false;
      }
      p1HasChosen = true;
    }


    else if (i == 1 && p2placement != 1 && !p2HasChosen)
    {
      if (p2placement == 0)
      {

        if (p1joined && p1placement == 0)
        {
          manette1.transform.SetParent(verticalLayoutRight.transform);
          p1placement = 2;
        }
        manette2.GetComponent<UIButton>().ExecuteClick();
        manette2.GetComponent<Button>().interactable = false;

      }
      else if (p2placement == 2)
      {

        if (p1joined && p1placement == 2)
        {
          manette1.transform.SetParent(verticalLayoutLeft.transform);
          p1placement = 0;
        }
        manette2.GetComponent<UIButton>().ExecuteClick();
        manette2.GetComponent<Button>().interactable = false;

      }
      p2HasChosen = true;
    }



    if(p1HasChosen && p2HasChosen)//c'est ici qu'on lancera la scene
    {
      Debug.Log("COUCOU VICTOR LES DEUX JOUEURS ONT CHOISI LEURS PERSOS");
    }
  }

  void Update()
  {

    

    for (int i = 0; i < ReInput.players.playerCount; i++)
    {
      if (enableJoinGame)
      {
        if (ReInput.players.GetPlayer(i).GetButtonDown("JoinGame") && numberPlayersReady < 2)
        {
          AssignNextPlayer(i);
          if (i == 0)
          {
            verticalLayoutCenter.transform.GetChild(0).gameObject.SetActive(true);
            p1joined = true;
            //buttonPlayer1.GetComponent<UIButton>().ExecuteClick();
            numberPlayersReady++;
            //listPlayerID.Add(i);
          }

          else if (i == 1)
          {
            verticalLayoutCenter.transform.GetChild(1).gameObject.SetActive(true);
            p2joined = true;
            numberPlayersReady++;
            //listPlayerID.Add(i);
          }
        }

        else if (ReInput.players.GetPlayer(i).GetAxis("UIHorizontal") < 0)
        {

          MoveControllerLeft(i);
        }

        else if (ReInput.players.GetPlayer(i).GetAxis("UIHorizontal") > 0)
        {
          MoveControllerRight(i);
        }

        else if (ReInput.players.GetPlayer(i).GetButtonDown("UISubmit"))
        {
          ChooseCharacter(i);
        }

        else if (ReInput.players.GetPlayer(i).GetButtonDown("UICancel"))
        {
          if(i == 0 && p1joined && p1HasChosen)
          {
            manette1.GetComponent<Button>().interactable = true;
            p1HasChosen = false;
          }
          else if (i == 1 && p2joined && p2HasChosen)
          {
            manette2.GetComponent<Button>().interactable = true;
            p2HasChosen = false;
          }
        }
      }

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
