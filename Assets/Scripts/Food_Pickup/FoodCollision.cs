using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCollision : MonoBehaviour
{
    private bool trigger;
    private Collider other;
    [SerializeField] private Collider myCollider;
    [SerializeField] private Character _owner; //player or enemy
    private Type myColliderType;
    public int adCount;

    public static event Action<float> TimeIncreasedEvent;
    private void Start()
    {
        myColliderType = myCollider.GetType();
        adCount = PlayerData.Instance.GetPlayerCurrentAdWatchedCount();
    }
    private void OnTriggerStay(Collider other)
    {
        if (myColliderType == typeof(CapsuleCollider)) 
        {
            if (other.tag == "Food" || other.tag == "Pickup" || other.tag == "PickupT" || other.tag == "Coins")
            {
                trigger = true;
                this.other = other;
            }
        }
    }
    private void FixedUpdate()
    {
        if (trigger)
        {
            if (other == null) return;
            //object eats food
            if (other.gameObject.tag == "Food" && other.GetComponent<Food>().Edible == true)
            {
                if (other.GetComponent<Food>().Name != transform.parent.name)
                {
                    Destroy(other.gameObject);
                    _owner.EatSomething(other.GetComponent<Food>().GetCurrentPoint());
                    trigger = false;
                }
            }
            //object eats pickup
            else if (other.gameObject.tag == "Pickup")
            {
                if (_owner._hasMissile)
                {
                    Destroy(other.transform.parent.gameObject);
                    trigger = false;
                }
                _owner._hasMissile = true;
                this.transform.parent.Find("Pickup_Parent").GetChild(0).gameObject.SetActive(true);
            } else if (other.gameObject.tag == "PickupT" && _owner.tag == "Player")
            {
                TimeIncreasedEvent?.Invoke(30f);
                Destroy(other.transform.parent.gameObject);
            }

            else if (other.tag == "Coins" && _owner.tag == "Player")
            {
                //destroy coin
                Destroy(other.transform.parent.gameObject);
                //sound
                _owner.CoinAlindi();
                //increase player coin as coin value
                //_adCount = PlayerData.Instance.GetPlayerCurrentAdWatchedCount();
                adCount += other.transform.parent.GetComponent<Coin>().value;
                //update DB.
                
                //updating UI in the player script.
                //debug current coin amount
                Debug.Log("Current coin amount = " + adCount);
            }


        }
    }
}
