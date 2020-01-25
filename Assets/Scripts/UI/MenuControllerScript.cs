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
  public GameObject buttonPressXToStart;
  public GameObject textPressStartToJoin;
  private int numberPlayersReady = 0;
  private bool enableJoinGame = true;
  private List<int> listPlayerID = new List<int>();
  private static MenuControllerScript instance;

  private bool p1joined = false;
  private bool p2joined = false;
  private int p1placement = 1; //0 for left, 1 for center, 2 for right
  private int p2placement = 1;

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

  // Instance

  public int maxPlayers = 4;

  private List<PlayerMap> playerMap; // Maps Rewired Player ids to game player ids
  private int gamePlayerIdCounter = 0;

  void Awake()
  {
    playerMap = new List<PlayerMap>();
    instance = this; // set up the singleton
  }


  void MoveControllerLeft(int i)
  {
    if (i == 0 && p1joined)
    {
      if (p1placement != 0)
      {
        if (p1placement == 1)
          verticalLayoutCenter.transform.GetChild(0).gameObject.SetActive(false);

        else if (p1placement == 2)
          verticalLayoutRight.transform.GetChild(0).gameObject.SetActive(false);

        verticalLayoutLeft.transform.GetChild(0).gameObject.SetActive(true);
        p1placement = 0;

      }
    }

    if (i == 1 && p2joined)
    {
      if (p2placement != 0)
      {

        if (p2placement == 1)
          verticalLayoutCenter.transform.GetChild(1).gameObject.SetActive(false);

        else if (p2placement == 2)
          verticalLayoutRight.transform.GetChild(1).gameObject.SetActive(false);

        verticalLayoutLeft.transform.GetChild(1).gameObject.SetActive(true);
        p2placement = 0;
      }
    }
  }

  void MoveControllerRight(int i)
  {
    if (i == 0 && p1joined)
    {
      if (p1placement != 2)
      {
        if (p1placement == 1)
          verticalLayoutCenter.transform.GetChild(0).gameObject.SetActive(false);

        else if (p1placement == 0)
          verticalLayoutLeft.transform.GetChild(0).gameObject.SetActive(false);

        verticalLayoutRight.transform.GetChild(0).gameObject.SetActive(true);
        p1placement = 2;

      }
    }

    if (i == 1 && p2joined)
    {
      if (p2placement != 2)
      {
        if (p2placement == 1)
          verticalLayoutCenter.transform.GetChild(1).gameObject.SetActive(false);

        else if (p2placement == 0)
          verticalLayoutLeft.transform.GetChild(1).gameObject.SetActive(false);


        verticalLayoutRight.transform.GetChild(1).gameObject.SetActive(true);
        p2placement = 2;

      }
    }
  }

  void Update()
  {

    /*if (Input.GetButtonDown("Cancel"))
    {
      GameObject buttonPlay = GameObject.Find("Button - Play");
      EventSystem.current.SetSelectedGameObject(buttonPlay);
      Debug.Log(EventSystem.current);
      enableJoinGame = false;
    }*/

    for (int i = 0; i < ReInput.players.playerCount; i++)
    {
      if (enableJoinGame)
      {
        if (ReInput.players.GetPlayer(i).GetButtonDown("JoinGame") && numberPlayersReady <= 2)
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
          if(i == 0 && p1placement != 1)
          {
            if(p1placement == 0)
            {
              if (p2joined)
              {
                verticalLayoutLeft.transform.GetChild(1).gameObject.SetActive(false);
                verticalLayoutRight.transform.GetChild(1).gameObject.SetActive(true);
                p2placement = 2;
              }
              verticalLayoutLeft.transform.GetChild(0).GetComponent<Button>().Select();
              verticalLayoutLeft.transform.GetChild(0).GetComponent<UIButton>().ExecuteClick();

            }
            else if (p1placement == 2)
            {
              if (p2joined)
              {
                verticalLayoutRight.transform.GetChild(1).gameObject.SetActive(false);
                verticalLayoutLeft.transform.GetChild(1).gameObject.SetActive(true);
                p2placement = 0;
              }
              verticalLayoutRight.transform.GetChild(0).GetComponent<Button>().Select();
              verticalLayoutRight.transform.GetChild(0).GetComponent<UIButton>().ExecuteClick();

            }
          }
          if (i == 1 && p2placement != 1)
          {
            if (p2placement == 0)
            {

              if (p1joined)
              {
                verticalLayoutLeft.transform.GetChild(0).gameObject.SetActive(false);
                verticalLayoutRight.transform.GetChild(0).gameObject.SetActive(true);
                p1placement = 2;
              }
              verticalLayoutLeft.transform.GetChild(1).GetComponent<Button>().Select();
              verticalLayoutLeft.transform.GetChild(1).GetComponent<UIButton>().ExecuteClick();

            }
            else if (p2placement == 2)
            {

              if (p1joined)
              {
                verticalLayoutRight.transform.GetChild(0).gameObject.SetActive(false);
                verticalLayoutLeft.transform.GetChild(0).gameObject.SetActive(true);
                p1placement = 0;
              }
              verticalLayoutRight.transform.GetChild(1).GetComponent<Button>().Select();
              verticalLayoutRight.transform.GetChild(1).GetComponent<UIButton>().ExecuteClick();

            }
          }
        }
      }

    }

    /*

    for (int i = 0; i < ReInput.players.playerCount; i++)
    {
      if ((ReInput.players.GetPlayer(i).GetButtonDown("Fire")) && (numberPlayersReady >= 1))
      {
        Debug.Log("Start game");
        if (numberPlayersReady == 1)
          configGame.InitConfigGame(1, 0, new List<int> { 0 }, null);
        else if (numberPlayersReady == 2)
          configGame.InitConfigGame(1, 1, new List<int> { 0 }, new List<int> { 1 });
        else if (numberPlayersReady == 3)
          configGame.InitConfigGame(2, 1, new List<int> { 0, 2 }, new List<int> { 1 });
        else if (numberPlayersReady == 4)
          configGame.InitConfigGame(2, 2, new List<int> { 0, 2 }, new List<int> { 1, 3 });


        SceneManager.LoadScene("MainGameScene");
        MusicEmitter.TransitionToBattleMusic();
      }
    }*/


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
