using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AllyBoidFSM : MonoBehaviour
{
    [Header("Pesos Flocking")]
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;

    [Header("Distancias")]
    public float visionRange = 6f;

    [Header("FOV")]
    public float fovAngle = 120f;   // ← Ángulo de visión

    public float separationDistance = 1.5f;
    public float neighborDistance = 6f;

    [Header("Movimiento")]
    public float maxSpeed = 4f;

    [Header("Referencias")]
    public Transform leader;
    public string enemyTag = "TeamB";
    public LayerMask obstacleMask;
    public float lowHealthThreshold = 30f;

    private AllyBoidFSMController fsm;

    void Awake()
    {
        fsm = new AllyBoidFSMController();

        var follow = new BoidFollowLeaderState().SetUp(fsm, this);
        var attack = new BoidAttackState().SetUp(fsm, this);
        var ret = new BoidReturnState().SetUp(fsm, this);
        var flee = new BoidFleeState().SetUp(fsm, this);

        fsm.possibleStates.Add(BoidStateType.Flee, flee);
        fsm.possibleStates.Add(BoidStateType.FollowLeader, follow);
        fsm.possibleStates.Add(BoidStateType.Attack, attack);
        fsm.possibleStates.Add(BoidStateType.ReturnToFormation, ret);

        fsm.currentState = follow;
    }

    public bool IsLowHealth()
    {
        var health = GetComponent<Health>();
        if (health == null) return false;
        return health.CurrentHealth <= lowHealthThreshold;
    }

    void Start()
    {
        BoidManager.Instance.Register(this);
    }

    void Update()
    {
        fsm.OnUpdate();
    }

    public bool CanSeeEnemy(out Transform enemy)
    {
        enemy = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange);

        foreach (var col in hits)
        {
            if (!col.CompareTag(enemyTag)) continue;

            Vector3 dir = (col.transform.position - transform.position).normalized;
            float dist = Vector3.Distance(transform.position, col.transform.position);

            // 1. Check ángulo
            float angle = Vector3.Angle(transform.forward, dir);
            if (angle > fovAngle * 0.5f)
                continue;

            // 2. Check obstrucción por paredes
            if (Physics.Raycast(transform.position, dir, dist, obstacleMask))
                continue;

            // 3. Lo ve
            enemy = col.transform;
            return true;
        }

        return false;
    }


    void OnDrawGizmosSelected()
    {
        // Rango de visión
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Cono de visión
        Vector3 leftDir = Quaternion.Euler(0, -fovAngle * 0.5f, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, fovAngle * 0.5f, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + leftDir * visionRange);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * visionRange);

        // Línea hacia enemigo detectado (solo jugando)
        if (Application.isPlaying)
        {
            if (CanSeeEnemy(out Transform enemy))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, enemy.position);
            }
        }
    }
}




