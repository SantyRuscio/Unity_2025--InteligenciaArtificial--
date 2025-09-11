using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AppleSpawner : MonoBehaviour
{
    [SerializeField] private AppleFactory appleFactory;

    private void Start()
    {
        SpawnApple();
    }

    private void SpawnApple()
    {
        Apples apple = appleFactory.GetLevel();

        // Elegimos un waypoint random
        Transform spawnPoint = AppleManager.instance.GetRandomSpawn();
        if (spawnPoint != null)
        {
            apple.SetPosition(spawnPoint.position);
        }

        // Suscribimos al evento para crear el siguiente Apple
        apple.OnReturnedToPool += OnAppleReturned;
    }

    private void OnAppleReturned(Apples returnedApple)
    {
        returnedApple.OnReturnedToPool -= OnAppleReturned;
        SpawnApple();
    }
}

