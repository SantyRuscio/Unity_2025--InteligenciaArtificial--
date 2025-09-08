using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class RivalLife : MonoBehaviour
{
    [SerializeField] private float _maxLife = 100f;
    public float _currentLife { get; private set; }

    private void Awake()
    {
        _currentLife = _maxLife; // inicia con vida máxima
    }

    public void DamageTaken(float damage)
    {
        Debug.Log("DAMAGE");
        _currentLife = Mathf.Clamp(_currentLife - damage, 0, _maxLife);

        if (_currentLife <= 0)
        {
            Debug.Log("Me muero");
            OnDead();
        }
        else
        {
            Debug.Log("Sigo vivo. Vida actual: " + _currentLife);
        }
    }

    private void OnDead()
    {
        Debug.Log("Ejecuto muerte");
        // acá podés poner animación, desactivar al enemigo, etc.
    }
}

