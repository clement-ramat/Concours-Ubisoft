using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPhysicalObject : NetworkBehaviour
{

    private Rigidbody myRigidbody;

    public override void OnStartServer()
    {
        base.OnStartServer();

        myRigidbody = GetComponent<Rigidbody>();
        if (myRigidbody != null)
        {
            myRigidbody.isKinematic = false;
        }
    }

}
