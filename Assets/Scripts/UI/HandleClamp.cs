using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleClamp : MonoBehaviour
{
    RectTransform rect;
    Vector3 newVector;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }
    void Update()
    {
        newVector.x = rect.localPosition.x;
        newVector.y = Mathf.Clamp(rect.localPosition.y, 0f, 128f);
        newVector.z = rect.localPosition.z;
        rect.localPosition = newVector;
    }
}
