using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLife : MonoBehaviour
{
    [SerializeField] private float _maxLife = 100f;
    public float _currentLife { get; private set; }
    public bool isLive = true;

    private void Awake()
    {
        _currentLife = _maxLife; // inicia con vida máxima
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("me sacan vida");
            DamageTaken(20f);
        }
    }

    public void DamageTaken(float damage)
    {
        _currentLife = Mathf.Clamp(_currentLife - damage, 0, _maxLife);

        if (_currentLife <= 0)
        {
            Debug.Log("ME muero");
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
        Destroy(gameObject);
    }
}
