using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class BoidsManager : MonoBehaviour
{
    public static BoidsManager Instance { get; private set; }
    private List<Boids> _boids = new List<Boids>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Registro de un boid
    public void RegisterBoid(Boids boid)
    {
        if (!_boids.Contains(boid))
            _boids.Add(boid);
    }

    // Desregistro de un boid
    public void UnregisterBoid(Boids boid)
    {
        _boids.Remove(boid);
    }

    // Devuelve el Boid más cercano a una posición
    public Boids GetClosestBoid(Vector3 fromPos)
    {
        Boids closest = null;
        float minDist = Mathf.Infinity;

        foreach (var boid in _boids)
        {
            if (boid == null) continue;

            float dist = Vector3.Distance(fromPos, boid.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = boid;
            }
        }
        return closest;
    }
}

