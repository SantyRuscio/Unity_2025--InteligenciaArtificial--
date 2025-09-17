using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterManager : MonoBehaviour
{
    public static HunterManager Instance { get; private set; }
    private List<Transform> _targets = new List<Transform>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterTarget(Transform target)
    {
        if (!_targets.Contains(target))
            _targets.Add(target);
    }

    public void UnregisterTarget(Transform target)
    {
        _targets.Remove(target);
    }

    public Transform GetClosestTarget(Vector3 fromPos)
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var t in _targets)
        {
            if (t == null) continue;

            float dist = Vector3.Distance(fromPos, t.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = t;
            }
        }
        return closest;
    }
}
