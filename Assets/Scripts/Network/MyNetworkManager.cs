using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Sirenix.OdinInspector;

public class MyNetworkManager : NetworkManager
{
    [SceneObjectsOnly]
    public GameObject CharacterSelectionPanel;

    public void ReplacePlayer(GameObject newGameObject)
    {
        NetworkConnection conn = NetworkClient.connection;

        // Cache a reference to the current player object
        GameObject oldPlayer = conn.identity.gameObject;

        // Instantiate the new player object and broadcast to clients
        NetworkServer.ReplacePlayerForConnection(conn, Instantiate(newGameObject, oldPlayer.transform.position, Quaternion.identity));

        // Remove the previous player object that's now been replaced
        NetworkServer.Destroy(oldPlayer);
    }
}
