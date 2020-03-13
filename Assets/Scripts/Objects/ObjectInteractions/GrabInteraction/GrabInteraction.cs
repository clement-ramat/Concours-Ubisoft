using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrabInteraction : IHumanInteraction
{
    private bool shouldFollow = false;
    public GameObject grabber;
  
    private void Update()
    {
        if (shouldFollow && grabber != null)
        {
            transform.position = grabber.transform.position;
            transform.rotation = grabber.transform.rotation;
        }        
    }

    public void UnGrab()
    {
        shouldFollow = false;

        //If object has rigibody, set as non-kinematic
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
        }

        SetColliderEnabled(true);
        grabber = null;
    }

    public void UnGrab(Vector3 dropPosition)
    {
        shouldFollow = false;

        transform.DOJump(dropPosition, 2f, 1, 0.2f).OnComplete(() =>
        {
            UnGrab();
        });
        
    }

    public void Grab(GameObject go)
    {
        //Debug.Log("GrabInteraction");
        grabber = go;

        transform.DOJump(go.transform.position, 2f, 1, 0.2f)
            .Join(
            transform.DORotate(go.transform.rotation.eulerAngles, 0.2f)
            ).OnComplete(() =>
            {
                //If object has rigibody, set as kinematic
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.isKinematic = true;
                }

                SetColliderEnabled(false);
                shouldFollow = true;
            });
        
    }

    public void SetColliderEnabled(bool enabled = true)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(includeInactive: true);
        foreach (Collider collider in colliders)
        {
            collider.enabled = enabled;
        }
    }

}
