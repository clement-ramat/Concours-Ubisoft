using Mirror;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MyUnityEventVector3 : UnityEvent<Vector3>
{

}

public class HeavyPossessInteraction : PossessInteraction
{

    [Title("Heavy Interaction Settings")]

    [SerializeField]
    public IJoystickInteraction joyStickInteraction;

    public MyUnityEventVector3 OnStickMovementEvent;

    [Title("Heavy Interaction Vibration")]
    public float heavyVibrationPower = 0.2f;

    public float heavyVibrationDuration = 0.2f;

    /// <summary>
    /// Should only be used by GhostEnemy to bypass Player's Joystick requirements
    /// </summary>
    public void ForceExecutePossessInteraction()
    {
        //Use a random Vector3 to use on event
        float randX = UnityEngine.Random.Range(0f, 1f);
        float randY = UnityEngine.Random.Range(0f, 1f);
        Vector3 randVector = new Vector3(randX, randY, 0f).normalized;

        //Trigger event
        OnStickMovementEvent.Invoke(randVector);
    }

    public override void JoystickInteraction(Vector3 inputAxis)
    {
        if (joyStickInteraction)
        {
            Vector3 inputMovement = joyStickInteraction.JoystickInteraction(inputAxis);
            if (joyStickInteraction.IsJoystickInteractionValid())
            {
                OnStickMovementEvent.Invoke(inputMovement);
                OnMakeVibration?.Invoke(heavyVibrationPower, heavyVibrationDuration);

                RpcInvokeOnStickMovement(inputMovement);
            }
        }
    }

    [ClientRpc]
    private void RpcInvokeOnStickMovement(Vector3 inputMovement)
    {
        if (!isClientOnly)
        {
            return;
        }

        if(OnStickMovementEvent != null)
        {
            OnStickMovementEvent.Invoke(inputMovement);
        }
    }
}
