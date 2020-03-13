using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterObjectSelection : NetworkBehaviour
{

    // Update is called once per frame
    void Update()
    {
 
        if(Input.GetKeyDown("g"))
        {
            Debug.Log(netIdentity);
            CmdSpawnGhost();
        }
        else if (Input.GetKeyDown("h"))
        {
            Debug.Log(netIdentity);
            CmdSpawnHuman();
        }

    }

    [Command]
    public void CmdSpawnGhost()
    {
        Debug.Log(connectionToClient.identity);
        ((MyNetworkManager)NetworkManager.singleton).SpawnCharacterForPlayer(CharacterType.Ghost, connectionToClient);
    }

    [Command]
    public void CmdSpawnHuman()
    {
        Debug.Log(connectionToClient.identity);
        ((MyNetworkManager)NetworkManager.singleton).SpawnCharacterForPlayer(CharacterType.Human, connectionToClient);
    }
}
