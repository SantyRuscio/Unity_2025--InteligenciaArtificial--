using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AppleManager : MonoBehaviour
{
    public static AppleManager instance;

    [SerializeField] private Transform[] spawnPoints; 
    public Transform[] SpawnPoints => spawnPoints;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Método para devolver un spawn random
    public Transform GetRandomSpawn()
    {
        if (spawnPoints.Length == 0)
            return null;

        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index];
    }
}



