using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class AudioTrigger : MonoBehaviour
{
    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    public LayerMask trigererLayerMask;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1<<other.gameObject.layer) & trigererLayerMask) != 0)
        {
            OnEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & trigererLayerMask) != 0)
        {
            OnExit.Invoke();
        }
    }
}
