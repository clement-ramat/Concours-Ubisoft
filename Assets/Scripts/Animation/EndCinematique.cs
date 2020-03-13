using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class EndCinematique : MonoBehaviour
{

    //Animated Things
    [SerializeField]
    private Animator StephAnimator;
    [SerializeField]
    private Animator ChestAnimator;
    [SerializeField]
    private CinemachineVirtualCamera DollyCamera;

    //Variables
    [SerializeField]
    private float openingSpeed = 0.1f;
    [SerializeField]
    private float trackDistance = 1.0f;


    //Timecodes
    [SerializeField]
    private float stephTriggerToOpenChest;
    [SerializeField]
    private float endCameraMovement;

    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("startEndAnimation");
            StartCoroutine(Animation());
        }
    }

    IEnumerator Animation()
    {
        StephAnimator.SetTrigger("openChestTrigger");

        //On fait bouger la caméra grâce à DOTween
        DOTween.To(() => DollyCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition, x => DollyCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = x, trackDistance, endCameraMovement);

        //
        yield return new WaitForSeconds(stephTriggerToOpenChest);

        ChestAnimator.SetFloat("OpeningMultiplier", openingSpeed);
        ChestAnimator.SetBool("Open", true);

        yield return new WaitForSeconds(0f);
    }
}
