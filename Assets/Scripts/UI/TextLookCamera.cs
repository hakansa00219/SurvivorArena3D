using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLookCamera : MonoBehaviour
{
    private Camera cameraToLookAt;

    void Start()
    {
        cameraToLookAt = Camera.main;
    }

    void Update()
    {
        if(cameraToLookAt != null)
        {
            Vector3 v = cameraToLookAt.transform.position - transform.position;
            v.x = v.z = 0.0f;
            transform.LookAt(cameraToLookAt.transform.position - v);
            transform.Rotate(0, 180, 0);
        }
        
    }
}
