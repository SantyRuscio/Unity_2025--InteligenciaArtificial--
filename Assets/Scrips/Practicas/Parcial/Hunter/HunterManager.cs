using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class HunterManager : MonoBehaviour
{
    public static HunterManager Instance { get; private set; }

    private List<Hunter> _hunters = new List<Hunter>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterHunter(Hunter hunter)
    {
        if (!_hunters.Contains(hunter))
            _hunters.Add(hunter);
    }

    public void UnregisterHunter(Hunter hunter)
    {
        _hunters.Remove(hunter);
    }

    public Hunter GetClosestHunter(Vector3 fromPos)
    {
        Hunter closest = null;
        float minDist = Mathf.Infinity;

        foreach (var h in _hunters)
        {
            if (h == null) continue;

            float dist = Vector3.Distance(fromPos, h.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = h;
            }
        }
        return closest;
    }
}

