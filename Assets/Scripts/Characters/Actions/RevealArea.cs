using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RevealArea : MonoBehaviour
{

    [SerializeField]
    private Vector3 activeScale = new Vector3(1, 1, 1);

    [SerializeField]
    private float timeToBlast = 0.5f;

    [Title("", "Explosion Settings")]
    [SerializeField]
    private float explosionForce = 350;

    [SerializeField]
    private float explosionRadius = 3f;

    [SerializeField]
    private LayerMask explosionLayerMask;

    [SerializeField]
    private GameObject particleEffects;

    private Collider myCollider;

    private void Start()
    {
        myCollider = GetComponent<Collider>();
        ResetRevealBlast();
    }


    /// <summary>
    /// Enable the blast collider and scale it to activeScale in timeToBlast seconds.
    /// Then disable the blast collider.
    /// </summary>
    /// 
    public void RevealBlast(bool isServer)
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendCallback(() =>
        {
            // Play particles
            ParticleSystem[] ps = particleEffects.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem effect in ps)
            {
                effect.Play();
            }
            // End play particles

            if (isServer)
            {
                myCollider.enabled = true;

                Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayerMask);
                for (int i = 0; i < colliders.Length; i++)
                {
                    Rigidbody rigidbody = colliders[i].GetComponentInParent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 2.0f, ForceMode.Impulse);
                    }
                }
            }
        })
        .Append(myCollider.transform.DOScale(activeScale, timeToBlast).SetEase(Ease.InOutSine))
        .AppendInterval(0.05f)
        .AppendCallback(() =>
        {
            myCollider.enabled = false;
            ResetRevealBlast();
        });
    }

    private void ResetRevealBlast()
    {
        myCollider.enabled = false;
        transform.localScale = Vector3.zero;
    }


    private void OnTriggerEnter(Collider other)
    {
        GhostEnemy ge = other.GetComponentInParent<GhostEnemy>();
        PossessInteraction possessable = other.GetComponentInParent<PossessInteraction>();
        //Debug.Log("TriggerEnter " + other);
        if (ge)
        {
            ge.Reveal();
        }
        else if (possessable && possessable.IsPossessed)
        {
            //Debug.Log(other + " is possessed");
            GhostEnemy enemy = possessable.PossessedBy.GetComponentInParent<GhostEnemy>();
            if (enemy)
            {
                //Debug.Log("enemy found");
                // fix null references
                enemy.UseGhostMeshAgent();

                possessable.ForceUnpossess();
                enemy.Reveal();
            }
        }


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

