using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakerAddInPushedObjects : MonoBehaviour
{
    private Shaker shaker;

    private void Start()
    {
        shaker = GetComponent<Shaker>();
        if (!shaker)
        {
            shaker = GetComponentInParent<Shaker>();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(shaker == null)
        {
            return;
        }

        //can't edit list if shaking
        if (shaker.currentShakingCoroutine != null)
        {
            return;
        }

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        //If rigidbody wasn't affected by the shaking, it is now because it touches our object
        if (rb != null && !shaker.affectedByShaking.Contains(rb))
        {
            shaker.affectedByShaking.Add(rb);
        }

        // NOTE: j'ai retiré cette ligne pour que les objets posés sur les tables puissent êtres possédés
        //PossessInteraction possessInteraction = collision.gameObject.GetComponent<PossessInteraction>();
        //if (possessInteraction != null && !possessInteraction.IsPossessed)
        //{

        //    possessInteraction.CanBeInteractWith = false;
        //}
    }

    private void OnCollisionExit(Collision collision)
    {

        if (shaker == null)
        {
            return;
        }

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        //If rigidbody was affected by the shaking, it doesn't now because it's not touching our object anymore
        if (rb != null && shaker.affectedByShaking.Contains(rb))
        {
            shaker.affectedByShaking.Remove(rb);
        }

        PossessInteraction possessInteraction = collision.gameObject.GetComponent<PossessInteraction>();
        if (possessInteraction != null && !possessInteraction.IsPossessed)
        {
            possessInteraction.CanBeInteractWith = true;
        }
    }
}
