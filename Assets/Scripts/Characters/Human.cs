using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using DG.Tweening;

[System.Serializable]
public class Human : Character
{

    [Title("Human Settings")]
    [SerializeField]
    [Required]
    private Grabber grabber;

    private bool beingAttacked;

    public PlayerAbilities playerAbilities;

    [Title("", "Camera Settings")]
    [SerializeField][Required]
    private Transform cameraVisualTransform;

    [SerializeField][Required]
    private Transform cameraHandShootTransform;

    [SerializeField][Required]
    private Transform cameraHoldTransform;

    [SerializeField][Required]
    private Transform cameraDefaultTransform;

    [Title("", "Flash")]
    

    [SerializeField]
    private float flashStunDuration = 0.5f;

    [SerializeField]
    private float flashCooldown = 1.5f;

    [SerializeField]
    private float flashVibrationPower;

    [SerializeField]
    private float flashVibrationDuration;

    [SerializeField]
    private float stunVibrationPower;

    [SerializeField]
    private float stunVibrationDuration;

    [Title("", "Events")]
    [SerializeField]
    private UnityEvent onUse;

    [SerializeField]
    private UnityEvent onGrab;

    [SerializeField]
    private UnityEvent onUngrab;

    [SerializeField]
    private UnityEvent onThrow;

    [SerializeField]
    private UnityEvent onFlash;

    public UnityEvent OnUse { get => onUse; }
    public UnityEvent OnGrab { get => onGrab; }
    public UnityEvent OnUngrab { get => onUngrab; }
    public UnityEvent OnThrow { get => onThrow; }
    public UnityEvent OnFlash { get => onFlash; }

    [SyncVar(hook = nameof(UpdateFlashVisuals))]
    private bool canFlash = false;

    [SyncVar(hook = nameof(UpdatePastFlashVisuals))]
    private bool canRevealPastObject = false;

    private bool isStunned = false;

    private bool isFlashing = false;

    

    public override void OnStartServer()
    {
        base.OnStartServer();

        playerAbilities.BasicCamera.onChange.AddResponseAndInvoke(UpdateFlashFromData);
        playerAbilities.PastCamera.onChange.AddResponseAndInvoke(UpdatePastFlashFromData);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        UpdateFlashVisuals(canFlash, canFlash);
        UpdatePastFlashVisuals(canRevealPastObject, canRevealPastObject);
    }

    public void UpdateFlashFromData(bool newValue)
    {
        canFlash = newValue;
        if (myAnimator != null)
        {
            myAnimator.SetBool("HasCamera", newValue);
        }
    }

    public void UpdatePastFlashFromData(bool newValue)
    {
        GetComponentInChildren<PhotoFlash>().pastUpgrade = newValue;
        canRevealPastObject = newValue;
    }

    private void UpdateFlashVisuals(bool oldValue, bool newValue)
    {
        cameraVisualTransform.gameObject.SetActive(newValue);

        if (newValue)
        {
            cameraVisualTransform.SetParent(cameraHoldTransform);

            cameraVisualTransform.localRotation = Quaternion.identity;
            cameraVisualTransform.localPosition = Vector3.zero;
        }

    }

    private void UpdatePastFlashVisuals(bool oldValue, bool newValue)
    {

    }

    public GrabInteraction GrabbedObject
    {
        get
        {
            return grabber.GrabbedObject;
        }
    }

    public Grabber Grabber
    {
        get { return grabber; }
    }


    private void Update()
    {
        characterMovement.MoveInput = MoveInput;

        if (isServer)
        {
            // ----- CHEAT CODES TO UNLOCK CAMERA UPGRADES -----
            if (Input.GetKeyDown(KeyCode.L))
            {
                playerAbilities.BasicCamera.value = true;
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                playerAbilities.PastCamera.value = true;
            }
            // ------------------------------------------------
        }
    }

    public void Interact()
    {
        //If we hold an object
        if (grabber.IsGrabbingObject())
        {
            if (interactionArea.HasInteractable())
            {
                /**
                 * The only case where you use an object you are grabbing is to 
                 * interact with an openable
                */
                // If it's an interactable object
                Openable openable = interactionArea.InteractableObject.GetComponent<Openable>();
                if (openable != null && openable.GrabbedObjectCanOpen(this))
                {
                    // Use it
                    Interaction();

                    // If the object has not been used by the interaction
                    if (grabber.IsGrabbingObject())
                    {
                        // Ungrab it
                        grabber.UnGrab();
                    }

                    interactionArea.EnableHighlight = true;

                }
                else
                {
                    // Ungrab it
                    grabber.UnGrab();
                    interactionArea.EnableHighlight = true;
                }
            }
            else
            {
                // Ungrab
                grabber.UnGrab();
                interactionArea.EnableHighlight = true;
            }
        }
        else
        {
            if (interactionArea.HasInteractable() && !IsWaving)
            {
                // If it's an interactable object
                UseInteraction interaction = interactionArea.InteractableObject.GetComponent<UseInteraction>();
                if (interaction != null)
                {
                    // Use it
                    Interaction();
                }
                else
                {
                    Grab();
                }
            }
        }
    }

