using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyDoorBehavior : MonoBehaviour
{

    [SerializeField]
    private Transform doorTransform;

    public List<Transform> spawnTransforms = new List<Transform>();

    public void Start()
    {
        doorTransform.gameObject.SetActive(true);
    }

    public void DisableLobbyDoor()
    {
        doorTransform.gameObject.SetActive(false);
    }

    public void OnOpenDoor()
    {
        LobbyBehavior lobbyBehavior = FindObjectOfType<LobbyBehavior>();
        if (lobbyBehavior != null)
        {
            lobbyBehavior.LoadNextScene();
        }
    }
}
