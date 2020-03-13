using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Doozy.Engine.UI;
using TMPro;
using Michsky.UI.Dark;

/// <summary>
/// Gere le changement de view du menu principal
/// </summary>
public class NetworkMenuHandler : MonoBehaviour
{
  public TMP_InputField ipadress;

  public UIView HostJoinView;
  public UIView WaitView;
  public UIView CharSelectView;
  private int activeView = 0; //0 : Host, 1 : Wait, 2 : CharSelect

  public SelectionMenuHandler selectionMenuHandler;

  public GameObject menuManager;

  public void Join()
  {
    NetworkManager networkManager = NetworkManager.singleton as NetworkManager;

    networkManager.networkAddress = ipadress.text;
    networkManager.StartClient();
  }

  public void Host()
  {
    NetworkManager networkManager = NetworkManager.singleton as NetworkManager;
    networkManager.StartHost();
  }

  public void Cancel()
  {
    NetworkManager networkManager = NetworkManager.singleton as NetworkManager;
    networkManager.StopHost();
  }
}
