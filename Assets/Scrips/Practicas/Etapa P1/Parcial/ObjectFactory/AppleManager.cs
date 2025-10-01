using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleManager : MonoBehaviour
{
    public static AppleManager instance;

    [Header("Spawn Points para las manzanas")]
    [SerializeField] private Transform[] spawnPoints;
    public Transform[] SpawnPoints => spawnPoints;

    // Lista de manzanas activas en escena
    private List<Transform> apples = new List<Transform>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public Transform GetRandomSpawn()
    {
        if (spawnPoints.Length == 0)
            return null;

        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index];
    }

    public void RegisterApple(Transform apple)
    {
        if (!apples.Contains(apple))
            apples.Add(apple);
    }

    public void UnregisterApple(Transform apple)
    {
        if (apples.Contains(apple))
            apples.Remove(apple);
    }

    public Transform GetClosestApple(Vector3 fromPos, float maxRadius = Mathf.Infinity)
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var apple in apples)
        {
            if (apple == null) continue;

            float dist = Vector3.Distance(fromPos, apple.position);
            if (dist < minDist && dist <= maxRadius)
            {
                minDist = dist;
                closest = apple;
            }
        }
        return closest;
    }
}
