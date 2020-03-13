using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Doozy.Engine.UI;
using UnityEngine.EventSystems;
using Michsky.UI.Dark;
using Mirror;

public class PauseMenuManager : MonoBehaviour
{

  private bool panelDown = false;
  public UIView pauseView;
  public GameObject buttonContinuePlay;
  public GameObject menuManager;

  private NetworkManager networkManager;

  // Start is called before the first frame update
  void Start()
  {
    networkManager = NetworkManager.singleton as NetworkManager;
  }

  // Update is called once per frame
  void Update()
  {
    if ((ReInput.players.GetPlayer(0).GetButtonDown("PauseGame")) && !panelDown)
    {
      panelDown = true;
      pauseView.Show();
      EventSystem.current.SetSelectedGameObject(buttonContinuePlay);
      menuManager.GetComponent<SplashScreenManager>().Restart();
      ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(false, "Default");
      ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(false, "Ghost");
      ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(false, "Human");

    }
    else if ((ReInput.players.GetPlayer(0).GetButtonDown("PauseGame")) && panelDown)
    {
      HideView();
    }
  }

  public void HideView()
  {
    panelDown = false;
    pauseView.Hide();
    EventSystem.current.SetSelectedGameObject(null);
    ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(true, "Default");
    ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(true, "Ghost");
    ReInput.players.GetPlayer(0).controllers.maps.SetMapsEnabled(true, "Human");
  }


  public void ReturnToMenu()
  {
    if (networkManager != null)
    {
      networkManager.StopHost();
    }
  }
}
