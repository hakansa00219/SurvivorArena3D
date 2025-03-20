using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] public int value = 1;
    void Start()
    {
        this.transform.position += new Vector3(0f, 1f, 0f);
        switch (this.transform.parent.name)
        {
            case "BronzeCoinContainer":
                value = 1;
                break;
            case "SilverCoinContainer":
                value = 3;
                break;
            case "GoldCoinContainer":
                value = 10;
                break;
        }
    }
}
