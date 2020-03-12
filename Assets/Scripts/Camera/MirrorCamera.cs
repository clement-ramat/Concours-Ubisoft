using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCamera : MonoBehaviour
{

    public Transform cameraTransform;

    public Transform mirrorTransform;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(cameraTransform.position.x , cameraTransform.position.y, 2 * mirrorTransform.position.z - cameraTransform.position.z);
        transform.LookAt(mirrorTransform);
        //transform.rotation = cameraTransform.rotation;

        //transform.Rotate(new Vector3(0, 1, 0), 180, Space.World);
        
    }

    public void CalculateRotation()
    {
        Vector3 dir = (cameraTransform.position - mirrorTransform.position).normalized;

        Quaternion rot = Quaternion.LookRotation(dir);
        rot.eulerAngles = mirrorTransform.eulerAngles - rot.eulerAngles;
        transform.localRotation = rot;
    }

}
