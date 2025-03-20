using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    private float _force = 500;
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.transform.GetComponent<Rigidbody>();
        Vector3 direction = this.transform.up - this.transform.forward;
        rb.AddForce(direction * _force);
    }
}
