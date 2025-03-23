using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class PullingPower : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1f;
    private SphereCollider _collider;

    private void Awake()
    {
        _collider = transform.GetComponent<SphereCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.attachedRigidbody && other.tag == "Food")
        {
            other.attachedRigidbody.useGravity = false;
            
            Vector3 direction = -(other.transform.position - transform.GetComponentInParent<Transform>().position);
            other.attachedRigidbody.AddForce(direction*_speed);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody && other.tag == "Food")
        {
            other.attachedRigidbody.velocity = Vector3.zero;
        }

    }
}
