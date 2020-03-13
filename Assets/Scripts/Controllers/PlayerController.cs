using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Rewired;
using ThirteenPixels.Soda;

public class PlayerController : NetworkBehaviour
{
    protected Rewired.Player playerRewired;

    [SyncVar]
    public GameObject myCharacter;

    private int rewiredID;

    [SyncVar]
    public CharacterType characterType;

    public int inputSendrate;

    private IEnumerator sendCoroutine;
    private Vector3 moveInput;

    [SerializeField]
    private ScopedInt characterTypeGlobalClient;

    protected virtual void Start()
    {
        playerRewired = ReInput.players.GetPlayer(rewiredID);
        myCharacter.GetComponent<Character>().OnVibration.AddListener(VibratePlayer);
    }
    
    public override void OnStartLocalPlayer()
    {
        if (sendCoroutine == null)
            sendCoroutine = SendCommands();

        StartCoroutine(sendCoroutine);
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
            
        moveInput = new Vector3(playerRewired.GetAxis("MoveX"), playerRewired.GetAxis("MoveY"), 0);

        if(characterType == CharacterType.Ghost)
        {
            if (playerRewired.GetButtonDown("Possess"))
            {
                CmdPossess();
            }

            if (playerRewired.GetButtonDown("LaunchPossessedObject"))
            {
                CmdLaunchPossessedObject();
            }

            if (playerRewired.GetButtonDown("RevealBlast"))
            {
                CmdRevealBlast();
            }
        }
        else if(characterType == CharacterType.Human)
        {
            if (playerRewired.GetButtonDown("Interact"))
            {
                CmdInteract();
            }

            if (playerRewired.GetButtonDown("Flash"))
            {
                CmdFlash();
            }

            if (playerRewired.GetButtonDown("Throw"))
            {
                CmdThrow();
            }

            if (playerRewired.GetButtonDown("Wave"))
            {
                CmdHumanWave();
            }
        }
    }

    

    public void OnDestroy()
    {
        if(isServer)
        {
            NetworkServer.Destroy(myCharacter);
            //NetworkServer.UnSpawn(myCharacter);
        }
        //myCharacter.GetComponent<Character>().OnVibration.RemoveListener(VibratePlayer);
        //CmdDestroyCharacter();
    }

    [Command]
    public void CmdDestroyCharacter()
    {
        NetworkServer.UnSpawn(myCharacter);
    }

    IEnumerator SendCommands()
    {
        while(true)
        {
            CmdMoveInput(moveInput);

            yield return new WaitForSeconds(1 / inputSendrate);
        }
    }


    [Command]
    public void CmdMoveInput(Vector3 moveInput)
    {
        myCharacter.GetComponent<Character>().MoveInput = moveInput;
    }

    [Command]
    public void CmdPossess()
    {
        myCharacter.GetComponent<GhostPlayer>().PossessionAction();
    }

    [Command]
    public void CmdLaunchPossessedObject()
    {
        myCharacter.GetComponent<Ghost>().LaunchPossessedObject();
    }

    [Command]
    public void CmdInteract()
    {
        myCharacter.GetComponent<Human>().Interact();
    }

    [Command]
    public void CmdThrow()
    {
        myCharacter.GetComponent<Human>().Throw();
    }

    [Command]
    public void CmdFlash()
    {
        myCharacter.GetComponent<Human>().FlashAttack();
    }

    [Command]
    public void CmdRevealBlast()
    {
        myCharacter.GetComponent<GhostPlayer>().RevealBlast();
    }

    [Command]
    public void CmdHumanWave()
    {
        myCharacter.GetComponent<Human>().Wave();
    }

    [TargetRpc]
    public void TargetSetCharacterType(NetworkConnection target, CharacterType characterType)
    {
        characterTypeGlobalClient.value = (int)characterType;
    }

    public void VibratePlayer(float vibrationPower, float vibrationDuration)
    {
        playerRewired.SetVibration(0, vibrationPower, vibrationDuration);
    }
}
