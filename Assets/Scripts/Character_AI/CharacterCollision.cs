using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    [SerializeField] private Character _owner;
    private bool trigger;
    private Collider other;
    [SerializeField] private Collider myCollider;
    private Type myColliderType;
    private void Start()
    {
        myColliderType = myCollider.GetType();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (myColliderType == typeof(SphereCollider) && !myCollider.isTrigger)
        {
            if (transform.parent.tag == "Player")
            {
                if (other.transform.parent.tag == "Enemy" && other.tag == "CentralCollider" && other.isTrigger)
                {
                    trigger = true;
                    this.other = other;
                }
            } else if (transform.parent.tag == "Enemy")
            {
                if ((transform.parent != other.transform.parent) && (other.transform.parent.tag == "Player" || other.transform.parent.tag == "Enemy") && other.tag == "CentralCollider" && other.isTrigger)
                {
                    trigger = true;
                    this.other = other;
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if(trigger)
        {
            if (other != null)
            {
                if (other == null) return;
                var _othersPoint = other.transform.parent.GetComponent<Character>().GetCurrentPoint();
                var _currentPoint = _owner.GetCurrentPoint();
                if ((_currentPoint >= _othersPoint * 1.33f)) // 3 / 4 oran
                {
                    _owner.EatSomething(_othersPoint);
                    MessageBox.Instance.CreateText(_owner.name, other.transform.parent.name, MessageBox.Action.Eaten);
                    if (other.transform.parent.tag == "Enemy") Destroy(other.transform.parent.gameObject);
                    else if (other.transform.parent.tag == "Player") other.transform.parent.GetComponent<Player>().OnDeath();
                    trigger = false;
                }
            }
        }
    }
}
