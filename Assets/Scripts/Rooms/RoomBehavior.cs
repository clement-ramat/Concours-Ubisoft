using Cinemachine;
using DG.Tweening;
using Mirror;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{

    [SerializeField]
    [Required]
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    [SerializeField]
    [Required]
    private Transform environmentTransform;

    public DoorBehavior finishDoor;

    [SerializeField]
    private Transform finishDoorTransform;

    /// <summary>
    /// Represent the points where players will spawn in the room.
    /// </summary>
    [SerializeField]
    private List<Transform> spawnTransforms;

    /// <summary>
    /// Represent the next points the players will go when opening the finish door of the room.
    /// </summary>
    [SerializeField]
    private List<Transform> nextRoomTransforms;

    [Title("Transparent Objects Settings")]
    [SerializeField]
    private bool hideFinishDoorOnRoomClosed = false;

    [SerializeField]
    private List<Renderer> wallsToHideFirst;

    [SerializeField]
    private bool disableOnClose = true;

    [SerializeField]
    private float rendererFadingAnimationDuration = 0.5f;

    private float transitionTime = 1.75f;

    [SerializeField]
    private Material opaqueMaterial;

    [SerializeField]
    private Material transparentMaterial;

    [Title("Others")]
    [SerializeField]
    private List<GameObject> aiToActivate;

    private CinemachineVirtualCamera currentCamera;
    public void ActivateRoom()
    {
        ActivateVirtualCamera(cinemachineVirtualCamera);

        ManageEnvironmentTransform(true);
        ManageLights(true);

        ManageTransparentRenderers(false);

        if(aiToActivate.Count > 0)
        {
            foreach(GameObject go in aiToActivate)
            {
                GhostEnemy ge = go.GetComponent<GhostEnemy>();
                if (ge) ge.RpcSetGhostActive(true);
            }
        }
    }

    public void CloseRoom(bool isServer = false)
    {
        cinemachineVirtualCamera.gameObject.SetActive(false);
        currentCamera = null;
        if (isServer)
        {
            if (finishDoor != null)
            { 
                finishDoor.SetActivateObjectsCanActivate(false);
            }

            Sequence mySequence = DOTween.Sequence();
            mySequence.AppendInterval(transitionTime).AppendCallback(() =>
            {
                if (finishDoor != null)
                {
                    finishDoor.Close();
                }
            });

            if (disableOnClose)
            {
                DisableRoom();
            }
        }

        // We directly hide those walls and the doors
        ManageTransparentRenderers(true);

        // We must disable the light now
        ManageLights(false);

        // After the door is closed, we simply hide all renderers
        Sequence mySequence2 = DOTween.Sequence();
        mySequence2.AppendInterval(transitionTime + 1.25f).AppendCallback(() =>
        {
            ManageEnvironmentTransform(false);
        });
    }

    /// <summary>
    /// Disable completely the room in a state where it cannot be used again.
    /// </summary>
    private void DisableRoom()
    {
        Sequence mySequence = DOTween.Sequence().AppendInterval(transitionTime + 0.75f).AppendCallback(() =>
        {
            NetworkIdentity[] networkIdentities = gameObject.GetComponentsInChildren<NetworkIdentity>();
            for (int i = networkIdentities.Length - 1; i >= 0; i--)
            {
                if (finishDoor != null && networkIdentities[i].gameObject != finishDoor.gameObject)
                {
                    NetworkServer.Destroy(networkIdentities[i].gameObject);
                }
            }
        });
        currentCamera = null;
    }


    private void ManageTransparentRenderers(bool value)
    {

        for (int i = 0; i < wallsToHideFirst.Count; i++)
        {
            Renderer renderer = wallsToHideFirst[i];

            Material materialToSet = value ? transparentMaterial : opaqueMaterial;

            if (materialToSet == null)
            {
                break;
            }

            renderer.material = materialToSet;

            if (value)
            {
                float targetAlpha = value ? 0.0f : 1.0f;

                Color materialColor = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b);
                DOTween.ToAlpha(() => materialColor, color => materialColor = color, targetAlpha, rendererFadingAnimationDuration).OnUpdate(() =>
                {
                    renderer.material.SetColor("_BaseColor", materialColor);
                }).OnComplete(() =>
                {
                    renderer.material.SetColor("_BaseColor", new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, targetAlpha));
                });
            }
        }

        Renderer[] renderers = finishDoor.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];

            Material materialToSet = value ? transparentMaterial : opaqueMaterial;

            if (materialToSet == null)
            {
                break;
            }

            renderer.material = materialToSet;

            if (value)
            {
                float targetAlpha = value ? 0.0f : 1.0f;

                Color materialColor = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b);
                DOTween.ToAlpha(() => materialColor, color => materialColor = color, targetAlpha, rendererFadingAnimationDuration).OnUpdate(() =>
                {
                    renderer.material.SetColor("_BaseColor", materialColor);
                }).OnComplete(() =>
                {
                    renderer.material.SetColor("_BaseColor", new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, targetAlpha));
                });
            }
        }


    }

    private void ManageEnvironmentTransform(bool value)
    {
        environmentTransform.gameObject.SetActive(value);
    }

    private void ManageLights(bool value)
    {
        ManageLight[] lights = GetComponentsInChildren<ManageLight>();
        for (int i = 0; i < lights.Length; i++)
        {

            lights[i].Manage(value);
        }
    }

    public void Reset()
    {
        currentCamera = null;
        CinemachineVirtualCamera[] cinemachineVirtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
        for (int i = 0; i < cinemachineVirtualCameras.Length; i++)
        {
            cinemachineVirtualCameras[i].gameObject.SetActive(false);
        }
        

        Light[] lights = GetComponentsInChildren<Light>();
        for (int i = 0; i < lights.Length; i++)
        {

            lights[i].gameObject.AddComponent<ManageLight>();
        }

        ManageEnvironmentTransform(false);
    }

    public List<Transform> GetSpawnPoints()
    {
        return spawnTransforms;
    }

    public List<Transform> GetNextRoomPoints()
    {
        return nextRoomTransforms;
    }

    public void ActivateVirtualCamera(CinemachineVirtualCamera _cinemachineVirtualCamera)
    {
        if (_cinemachineVirtualCamera != currentCamera)
        {
            CinemachineVirtualCamera[] cinemachineVirtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
            for (int i = cinemachineVirtualCameras.Length - 1; i >= 0; i--)
            {
                cinemachineVirtualCameras[i].gameObject.SetActive(false);
            }

            _cinemachineVirtualCamera.gameObject.SetActive(true);
            currentCamera = _cinemachineVirtualCamera;
        }
    }

    public CinemachineVirtualCamera GetCurrentCamera()
    {
        return currentCamera;
    }
}
