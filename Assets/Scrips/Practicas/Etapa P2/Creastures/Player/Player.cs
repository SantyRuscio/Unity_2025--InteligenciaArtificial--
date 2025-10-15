using UnityEngine;
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
    [SerializeField] private float _damage = 20f;


    [Header("Shooting Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float shootRange = 50f;
    [SerializeField] private LayerMask hitLayers;

    private void Awake()
    {
        movimiento = new Movement()
            .SetPlayerBody(playerBody)
            .SetPlayerSpeed(_speed);

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
        movimiento.Move(playerMovementInput);
    }

    void Shoot()
    {
        if (playerCamera == null)
        {
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, shootRange, hitLayers))
        {
            Debug.Log($"Disparo impactó en: {hit.collider.name}");

            // Si el objeto tiene vida, le hacemos daño
            var life = hit.collider.GetComponent<IDamageable>();

            if (life != null)
            {
                life.TakeDamage(_damage);
            }

        }
        else
        {
            Debug.Log("No se golpeó ningún objeto");
        }
    }
}
