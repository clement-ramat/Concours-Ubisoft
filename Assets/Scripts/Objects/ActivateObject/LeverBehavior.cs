using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for a lever object.
/// We can simply activate the lever (make it go down).
/// We can also configure if the lever deactivate itself automatically after a given time.
/// </summary>
public class LeverBehavior : ActivateObject
{
    [Header("Lever Settings")]
    [SerializeField]
    private bool deactivateItSelf = false;

    [SerializeField]
    private float deactivateItSelfDelay = 0.2f;

    private Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public override void Activate()
    {
        if(Activated && deactivateItSelf)
        {
            return;
        }

        Activated = !Activated;

        if (deactivateItSelf)
        {
            StartCoroutine(GoUpAutomaticDelayCoRoutine());
        }
    }

    public void ActivateDirection(Vector3 direction)
    {
        if (direction == Vector3.up && Activated)
        {
            Activate();
        }else if (direction == Vector3.down && !Activated)
        {
            Activate();
        }
    }

    IEnumerator GoUpAutomaticDelayCoRoutine()
    {
        yield return new WaitForSeconds(deactivateItSelfDelay);

        Activated = false;
    }

    private void Update()
    {
        if (animator != null)
        {
            animator.SetBool("Down", Activated);
        }
    }
}
