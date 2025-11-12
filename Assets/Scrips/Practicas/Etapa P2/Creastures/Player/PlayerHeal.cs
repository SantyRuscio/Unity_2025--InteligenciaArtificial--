using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================
public class PlayerHeal : MonoBehaviour, IDamageable, IHeal
{
    [Header("Life Settings")]
    [SerializeField] private float _maxLife = 100f;
    [SerializeField] private float _currentLife;

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

        Debug.Log("Entro el damage ahora mi vida es de" + _currentLife);
    }

    private void DeadExecute()
    {
        _isDead = true;
        Debug.Log("Player muerto");
        Destroy(gameObject);
    }

   public void Heal(float amount)
   {
       if (_isDead) return;
  
       _currentLife += amount;
       _currentLife = Mathf.Clamp(_currentLife, 0, _maxLife);
   }
}