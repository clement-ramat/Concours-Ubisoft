using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Mirror;
using UnityEngine.Events;

public class DoorBehavior : Openable
{
    [Title("Door Settings")]
    [SerializeField]
    private float openingDuration = 2;

    [SerializeField]
    [ChildGameObjectsOnly]
    private Transform rightPivot;

    [SerializeField]
    [ChildGameObjectsOnly]
    private Transform leftPivot;

    [Title("Plank Settings")]

    [SerializeField]
    private float plankOffsetUnlock = 2.75f;

    [SerializeField]
    private float plankInitialPositionX = 1.37f;

    [SerializeField]
    private float plankAnimationDuration = 1f;

    [Title("", "Slab settings")]
    [SerializeField]
    [ChildGameObjectsOnly]
    private List<Transform> plankLocks = new List<Transform>();

    [Title("", "More planks Settings")]

    [SerializeField]
    private bool isLockedWithPlanks = false;

    [SerializeField]
    [ChildGameObjectsOnly]
    private List<Transform> morePlanks = new List<Transform>();

    [Title("", "Events")]
    public UnityEvent OnPlankOpen;
    public UnityEvent OnPlankClose;

    [Title("Lock Settings")]
    [SerializeField]
    private Transform lockTransform;
    //private Transform savedStartLockTransform;


    //public void Start()
    //{
    //    if (lockTransform != null)
    //    {
    //        savedStartLockTransform = lockTransform;
    //        Debug.Log("saving transform");
    //    }
    //}

    [SyncVar(hook = nameof(OnUnlockMorePlanksHook))]
    private bool hasUnlockMorePlank = false;

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (isLockedWithPlanks)
        {
            IsLocked = isLockedWithPlanks;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isLockedWithPlanks)
        {
            for (int i = 0; i < morePlanks.Count; i++)
            {
                MovePlank(morePlanks[i], i % 2 == 0, true);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (lockTransform != null)
        {
            lockTransform.gameObject.SetActive(IsLocked && isKeyNeeded);
        }
    }

    public override void Open()
    {
        base.Open();

        if (rightPivot != null && leftPivot != null)
        {
            rightPivot.DOLocalRotate(new Vector3(0, 90, 0), openingDuration).SetEase(Ease.OutSine);
            leftPivot.DOLocalRotate(new Vector3(0, -90, 0), openingDuration).SetEase(Ease.OutSine);
        }
    }

    public override void Close()
    {
        base.Close();

        if (rightPivot != null && leftPivot != null)
        {
            rightPivot.DOLocalRotate(new Vector3(0, 0, 0), openingDuration).SetEase(Ease.OutSine);
            leftPivot.DOLocalRotate(new Vector3(0, 0, 0), openingDuration).SetEase(Ease.OutSine);
        }
    }

    protected override void OnActivateObjectActive(ActivateObject activateObject)
    {
        base.OnActivateObjectActive(activateObject);

        RpcUnlockPlank(activateObjects.IndexOf(activateObject));
    }

    protected override void OnActivateObjectDisable(ActivateObject activateObject)
    {
        base.OnActivateObjectDisable(activateObject);

        RpcLockPlank(activateObjects.IndexOf(activateObject));
    }

    [Server]
    public void UnlockMorePlank()
    {
        if (isLockedWithPlanks)
        {
            IsLocked = false;
            hasUnlockMorePlank = true;
        }
    }

    [ClientRpc]
    private void RpcUnlockPlank(int plankIndex)
    {
        if (plankIndex > 0 && plankIndex >= plankLocks.Count && !isOpen)
        {
            return;
        }

        MovePlank(plankLocks[plankIndex], plankIndex % 2 == 0);
    }

    [ClientRpc]
    private void RpcLockPlank(int plankIndex)
    {
        if (plankIndex > 0 && plankIndex >= plankLocks.Count && !isOpen)
        {
            return;
        }

        ResetPlank(plankLocks[plankIndex]);
    }

    private void OnUnlockMorePlanksHook(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            for (int i = 0; i < morePlanks.Count; i++)
            {
                MovePlank(morePlanks[i], i % 2 == 0, false);
            }
        }
    }

    private void MovePlank(Transform plankTransform, bool toRight = true, bool instant = false)
    {
        if (toRight)
        {
            plankTransform.DOLocalMoveX(plankInitialPositionX + plankOffsetUnlock, instant ? 0.0f : plankAnimationDuration).SetEase(Ease.OutSine);
        }
        else
        {
            plankTransform.DOLocalMoveX(plankInitialPositionX - plankOffsetUnlock, instant ? 0.0f : plankAnimationDuration).SetEase(Ease.OutSine);
        }

        if (!instant && OnPlankOpen != null)
        {
            OnPlankOpen.Invoke();
        }
    }

    private void ResetPlank(Transform plankTransform, bool instant = false)
    {
        plankTransform.DOLocalMoveX(plankInitialPositionX, instant ? 0.0f : plankAnimationDuration).SetEase(Ease.OutSine);

        if (OnPlankClose != null)
        {
            OnPlankClose.Invoke();
        }
    }
}
