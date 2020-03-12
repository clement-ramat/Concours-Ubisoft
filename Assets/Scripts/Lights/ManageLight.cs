using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageLight : MonoBehaviour
{

    [SerializeField]
    private float animationDuration = 1.25f;

    private Light myLight;
    private float initialIntensity;

    // Start is called before the first frame update
    void Start()
    {
        myLight = GetComponent<Light>();
        if(myLight != null)
        {
            initialIntensity = myLight.intensity;
        }
    }

    public void Manage(bool value)
    {
        float intensityTarget = value ? initialIntensity : 0;
        if (myLight != null)
        {
            myLight.DOIntensity(intensityTarget, animationDuration).SetEase(Ease.OutSine);
        }
    }
}
