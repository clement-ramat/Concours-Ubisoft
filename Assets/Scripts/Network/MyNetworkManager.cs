using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public enum CharacterType
{
    Ghost,
    Human
}

public class MyNetworkManager : NetworkManager
{

    //[SerializeField]
    //public CharacterType characterToSpawn;

    public GameObject playerControllerPrefab;
    public GameObject ghostPrefab;
    public GameObject humanPrefab;

    /*public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

    }*/

    public void SpawnCharacterForPlayer(CharacterType characterType, NetworkConnection conn)
    {
        GameObject oldObject = conn.identity.gameObject;

        GameObject playerController = Instantiate(playerControllerPrefab, GetStartPosition().position, Quaternion.identity);

        NetworkServer.ReplacePlayerForConnection(conn, playerController);

        NetworkServer.Destroy(oldObject);

        Vector3 playerPosition = GetPlayerSpawnPoint();
        if (characterType == CharacterType.Ghost)
        {
            playerController.GetComponent<PlayerController>().TargetSetCharacterType(conn, CharacterType.Ghost);

            GameObject ghost = Instantiate(ghostPrefab, playerPosition, Quaternion.identity);

            NetworkServer.Spawn(ghost);

            playerController.GetComponent<PlayerController>().myCharacter = ghost;
            playerController.GetComponent<PlayerController>().characterType = CharacterType.Ghost;

        }
        else
        {

            playerController.GetComponent<PlayerController>().TargetSetCharacterType(conn, CharacterType.Human);

            GameObject human = Instantiate(humanPrefab, playerPosition, Quaternion.identity);

            NetworkServer.Spawn(human);

            playerController.GetComponent<PlayerController>().myCharacter = human;
            playerController.GetComponent<PlayerController>().characterType = CharacterType.Human;
        }

    }

    private Vector3 GetPlayerSpawnPoint()
    {
        RoomsManager roomsManager = FindObjectOfType<RoomsManager>();
        if (roomsManager != null)
        {
            List<Transform> transforms = roomsManager.GetCurrentRoom().GetSpawnPoints();
            return transforms[Random.Range(0, transforms.Count)].position;
        }

        LobbyBehavior lobbyManager = FindObjectOfType<LobbyBehavior>();
        if (lobbyManager != null)
        {
            List<Transform> transforms = lobbyManager.GetSpawnsTranforms();
            return transforms[Random.Range(0, transforms.Count)].position;
        }

        return Vector3.zero;
    }

}
