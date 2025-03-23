using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindClosestEnemy : MonoBehaviour
{
    public readonly static HashSet<FindClosestEnemy> Pool = new HashSet<FindClosestEnemy>();

    private void OnEnable()
    {
        FindClosestEnemy.Pool.Add(this);
    }

    private void OnDisable()
    {
        FindClosestEnemy.Pool.Remove(this);
    }
    public static Dictionary<ulong, FindClosestEnemy> FindClosest(Transform transform, int count)
    {
        // how enemys will be searched
        var lookUpEnemyCount = count <= 3 ? count-1 : 3;
        bool added = false;
        Dictionary<ulong, FindClosestEnemy> dict = new Dictionary<ulong, FindClosestEnemy>();
        
        // initialize null values number of count
        // -1 for self.
        for (int i = 0; i < lookUpEnemyCount; i++)
        {
            dict.Add(ulong.MaxValue - System.Convert.ToUInt64(i), null);
        }
        
        //get all enemys around
        var e = FindClosestEnemy.Pool.GetEnumerator();
        while (e.MoveNext())
        {
            // if e is self, continue
            if (e.Current.gameObject == transform.gameObject) continue;
            
            // if added, sort again
            if (added)
            {
                dict = dict.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, z => z.Value);
            }

            added = false;

            // calculate enemys points, and convert to ulong
            ulong d = System.Convert.ToUInt64((e.Current.transform.position - transform.position).sqrMagnitude); // su an baktigin
            
            for(int i = 0; i< lookUpEnemyCount; ++i)
            {
                // if next enemy is lower any enemy on the list, replace new enemy and point
                if(d <= dict.ElementAt(i).Key && !dict.ContainsKey(d))
                {
                    dict.Remove(dict.ElementAt(i).Key);
                    dict.Add(d, e.Current);
                    added = true;
                    break;
                }
            }
        }
        dict = dict.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, z => z.Value);
        return dict;
    }
}
