using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastObjectBehavior : NetworkBehaviour
{

    [SerializeField]
    private Collider[] ghostsColliderOnFlash;

    [SerializeField]
    private float timeActivatedWhenFlashed = 5f;

    [SerializeField]
    private float revealedEffectSpeed = 5f;

    [SerializeField]
    private Renderer[] renderers;

    [SerializeField]
    private Material regularMaterial;

    [SerializeField]
    private Material feedbackMaterial;

    [SerializeField]
    private GameObject feedbackParticles;

    [SerializeField]
    private float disappearDuration = 1f;

    [SyncVar]
    private bool isRevealed = false;
    private float currentAlphaTarget;

    [SyncVar(hook = nameof(OnClientDisappearedChange))]
    private bool isDisappeared = false;

    // Start is called before the first frame update
    void Start()
    {
        ManageGhostsCollider(false);
        ManageGhostRenderers(true, false);

        currentAlphaTarget = 0.0f;
    }

    void Update()
    {
        float alphaTarget = isRevealed ? 1.0f : 0.0f;
        currentAlphaTarget = Mathf.Lerp(currentAlphaTarget, alphaTarget, Time.deltaTime * revealedEffectSpeed);

        foreach (Renderer renderer in renderers)
        {
            if (renderer.material)
            {
                renderer.material.SetFloat("Vector1_C688DFF3", currentAlphaTarget);
            }
        }
    }

    private void ManageGhostRenderers(bool value, bool useParticles = true)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = value;
        }

        if ((feedbackParticles != null) && useParticles && !value)
        {
            PlayFeedbackParticles();
            RpcPlayFeedbackParticles();
        }
    }

    private void PlayFeedbackParticles()
    {

        ParticleSystem[] particles = feedbackParticles.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }

    }

    [ClientRpc]
    private void RpcPlayFeedbackParticles()
    {
        PlayFeedbackParticles();
    }


    private void ManageGhostsCollider(bool value)
    {
        foreach (Collider collider in ghostsColliderOnFlash)
        {
            collider.enabled = value;
        }
    }

    IEnumerator TimeActivatedCoCouroutine()
    {
        ManageGhostsCollider(true);

        isRevealed = true;

        yield return new WaitForSeconds(timeActivatedWhenFlashed);

        isRevealed = false;

        ManageGhostsCollider(false);
    }

    [Server]
    public void OnFlashed()
    {
        if (isDisappeared)
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(TimeActivatedCoCouroutine());
    }

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (isRevealed)
        {
            return;
        }

        StopAllCoroutines();
        isDisappeared = true;
    }

    [Server]
    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(DisappearCoRoutine());
    }

    private void OnClientDisappearedChange(bool oldValue, bool newValue)
    {
        ManageGhostRenderers(!newValue);
    }

    IEnumerator DisappearCoRoutine()
    {
        yield return new WaitForSeconds(disappearDuration);
        isDisappeared = false;
    }
}
