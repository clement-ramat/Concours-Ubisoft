using DG.Tweening;
using Mirror;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
class TypeWriterSpawnable
{
    public GameObject objectPrefab;
    public int keyIndex = -1;
}

public class TypeWriterBehavior : NetworkBehaviour
{

    [Title("Key Stuck Settings")]
    [SerializeField]
    private bool hasKeyStuck = false;

    [Title("", "Animation Settings")]
    [SerializeField]
    private Transform keyVisualsTransform;

    [SerializeField]
    private List<Transform> moveTransforms = new List<Transform>();

    [SerializeField]
    private float animationDuration = 0.25f;

    [Title("", "Spawnable Settings")]
    [SerializeField]
    private TypeWriterSpawnable typeWriterSpawnable;

    [SerializeField]
    [Required]
    private Transform spawnTransform;

    [SerializeField]
    private float launchPower;

    [Title("", "Events")]
    public UnityEvent OnKeyMove;
    public UnityEvent OnKeyLaunch;

    [ReadOnly]
    private int nbTimeDone = 0;

    private bool hasSpawnKey = false;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!hasKeyStuck)
        {
            Destroy(keyVisualsTransform.gameObject);
        }
    }

    public void MoveKey(Vector3 direction)
    {
        if(!hasKeyStuck || direction == Vector3.zero)
        {
            return;
        }

        nbTimeDone++;

        if (nbTimeDone > moveTransforms.Count)
        {

            if (!hasSpawnKey)
            {
                hasSpawnKey = true;

                if (OnKeyLaunch != null)
                {
                    OnKeyLaunch.Invoke();
                }

                Destroy(keyVisualsTransform.gameObject);

                if (isServer)
                {
                    GameObject gameObject = Instantiate(typeWriterSpawnable.objectPrefab, spawnTransform.position, spawnTransform.rotation);

                    KeyBehavior keyBehavior = gameObject.GetComponent<KeyBehavior>();
                    if (keyBehavior != null)
                    {
                        keyBehavior.Index = typeWriterSpawnable.keyIndex;
                    }

                    NetworkServer.Spawn(gameObject);

                    Rigidbody keyRigidbody = gameObject.GetComponent<Rigidbody>();
                    if (keyRigidbody != null)
                    {
                        keyRigidbody.AddForce(-1 * gameObject.transform.forward * launchPower, ForceMode.Impulse);
                    }
                }
            }
        }
        else
        {

            keyVisualsTransform.DOMove(moveTransforms[nbTimeDone - 1].position, animationDuration);
            keyVisualsTransform.DORotate(moveTransforms[nbTimeDone - 1].rotation.eulerAngles, animationDuration);

            if (OnKeyMove != null)
            {
                OnKeyMove.Invoke();
            }
        }
    }
}
