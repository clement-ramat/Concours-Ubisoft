using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

public class PaintingBehavior : MonoBehaviour
{

    [Title("Animation Movement")]
    [SerializeField]
    private GameEvent EndOfLevelGameEvent;

    [SerializeField]
    private GlobalBool paintingAcquired;

    [Title("Animation Movement")]
    [SerializeField]
    private GlobalPlayerGhost globalPlayerGhost;

    [SerializeField]
    private GlobalPlayerHuman globalPlayerHuman;

    [SerializeField]
    private float animationDelay = 2f;

    [SerializeField]
    private float inAirYOffset = 1f;

    [SerializeField]
    private float inAirDuration = 1f;

    [SerializeField]
    private float inAirDelay = 1.5f;

    [SerializeField]
    private float rotationDuration = 2f;

    [SerializeField]
    private float toPlayerDuration = 0.75f;

    private void Start()
    {
        // --- TODO : Make it via an game cut-scene 
        if (globalPlayerHuman.value != null)
        {
            globalPlayerHuman.componentCache.ResetCharacter();
            globalPlayerHuman.componentCache.CanMove(false);
        }

        if (globalPlayerGhost.value != null)
        {
            globalPlayerGhost.componentCache.ResetCharacter();
            globalPlayerGhost.componentCache.CanMove(false);
        }
        // -------------------------------------

        Sequence rotationSequence = DOTween.Sequence();
        rotationSequence
            .AppendInterval(animationDelay).AppendCallback(() => {
                transform.DORotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.FastBeyond360)
                .SetRelative()
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
            });
            
        Sequence mySequence = DOTween.Sequence();
        mySequence
            .AppendInterval(animationDelay)
            .Append(transform.DOJump(new Vector3(transform.position.x, transform.position.y + inAirYOffset, transform.position.z), 3f, 1, inAirDuration))
            .AppendInterval(inAirDelay);

        if (globalPlayerHuman.value != null)
        {
            mySequence.Append(transform.DOJump(globalPlayerHuman.value.transform.position, 3f, 1, toPlayerDuration));
            mySequence.Join(transform.DOScale(Vector3.zero, toPlayerDuration * 1.25f)).SetEase(Ease.InSine);
        }

        mySequence.AppendInterval(0.75f);

        // When we have finish the animation sequence
        mySequence.OnComplete(() =>
        {
            GivePainting();

            // --- TODO : Make it via an game cut-scene 
            if (globalPlayerHuman.value != null)
            {
                globalPlayerHuman.componentCache.CanMove(true);
            }

            if (globalPlayerGhost.value != null)
            {
                globalPlayerGhost.componentCache.CanMove(true);
            }
            // -------------------------------------
        });
    }

    private void GivePainting()
    {
        paintingAcquired.value = true;

        EndOfLevelGameEvent.Raise();
    }
}
