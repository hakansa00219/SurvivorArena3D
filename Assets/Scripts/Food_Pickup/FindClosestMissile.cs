using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindClosestMissile : MonoBehaviour
{
    public readonly static HashSet<FindClosestMissile> Pool = new HashSet<FindClosestMissile>();

    
    private void OnEnable()
    {
        FindClosestMissile.Pool.Add(this);
    }

    private void OnDisable()
    {
        FindClosestMissile.Pool.Remove(this);
    }
    public static FindClosestMissile FindClosest(Vector3 pos)
    {
        FindClosestMissile result = null;
        float dist = float.PositiveInfinity;
        var e = FindClosestMissile.Pool.GetEnumerator();
        while (e.MoveNext())
        {
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
