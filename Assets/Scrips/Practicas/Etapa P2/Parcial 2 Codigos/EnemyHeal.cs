using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeal : MonoBehaviour, IDamageable
{
    [Header("Life Settings")]
    [SerializeField] private float _maxLife = 100f;
    [SerializeField] private float _currentLife;

    [SerializeField] private AudioClip Damage;
    [SerializeField] private AudioSource _audioSource;

    private bool _isDead = false;

    private void Awake()
    {
        _currentLife = _maxLife;

    }

    private void Update()
    {
        if (!_isDead && _currentLife <= 0)
        {
            DeadExecute();
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _currentLife -= damage;
        _currentLife = Mathf.Clamp(_currentLife, 0, _maxLife);

        Debug.Log("Entró el damage, ahora mi vida es de " + _currentLife);

        if (_audioSource != null)
            _audioSource.PlayOneShot(Damage);
    }

    private void DeadExecute()
    {
        _isDead = true;
        Debug.Log("Enemigo muerto");
        Destroy(gameObject);
    }
}
