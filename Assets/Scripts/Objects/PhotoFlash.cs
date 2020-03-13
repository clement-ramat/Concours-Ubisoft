using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;

public class PhotoFlash : MonoBehaviour
{
    [SerializeField]
    private float minIntensity = 0;

    [SerializeField]
    private float maxIntensity = 1000;

    [SerializeField]
    private float flashDuration = 0.3f;

    [SerializeField]
    private GameObject characterToFlee = null;

    private int opacityPropertyId;

    private Light spotLight;

    private Transform parentSave;

    private Vector3 savedPosition;

    private Quaternion savedRotation;

    [SerializeField][ReadOnly]
    private List<Collider> colliders;

    [HideInInspector]
    public bool pastUpgrade = false;

    // Start is called before the first frame update
    void Awake()
    {
        spotLight = GetComponent<Light>();
        spotLight.intensity = 0;

        // Disabling the flash colliders
        colliders = new List<Collider>(GetComponentsInChildren<Collider>());
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        // Saved light parent, local position and rotation
        parentSave = transform.parent;
        savedPosition = transform.localPosition;
        savedRotation = transform.localRotation;

    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void FlashAttack()
    {
        // Enabling the flash colliders (may call OnTriggerEnter)
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }

        
    } 

    public void FlashAnimation()
    {
        

        // Detach from parent so that flash stay still
        transform.SetParent(null);

        // Make the flash 'animation'
        spotLight.intensity = maxIntensity;
        spotLight.DOIntensity(minIntensity, flashDuration).SetEase(Ease.InQuad).OnComplete(OnFlashEnd);
    }

    public void OnFlashEnd()
    {
        // Set back to parent after flashing
        transform.SetParent(parentSave);
        transform.localPosition = savedPosition;
        transform.localRotation = savedRotation;

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        GhostEnemy enemy = other.GetComponentInParent<GhostEnemy>();

        // Check if other collider is an ennemy ghost
        if (enemy)
        {
            enemy.Flash();
        }

        if (pastUpgrade)
        {
            PastObjectBehavior pastObjectBehavior = other.GetComponentInParent<PastObjectBehavior>();
            if (pastObjectBehavior != null)
            {
                pastObjectBehavior.OnFlashed();
            }
        }

    }
}
