using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirteenPixels.Soda;
using DG.Tweening;
using Cinemachine;

public class GhostFleeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject KeyGhostPrefab;

    private GameObject KeyGhost;

    [SerializeField]
    private GlobalBool ShouldPlayAnimation;

    [SerializeField]
    private Transform startPoint;

    [SerializeField]
    private Transform endPoint;

    private bool hasPlayed = false;

    [SerializeField]
    private bool playerCanMoveDuringAnimation = false;

    [SerializeField]
    private GlobalPlayerGhost globalPlayerGhost;

    [SerializeField]
    private GlobalPlayerHuman globalPlayerHuman;

    [SerializeField]
    private bool cameraMovement = true;

    [SerializeField]
    private RoomBehavior roomBehavior;

    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    private CinemachineVirtualCamera previousCamera;
    private void Start()
    {
        ShouldPlayAnimation.onChange.AddResponse(PlayAnimation);
    }

    private void OnEnable()
    {
        if(ShouldPlayAnimation.value && !hasPlayed)
        {
            StartCoroutine(Animation());
        }
    }

   /* 
    private void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            PlayAnimation(true);
        }
    }*/

    private void PlayAnimation(bool newValue)
    {
        StartCoroutine(Animation());
    }

    IEnumerator Animation()
    {
        if(!playerCanMoveDuringAnimation)
        {
            if (globalPlayerHuman.value != null)
            {
                globalPlayerHuman.componentCache.ResetCharacter();
                globalPlayerHuman.componentCache.CanMove(false);
            }

            if (globalPlayerGhost.value != null)
            {
                globalPlayerGhost.componentCache.ResetCharacter();
                globalPlayerGhost.componentCache.CanMove(false);
            }
        }

        hasPlayed = true;

        if (cameraMovement)
        {
            //save previous camera
            previousCamera = roomBehavior.GetCurrentCamera();
            //Move camera
            roomBehavior.ActivateVirtualCamera(virtualCamera);
        }

        //wait for camera to be there
        yield return new WaitForSeconds(0.5f);

        //spawn "invisible" ghost
        SpawnKeyGhost();

        //play spawn ghost particle once

        foreach (ParticleSystem particle in KeyGhost.GetComponentsInChildren<ParticleSystem>())
        {
            Debug.Log(particle);
            particle.Play();
        }

        //wait end for showing mesh

        //show ghost mesh
        foreach(Renderer renderer in KeyGhost.GetComponentsInChildren<Renderer>())
        {
            Debug.Log(renderer);
            renderer.enabled = true;
        }

        yield return new WaitForSeconds(1.5f);


        //look at end point
        KeyGhost.transform.LookAt(endPoint);

        //Move to end point
        KeyGhost.GetComponent<Animator>().SetBool("moving", true);

        //End of animation
        KeyGhost.transform.DOMove(endPoint.position, 2f).OnComplete(() =>
        {
            Destroy(KeyGhost);

            //reset camera
            if(cameraMovement)
            {
                roomBehavior.ActivateVirtualCamera(previousCamera);
            }

            if (!playerCanMoveDuringAnimation)
            {
                // --- TODO : Make it via an game cut-scene 
                if (globalPlayerHuman.value != null)
                {
                    globalPlayerHuman.componentCache.CanMove(true);
                }

                if (globalPlayerGhost.value != null)
                {
                    globalPlayerGhost.componentCache.CanMove(true);
                }
                // -------------------------------------
            }
        });
    }


    public GameObject SpawnKeyGhost()
    {
        return KeyGhost = Instantiate(KeyGhostPrefab, startPoint.position, startPoint.rotation);
    }
}
