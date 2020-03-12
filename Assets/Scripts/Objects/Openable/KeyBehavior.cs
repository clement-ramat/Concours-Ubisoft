using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBehavior : NetworkBehaviour
{
    // The index of the key (can only be used with openable of the same index)
    [SerializeField]
    private int keyIndex;

    public int Index
    {
        get
        {
            return keyIndex;
        }
        set
        {
            keyIndex = value;
        }
    }

    public void ConsumeKey()
    {
        NetworkServer.Destroy(gameObject);
    }
}
