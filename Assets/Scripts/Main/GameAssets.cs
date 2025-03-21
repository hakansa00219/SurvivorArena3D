﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
             
            return _i;
            
        }
    }

    public GameObject FoodObject;
    public GameObject FuzeObject;
    public GameObject FuzePickupObject;
    public GameObject Enemy;
    public RectTransform LoadingImage;
    
}
