using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; //DOTWEEN!!!
using Sirenix.OdinInspector;
using Mirror;

public class Shaker : NetworkBehaviour
{
    [Title("Shaking Parameters")]
    [SerializeField]
    private float durationShake = 0.1f;

    [SerializeField]
    private float distanceShake = 0.3f;


    [Title("Pushed Objects Parameters")]
    [SerializeField] [ReadOnly]
    [Tooltip("List of gameobject that will also move with the shaking object")]
    public List<Rigidbody> affectedByShaking = new List<Rigidbody>();

    //[SerializeField]
    //private Vector3 pushDirection = Vector3.zero;

    [SerializeField]
    private float pushForceMultiplier = 5f;

    //To know if there's already a shaking going on
    public Coroutine currentShakingCoroutine = null;

    private bool isShaking;
    private Vector3 initialPosition;

    public Vector3 InitialPosition
    {
        get
        {
            if (isShaking)
            {
                return initialPosition;
            }

            return transform.position;
        }
    }

    private void Start()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        //Ajoute un Shaker à tout les colliders enfants du Shaker
        foreach (Collider coll in colliders)
        {
            if (coll && !coll.isTrigger && !coll.GetComponent<ShakerAddInPushedObjects>())
            {
                coll.gameObject.AddComponent<ShakerAddInPushedObjects>();
            }
        }

        initialPosition = transform.position;
    }

    public void Shake(Vector3 direction)
    {
        if (isServer)
        {
            Shake(direction, durationShake);
        }
    }

    private void Shake(Vector3 direction, float duration)
    {
        //Shake, but only if there's no other shake already happening

        if (currentShakingCoroutine != null)
        {
            return;
        }

        //Vérifie qu'on secoue sur le bon plan et corrige si nécessaire.
        if (direction.y != 0) {
            direction.z = direction.y;
            direction.y = 0;
        }

        //start shaking
        currentShakingCoroutine = StartCoroutine(_Shaking(direction.normalized, duration));
    }

    //Only called within the coroutine, push the linked rigidbody toward the direction of the shake
    private void PushAffectedObjectsToward(Vector3 direction, float duration)
    {
        foreach (Rigidbody rb in affectedByShaking)
        {
            rb.AddForce(direction * pushForceMultiplier, ForceMode.Impulse);
        }
    }

    private IEnumerator _Shaking(Vector3 direction, float duration)
    {
        isShaking = true;
        direction = direction.normalized;

        //remember start pos
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + direction * distanceShake;

        float durationFirstHalf = duration / 2f;
        float durationSecondHalf = duration / 2f;

        //Move toward direction
        transform.DOMove(targetPosition, durationFirstHalf);

        //PushAffectedObjectsToward(transform.forward, durationFirstHalf);
        PushAffectedObjectsToward(direction.normalized, durationFirstHalf);

        yield return new WaitForSeconds(durationFirstHalf);

        //Move back to place
        transform.DOMove(startPosition, durationSecondHalf);
        yield return new WaitForSeconds(durationSecondHalf);

        //Allow for new shake
        currentShakingCoroutine = null;
        isShaking = false;
    }
    
}
