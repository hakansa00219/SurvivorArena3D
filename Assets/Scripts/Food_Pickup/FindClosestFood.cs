using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindClosestFood : MonoBehaviour
{
    public readonly static HashSet<FindClosestFood> Pool = new HashSet<FindClosestFood>();

    private void OnEnable()
    {
        FindClosestFood.Pool.Add(this);
    }

    private void OnDisable()
    {
        FindClosestFood.Pool.Remove(this);
    }
    public static FindClosestFood FindClosest(Vector3 pos , string enemyName)
    {
        FindClosestFood result = null;
        float dist = float.PositiveInfinity;
        var e = FindClosestFood.Pool.GetEnumerator();
        while (e.MoveNext())
        {
            if (enemyName == e.Current.transform.GetComponent<Food>().Name) continue;
            float d = (e.Current.transform.position - pos).sqrMagnitude;
            if (d < dist)
            {
                result = e.Current;
                dist = d;
            }
        }
        return result;
    }

}
