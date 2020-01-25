using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class ApplyStrangeForce : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
            GetComponent<Rigidbody>().isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("space"))
        {
            Debug.Log("space");
            if (hasAuthority)
                ApplyForce();
            else
                CmdApplyForce();
        }

    }

    [Command]
    public void CmdApplyForce()
    {
        Debug.Log("yo");
        ApplyForce();
    }

    private void ApplyForce()
    {
        Debug.Log("fuck");

        GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-10, 10), Random.Range(0, 10), Random.Range(-10, 10)), ForceMode.Impulse);

    }

}
