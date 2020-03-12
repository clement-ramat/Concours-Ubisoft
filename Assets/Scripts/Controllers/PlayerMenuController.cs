using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Rewired;
using Michsky.UI.Dark;
using UnityEngine.EventSystems;

public class PlayerMenuController : NetworkBehaviour
{

  private bool characterSelectionPanelDown = false;
  private bool playerHasChosenCharacter = false;

  public SelectionMenuHandler selectionMenuHandler;

  [SyncVar]
  public int playerId;

  // Update is called once per frame
  void Update()
  {
    if (!isLocalPlayer)
      return;

    if (selectionMenuHandler != null)
    {
      if (ReInput.players.GetPlayer(0).GetAxis("UIHorizontal") < 0)
      {
        CmdMoveLeft();
      }
      else if (ReInput.players.GetPlayer(0).GetAxis("UIHorizontal") > 0)
      {
        CmdMoveRight();
      }
      else if (ReInput.players.GetPlayer(0).GetButtonDown("UISubmit"))
      {
        CmdSubmit();
        playerHasChosenCharacter = true;
      }
      else if (ReInput.players.GetPlayer(0).GetButtonDown("UICancel"))
      {
        if (playerHasChosenCharacter)
        {
          CmdCancel();
          playerHasChosenCharacter = false;
        }
        else
        {
          NetworkMenuHandler networkMenuHandler = FindObjectOfType<NetworkMenuHandler>();

          networkMenuHandler.Cancel();
          networkMenuHandler.menuManager.GetComponent<MainPanelManager>().PanelAnim(0);
          EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);

        }
      }
      else if (ReInput.players.GetPlayer(0).GetButtonDown("StartGame"))
      {
        CmdStartGame();
      }
    }

  }

  [TargetRpc]
  public void TargetSetupController(NetworkConnection target, GameObject menuObj)
  {
    selectionMenuHandler = menuObj.GetComponent<SelectionMenuHandler>();
  }

  [Command]
  public void CmdMoveLeft()
  {
    selectionMenuHandler.MoveControllerLeft(playerId);
  }

  [Command]
  public void CmdMoveRight()
  {
    selectionMenuHandler.MoveControllerRight(playerId);
  }

  [Command]
  public void CmdSubmit()
  {
    selectionMenuHandler.ChooseCharacter(playerId);
  }


  [Command]
  public void CmdCancel()
  {
    selectionMenuHandler.CancelSelection(playerId);
  }


  [Command]
  public void CmdStartGame()
  {
    selectionMenuHandler.StartGame();
  }
}
