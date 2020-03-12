using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorteeButton : MonoBehaviour
{
    public Portee portee;

    private bool active = false;

    public Material activeMat;
    private Material baseMat;
    private Renderer meshRenderer;


    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        baseMat = meshRenderer.material;
    }


    public void EnableButton(bool b)
    {
        active = b;

        if (b)
        {
            meshRenderer.material = activeMat;
        }
        else
        {
            meshRenderer.material = baseMat;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active)
        {
            if (other.GetComponentInParent<Human>())
            {
                EnableButton(true);
                StartCoroutine(portee.playAnimation());
            }
        }
    }
}
