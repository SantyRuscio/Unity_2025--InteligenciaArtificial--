using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100;
   public float currentHealth;

    public bool IsDead => currentHealth <= 0;

    public float CurrentHealth => currentHealth;

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

