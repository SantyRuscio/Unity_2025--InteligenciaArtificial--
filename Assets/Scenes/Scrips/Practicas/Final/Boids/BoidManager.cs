using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================

public class BoidManager : MonoBehaviour
{
    public static BoidManager Instance;

    public List<AllyBoidFSM> boids = new();

    void Awake()
    {
        Instance = this;
    }

    public void Register(AllyBoidFSM b)
    {
        if (!boids.Contains(b)) boids.Add(b);
    }

    public void Unregister(AllyBoidFSM b)
    {
        if (boids.Contains(b)) boids.Remove(b);
    }
}