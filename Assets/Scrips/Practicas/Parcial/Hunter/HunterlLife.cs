using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;

public class HunterlLife : MonoBehaviour
{
    [SerializeField] private float _maxLife = 100f;
    public float _currentLife { get; private set; }

    [SerializeField] private Image fillImage; 

    private void Awake()
    {
        _currentLife = _maxLife; 
        UpdateHealthBar();       
    }

    public void DamageTaken(float damage)
    {
        Debug.Log("DAMAGE");
        _currentLife = Mathf.Clamp(_currentLife - damage, 0, _maxLife);

        UpdateHealthBar(); 

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

    private void UpdateHealthBar()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = _currentLife / _maxLife;
        }
    }

    private void OnDead()
    {
        Destroy(gameObject);
    }
}

