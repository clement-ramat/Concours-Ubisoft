using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Character
{

    [SerializeField]
    private GrabberLightObject grabberLightObject;


    //Garde une référence du grabber en utilisation
    private Grabber currentlyUsedGrabber = null;

    private void Update()
    {
        // USE INTERACTION
        if (Input.GetKeyDown("e"))
        {
            if(interactionArea.HasInteractable())
            {
                // retrieve the UseInteraction component (if this object has one) and use it
                UseInteraction interaction = interactionArea.InteractableObject.GetComponent<UseInteraction>();
                if (interaction) interaction.HumanAction(this);
            }
        }

        //INPUT MOCHE POUR TESTER
        if (Input.GetKeyDown("space"))
        {
            //GRAB ACTION
           
            if (currentlyUsedGrabber != null)
            {
                //UNGRAB
                Ungrab();
            }
            else if (interactionArea.HasInteractable())
            {
                //GRAB
                interactionArea.InteractableObject.GetComponent<GrabInteraction>().HumanAction(this); //Beau et élégant
            }
        }

        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        characterMovement.SetMoveInput(moveInput);
    }

    /*
     * Grab Objects
     * 
     * Les objets qui peuvent se faire grab appelent "GrabLightObject", "GrabHeavyObject" du personnage
     * ou autre qui appeleront le Component de grab qui contient l'action correct.
     * 
     * */
    /// <summary>
    /// Appelé par les objets pour se faire grab. 
    /// </summary>
    /// <param name="go"></param>
    public void GrabLightObject(GameObject go)
    {
        grabberLightObject.Grab(go);
        currentlyUsedGrabber = grabberLightObject;
    }

    public void Ungrab()
    {
        currentlyUsedGrabber.Ungrab();
        currentlyUsedGrabber = null;
    }
}




