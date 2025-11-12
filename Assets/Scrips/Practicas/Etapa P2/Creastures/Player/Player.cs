using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================
public class Player : MonoBehaviour
{
    [Header("General PlayerSettings")]
    private Movement movimiento;
    private Vector3 playerMovementInput;

    [SerializeField] private Controller inputManager;
    private View view;

    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private Animator animator;

    [Header("Variables Player")]
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _rotationSpeed = 120f;
    [SerializeField] private float _damage = 20f;

    [Header("Shooting Settings")]
    [SerializeField] private Transform _shootPoint; 
    [SerializeField] private float shootRange = 50f;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;



    private void Awake()
    {
        if (!playerBody)
            playerBody = GetComponent<Rigidbody>();

        movimiento = new Movement()
            .SetPlayerBody(playerBody)
            .SetPlayerSpeed(_speed)
            .SetRotationSpeed(_rotationSpeed);

        view = new View().SetAnimator(animator);

        if (inputManager != null)
        {
            inputManager.OnMove += Move;
            inputManager.OnClick += Shoot;
        }
    }

    private void OnDestroy()
    {
        if (inputManager != null)
        {
            inputManager.OnMove -= Move;
            inputManager.OnClick -= Shoot;
        }
    }

    void Move(float dirHorizontal, float dirVertical)
    {
        playerMovementInput = new Vector3(dirHorizontal, 0f, dirVertical);
        movimiento.MoveTank(playerMovementInput, transform);
    }

    void Shoot()
    {
        if (_shootPoint == null)
        {
            Debug.LogWarning("No hay un Shoot Point asignado!");
            return;
        }

        audioSource.PlayOneShot(shootSound);

        Ray ray = new Ray(_shootPoint.position, _shootPoint.forward);
        Debug.DrawRay(ray.origin, ray.direction * shootRange, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, shootRange, hitLayers))
        {
            Debug.Log($"Disparo impactó en: {hit.collider.name}");
            hit.collider.GetComponent<IDamageable>()?.TakeDamage(_damage);
        }
        else
        {
            Debug.Log("No se golpeó ningún objeto");
        }
    }

    private void OnDrawGizmos()
    {
        if (_shootPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(_shootPoint.position, _shootPoint.forward * shootRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_shootPoint.position, 0.05f);
    }

}