    public void Throw()
    {
        grabber.Throw();
        interactionArea.EnableHighlight = true;
    }
    public void Grab()
    {
        //grabber.ManageGrab(interactionArea);
        // If it's an grable object
        GrabInteraction grabInteraction = interactionArea.InteractableObject.GetComponent<GrabInteraction>();
        if (grabInteraction != null && !IsWaving)
        {
            // Grab it
            grabber.Grab(grabInteraction);
            interactionArea.EnableHighlight = false;
        }
    }

    public void ConsumeGrabbedObject()
    {
        grabber.ConsumeGrabbedObject();
    }

    [Server]
    public void Interaction()
    {
        interactionArea.InteractableObject.GetComponent<UseInteraction>().HumanAction(this);
        RpcInteractionUIEvent();
    }

    public void FlashAttack()
    {
        PhotoFlash photoFlash = GetComponentInChildren<PhotoFlash>();

        // if the attack is spammed, it's possible the gameobject is not re-attached to the human fast enough
        if (photoFlash != null && canFlash && !IsWaving && !grabber.IsGrabbingObject() && !isStunned && !isFlashing)
        {
           
            StartCoroutine(DisableCharacterMovementFor(flashStunDuration, false));

            Sequence mySequence = DOTween.Sequence();
            mySequence.AppendCallback(() =>
            {
                isFlashing = true;

                if (myAnimator != null)
                {
                    myAnimator.SetBool("Flashing", isFlashing);
                }

                cameraVisualTransform.SetParent(cameraHandShootTransform);

                cameraVisualTransform.localRotation = Quaternion.identity;
                cameraVisualTransform.localPosition = Vector3.zero;
            })
            .AppendInterval(flashStunDuration)
            .AppendCallback(() =>
            {
                isFlashing = false;

                if (myAnimator != null)
                {
                    myAnimator.SetBool("Flashing", isFlashing);
                }
                cameraVisualTransform.SetParent(cameraHoldTransform);

                cameraVisualTransform.localRotation = Quaternion.identity;
                cameraVisualTransform.localPosition = Vector3.zero;
            });
        }
    }

    /// <summary>
    /// Called via the animator when we play the flash animation.
    /// If we are the server, enable the flash attack.
    /// If we are the client, simply play the flash animation.
    /// </summary>
    public void OnFlashAnimationEvent()
    {
        Debug.Log("On Flash Animation Event");

        PhotoFlash photoFlash = GetComponentInChildren<PhotoFlash>();
        if(photoFlash != null)
        {
            if (isServer)
            {
                photoFlash.FlashAttack();
            }

            photoFlash.FlashAnimation();
            VibrationEvent(flashVibrationPower, flashVibrationDuration);

            OnFlash?.Invoke();
        }
    }

    /// <summary>
    /// TODO : Reset the human (ungrab object, cancel wave...) to initial state 
    /// </summary>
    public override void ResetCharacter()
    {
        base.ResetCharacter();

        isStunned = false;
        isFlashing = false;

        if (myAnimator != null)
        {
            myAnimator.SetBool("Stunned", false);
            myAnimator.SetBool("Flashing", false);
        }

        cameraVisualTransform.SetParent(cameraHoldTransform);

        cameraVisualTransform.localRotation = Quaternion.identity;
        cameraVisualTransform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// As a human we can only wave if we are not grabbing an object, not stunned and not currently flashing.
    /// </summary>
    /// <returns></returns>
    protected override bool CanWave()
    {
        return base.CanWave() && !grabber.IsGrabbingObject() && !isStunned && !isFlashing;
    }

    /// <summary>
    /// Stun the human for stunDuration and knockback him of power force  based on the position of the attack.
    /// </summary>
    /// <param name="attackTransform"></param>
    /// <param name="power"></param>
    /// <param name="stunDuration"></param>
    [Button]
    public void HandleAttack(Transform attackTransform, float power, float stunDuration)
    {
        ResetCharacter();

        Vector3 moveDirection = transform.position - attackTransform.position;
        moveDirection.y = 0f;

        RpcVibrationEvent(stunVibrationPower, stunVibrationDuration);

        StartCoroutine(DisableCharacterMovementFor(stunDuration));

        GetComponent<Rigidbody>().AddForce(moveDirection.normalized * power, ForceMode.Impulse);

        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendCallback(() =>
        {
            if (myAnimator != null)
            {
                myAnimator.SetBool("Stunned", true);

            }

            isStunned = true;
        })
        .AppendInterval(stunDuration)
        .AppendCallback(() =>
        {
            if (myAnimator != null)
            {
                myAnimator.SetBool("Stunned", false);
            }

            isStunned = false;
        });
    }

    private void OnDestroy()
    {
        if (grabber.IsGrabbingObject())
        { grabber.UnGrab(); }
    }

    [ClientRpc]
    public void RpcInteractionUIEvent()
    {
        OnUse?.Invoke();
    }

    [ClientRpc]
    public void RpcGrabUIEvent()
    {
        OnGrab?.Invoke();
    }

    [ClientRpc]
    public void RpcUngrabUIEvent()
    {
        OnUngrab?.Invoke();
    }

    [ClientRpc]
    public void RpcThrowUIEvent()
    {
        OnThrow?.Invoke();
    }
}


