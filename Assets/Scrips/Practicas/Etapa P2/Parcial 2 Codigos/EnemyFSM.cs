using System.Collections;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Alert }
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Referencias")]
    public PathFinderParcial_Astar pathFinder;
    public Transform player;
    public Transform[] patrolPoints;

    private int patrolIndex = 0;
    private float checkInterval = 0.3f;
    private Vector3 lastSeenPosition;

    [Header("FOV (Field of View)")]
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [Header("Colores Debug")]
    public Color idleColor = Color.yellow;
    public Color chaseColor = Color.red;
    public Color alertColor = Color.cyan;

    private bool playerVisible = false;

    void Start()
    {
        if (pathFinder == null)
            pathFinder = GetComponent<PathFinderParcial_Astar>();

        StartCoroutine(StateMachine());
    }

    IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case EnemyState.Patrol:
                    PatrolState();
                    break;
                case EnemyState.Chase:
                    ChaseState();
                    break;
                case EnemyState.Alert:
                    AlertState();
                    break;
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    // =========================
    // ESTADOS FSM
    // =========================

    void PatrolState()
    {
        if (CanSeePlayer())
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (patrolPoints.Length == 0) return;

        if (!pathFinder) return;

        NodeParcial_Astar start = pathFinder.GetClosestNode(transform.position);
        NodeParcial_Astar goal = pathFinder.GetClosestNode(patrolPoints[patrolIndex].position);

        if (start && goal)
        {
            pathFinder.BuscarNuevoCamino(patrolPoints[patrolIndex].position);
        }

        if (Vector3.Distance(transform.position, patrolPoints[patrolIndex].position) < 1f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }
    }

    void ChaseState()
    {
        if (CanSeePlayer())
        {
            lastSeenPosition = player.position;
            NodeParcial_Astar start = pathFinder.GetClosestNode(transform.position);
            NodeParcial_Astar goal = pathFinder.GetClosestNode(player.position);
            if (start && goal)
                pathFinder.BuscarNuevoCamino(player.position);
        }
        else
        {
            currentState = EnemyState.Alert;
        }
    }

    void AlertState()
    {
        if (CanSeePlayer())
        {
            currentState = EnemyState.Chase;
            return;
        }

        NodeParcial_Astar start = pathFinder.GetClosestNode(transform.position);
        NodeParcial_Astar goal = pathFinder.GetClosestNode(lastSeenPosition);
        if (start && goal)
            pathFinder.BuscarNuevoCamino(lastSeenPosition);

        if (Vector3.Distance(transform.position, lastSeenPosition) < 1f)
        {
            currentState = EnemyState.Patrol;
        }
    }

    // =========================
    // DETECCIÓN DE JUGADOR
    // =========================

    bool CanSeePlayer()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        if (rangeChecks.Length > 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    playerVisible = true;
                    return true;
                }
            }
        }

        playerVisible = false;
        return false;
    }

    // =========================
    // DEBUG VISUAL (GIZMOS)
    // =========================

    private void OnDrawGizmosSelected()
    {
        // Color del estado actual
        switch (currentState)
        {
            case EnemyState.Patrol: Gizmos.color = idleColor; break;
            case EnemyState.Chase: Gizmos.color = chaseColor; break;
            case EnemyState.Alert: Gizmos.color = alertColor; break;
        }

        // Dibujar rango
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // Dibujar cono FOV
        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2, false);
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);

        if (playerVisible)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    // Convierte ángulo a dirección en el mundo
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
