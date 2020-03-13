using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Mirror;

[System.Serializable]
class ChestSpawnable
{
    public GameObject objectPrefab;
    public int keyIndex = -1;
    public bool spawnServer = true;
    public bool moveToChestFront = true;
}


public class ChestBehavior : Openable
{

    [Header("Chest Behavior Settings")]

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

    public override void Open()
    {
        base.Open();
        Debug.Log("Chest has opened");

        alreadyUsed = true;

        Sequence animationSequence = DOTween.Sequence();
        animationSequence.AppendInterval(1.25f).OnComplete(
            () =>
            {
                Animator animator = GetComponentInChildren<Animator>();
                if (animator != null)
                {
                    animator.SetBool("Open", true);
                }
            });

        // Activate every chests objects and set them to the spawn position
        foreach (ChestSpawnable objectToSpawn in objectsToSpawn)
        {

            // We need to spawn the object server-side only
            if (objectToSpawn.spawnServer)
            {

                if (!isServer)
                {
                    break;
                }

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

                if (objectToSpawn.moveToChestFront)
                {
                    Sequence mySequence = DOTween.Sequence();
                    mySequence.AppendInterval(2f).Append(
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
            else
            {
                GameObject newObject = Instantiate(objectToSpawn.objectPrefab, spawnPoint.position, spawnPoint.rotation);

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

                if (objectToSpawn.moveToChestFront)
                {
                    Sequence mySequence = DOTween.Sequence();
                    mySequence.AppendInterval(2f).Append(
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

    public override void Close()
    {
        base.Close();

        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetBool("Open", false);
        }
    }
}
