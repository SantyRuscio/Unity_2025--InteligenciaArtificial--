using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ===============================
// Ruscio - Beghin
// ===============================

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100;
   public float currentHealth;

    public GameObject bloodEffectPrefab;
    public bool IsDead => currentHealth <= 0;

    public float CurrentHealth => currentHealth;

    public float MaxHealth => maxHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (bloodEffectPrefab != null)
        {
            GameObject fx = Instantiate(bloodEffectPrefab, transform.position + Vector3.up, Quaternion.identity);

            Destroy(fx, 2f);
        }

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

