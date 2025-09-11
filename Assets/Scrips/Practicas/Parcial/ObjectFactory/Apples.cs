using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Apples : MonoBehaviour
{
    private FactoryGeneric<Apples> _factoryGeneric;

    public event Action<Apples> OnReturnedToPool;

    private void Awake()
    {
        // Busca la Factory automáticamente
        _factoryGeneric = FindAnyObjectByType<FactoryGeneric<Apples>>();
    }

    // Se usa para colocar la manzana en cualquier posición
    public void SetPosition(Vector3 newPos)
    {
        transform.position = newPos;
    }

    // Inicializa la manzana y la activa
    public void Initialize(FactoryGeneric<Apples> factory)
    {
        _factoryGeneric = factory;
        gameObject.SetActive(true);
    }

    // Desactiva la manzana al volver al pool
    public void ResetObject()
    {
        gameObject.SetActive(false);
    }

    // Cuando algo entra en su collider
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Apple: Collider detectado con " + other.name);

        if (other.CompareTag("Player"))
        {
            ReturnToPool();
        }
    }

    // Devuelve la manzana al pool y dispara el evento
    private void ReturnToPool()
    {
        if (_factoryGeneric != null)
        {
            _factoryGeneric.ReleaseLevel(this);
            OnReturnedToPool?.Invoke(this);
        }
    }
}
