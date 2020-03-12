using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using Rewired;

/// <summary>
/// Parent class to all characters (players, ghost, human)
/// </summary>
[System.Serializable][SelectionBase]
public abstract class Character : NetworkBehaviour
{
    [Title("Character Settings")]
    [Title("", "References")]
    [SerializeField]
    [Required]
    protected Transform visualsParent;

    [SerializeField]
    [Required]
    protected Transform colliderParent;

    [SerializeField]
    [Required]
    protected Rigidbody rb;

    [Title("", "Movement")]
    [SerializeField]
    [Required]
    protected CharacterMovement characterMovement;

    public Vector3 MoveInput;

    [Title("", "Wave Settings")]
    [SerializeField]
    private float waveDuration = 2f;

    [SerializeField]
    [Range(0, 1)]
    private float waveMoveSpeedReduction = 0.75f;

    [Title("", "Mechanics")]
    [SerializeField]
    [Required]
    public InteractionArea interactionArea;


    public UnityEvent OnEmote;

    [Title("", "Vibration")]
    public UnityEventFloat2 OnVibration;
    public float vibrationFactor = 1f;

    protected Animator myAnimator;
    protected NetworkAnimator myNetworkAnimator;

    private bool isWaving = false;
    public bool IsWaving
    {
        get
        {
            return isWaving;
        }
    }

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        myNetworkAnimator = GetComponent<NetworkAnimator>();
    }

    [Server]
    public void CanMove(bool value)
    {
        characterMovement.CanMove = value;
    }

    public virtual void ResetCharacter()
    {
        StopAllCoroutines();

        isWaving = false;
        characterMovement.ResetMoveSpeed();
    }

    [Server]
    public IEnumerator DisableCharacterMovementFor(float duration, bool disable = true)
    {

        if (disable)
        {
            characterMovement.enabled = false;
        }
        else
        {
            characterMovement.CanMove = false;
        }

        yield return new WaitForSeconds(duration);

        if (disable)
        {
            characterMovement.enabled = true;
        }
        else
        {
            characterMovement.CanMove = true;
        }
    }

    public void Wave()
    {
        if (CanWave() && myNetworkAnimator != null)
        {
            StartCoroutine(WaveCoRoutine());
            RpcEmoteEvent();
        }
    }

    protected virtual bool CanWave()
    {
        return !isWaving;
    }

    protected IEnumerator WaveCoRoutine()
    {
        myNetworkAnimator.SetTrigger("Wave");
        characterMovement.moveSpeed *= waveMoveSpeedReduction;

        isWaving = true;

        yield return new WaitForSeconds(waveDuration);

        isWaving = false;

        characterMovement.ResetMoveSpeed();
    }

    [ClientRpc]
    public void RpcEmoteEvent()
    {
        OnEmote?.Invoke();
    }

    [ClientRpc]
    public void RpcVibrationEvent(float vibrationPower, float vibrationDuration)
    {
        OnVibration?.Invoke(vibrationPower * vibrationFactor, vibrationDuration);
    }

    public void VibrationEvent(float vibrationPower, float vibrationDuration)
    {
        OnVibration?.Invoke(vibrationPower * vibrationFactor, vibrationDuration);
    }
}
