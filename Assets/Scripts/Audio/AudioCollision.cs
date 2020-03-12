using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioCollision : MonoBehaviour
{
    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    public LayerMask trigererLayerMask;


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Audio Collision : On Collision Enter");

        if ((collision.gameObject.layer & trigererLayerMask) != 0)
        {
            OnEnter.Invoke();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Audio Collision : On Collision Exit");

        if ((collision.gameObject.layer & trigererLayerMask) != 0)
        {
            OnExit.Invoke();
        }
    }
}
