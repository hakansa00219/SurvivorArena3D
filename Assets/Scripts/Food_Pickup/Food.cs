using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Food : MonoBehaviour
{
    public string Name = "default";
    public bool Edible = false;
    private UInt32 points = 5;

    public void SetCurrentPoint(UInt32 points)
    {
        this.points = points;
    }

    public UInt32 GetCurrentPoint()
    {
        return points;
    }

    private void Start()
    {
        StartCoroutine(Edibility());
        if (!this.GetComponent<Rigidbody>().useGravity) Debug.Log(this.Name + " startta gravity false");
        Invoke("Gravityyy", 0.5f);
        
    }
    private void Update()
    {
        if (!this.GetComponent<Rigidbody>().useGravity) Debug.Log(this.Name + " updatede gravity false");
    }
    private IEnumerator Edibility()
    {
        yield return new WaitForSeconds(0.5f);
        Edible = true;
    }

    private void Gravityyy()
    {
        this.GetComponent<Rigidbody>().useGravity = true;
        //this.GetComponent<Rigidbody>().mass = GetCurrentPoint();
    }
}
