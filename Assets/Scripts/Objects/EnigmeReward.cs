using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Mirror;
using DG.Tweening;
using FMODUnity;

public class EnigmeReward : NetworkBehaviour
{

    [SerializeField]
    [SceneObjectsOnly]
    private ChestSpawnable[] objectsToSpawn;

    [SerializeField]
    [Required]
    private Transform spawnPoint;

    [SerializeField]
    [Required]
    private Transform frontPoint;

    [SerializeField]
    private float dropLerpDuration = 1.5f;

    [FMODUnity.EventRef]
    public string enigmeResolvedEventRef = "event:/SFX 2D/resolve_jingle";

    private FMOD.Studio.EventInstance playEnigmeResolved;

    public void Awake()
    {
        playEnigmeResolved = FMODUnity.RuntimeManager.CreateInstance(enigmeResolvedEventRef);
    }


    public void DropReward()
    {
        if (isServer)
        {
            if (objectsToSpawn.Length > 0)
            {
                PlayEnigmeResolvedJingle();

                foreach (ChestSpawnable objectToSpawn in objectsToSpawn)
                {

                    GameObject newObject = Instantiate(objectToSpawn.objectPrefab, spawnPoint.position, spawnPoint.rotation);
                    KeyBehavior keyBehavior = newObject.GetComponent<KeyBehavior>();

                    NetworkServer.Spawn(newObject);

                    if (keyBehavior != null)
                    {
                        keyBehavior.Index = objectToSpawn.keyIndex;
                    }

                    // Disabling the colliders of the object to spawn (to not collide with the chest)
                    List<Collider> colliders = new List<Collider>(newObject.GetComponentsInChildren<Collider>());
                    colliders.ForEach(collider =>
                    {
                        collider.enabled = false;
                    });

                    // Disabling the gravity on the rigidbody, to prevent the object from falling through the map
                    Rigidbody rigidbody = newObject.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.useGravity = false;
                    }


                    // Because the colliders were deactivated AFTER instantiating and surely got pushed away by the chest, 
                    // we need to put the object back to its original position
                    newObject.transform.position = spawnPoint.position;

                    Sequence mySequence = DOTween.Sequence();
                    mySequence.AppendInterval(0.1f).Append(
                        newObject.transform.DOJump(frontPoint.position, 1f, 1, dropLerpDuration).SetEase(Ease.InOutSine)
                        ).Join(
                        newObject.transform.DORotate(frontPoint.rotation.eulerAngles, dropLerpDuration).SetEase(Ease.InOutSine)
                        ).OnComplete(() =>
                        {
                            colliders.ForEach(collider =>
                            {
                                collider.enabled = true;
                            });

                            if (rigidbody != null)
                            {
                                rigidbody.useGravity = true;
                            }
                        });
                }
            }
        }
    }

    public void PlayEnigmeResolvedJingle()
    {
        Debug.Log("Playing resolved Jingle");
        playEnigmeResolved.setParameterByName("Jingle Num", 1);
        playEnigmeResolved.start();
    }
}

