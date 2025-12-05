using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;
    private float currentHealth;

    public bool IsDead => currentHealth <= 0;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        BoidManager.Instance?.Unregister(GetComponent<AllyBoidFSM>());
        Destroy(gameObject);
    }
}

