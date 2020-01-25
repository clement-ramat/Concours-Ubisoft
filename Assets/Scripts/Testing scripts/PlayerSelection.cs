using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSelection : NetworkBehaviour
{

    public GameObject ghostPrefab;
    public GameObject humanPrefab;

    public Vector3 spawnPoint;

    public bool spawned = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawned && Input.GetKeyDown("space"))
        {
            spawned = true;
            CmdSpawnGhost();
        }

        if(!spawned && Input.GetKeyDown("enter"))
        {
            spawned = true;
            CmdSpawnHuman();
        }
    }

    [Command]
    public void CmdSpawnGhost()
    {
        NetworkConnection conn = connectionToClient;

        // Cache a reference to the current player object
        GameObject oldPlayer = conn.identity.gameObject;

        Debug.Log(oldPlayer);

        // Instantiate the new player object and broadcast to clients
        NetworkServer.ReplacePlayerForConnection(conn, Instantiate(ghostPrefab, transform.position, Quaternion.identity));

        // Remove the previous player object that's now been replaced
        NetworkServer.Destroy(oldPlayer);
    }

    [Command]
    public void CmdSpawnHuman()
    {
        NetworkConnection conn = connectionToClient;

        // Cache a reference to the current player object
        GameObject oldPlayer = conn.identity.gameObject;

        Debug.Log(oldPlayer);

        // Instantiate the new player object and broadcast to clients
        NetworkServer.ReplacePlayerForConnection(conn, Instantiate(humanPrefab, transform.position, Quaternion.identity));

        // Remove the previous player object that's now been replaced
        NetworkServer.Destroy(oldPlayer);

    }
}
