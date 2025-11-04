using System.Collections;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Alert }
    public EnemyState currentState = EnemyState.Patrol;

    [Header("Referencias")]
    public Transform target; 
    public PathFinderParcial_Astar pathFinder;

    [Header("Patrulla")]
    public Transform[] patrolPoints;
    private int patrolIndex = 0;

    [Header("Parámetros de Visión")]
    public float visionRange = 10f;
    public float visionAngle = 60f;
    public LayerMask obstacleMask;

    [Header("Debug")]
    public bool playerVisible = false;

    private Vector3 lastSeenPosition;

    // ===============================================================
    // EVENTOS DE ALERTA (suscribir / desuscribir)
    // ===============================================================
    void OnEnable() => AlertManager.OnAlert += ReceiveAlert;
    void OnDisable() => AlertManager.OnAlert -= ReceiveAlert;

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        GoToNextPatrolPoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                PatrolBehaviour();
                break;

            case EnemyState.Chase:
                ChaseBehaviour();
                break;

            case EnemyState.Alert:
                AlertBehaviour();
                break;
        }
    }

    // ===============================================================
    // MÉTODOS DE ESTADO
    // ===============================================================

    void PatrolBehaviour()
    {
        if (CanSeePlayer())
        {
            currentState = EnemyState.Chase;
            pathFinder.BuscarNuevoCamino(target.position);
            return;
        }

        if (!pathFinder.IsMoving)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            GoToNextPatrolPoint();
        }
    }

    void ChaseBehaviour()
    {
        if (CanSeePlayer())
        {
            lastSeenPosition = target.position;
            pathFinder.BuscarNuevoCamino(lastSeenPosition);
        }
        else
        {

            currentState = EnemyState.Alert;
            pathFinder.BuscarNuevoCamino(lastSeenPosition);
        }
    }

    void AlertBehaviour()
    {
        if (CanSeePlayer())
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (!pathFinder.IsMoving)
        {
            currentState = EnemyState.Patrol;
            GoToNextPatrolPoint();
        }
    }

    // ===============================================================
    // ALERTA GLOBAL (EVENTO)
    // ===============================================================

    void ReceiveAlert(Vector3 position, EnemyFSM source)
    {
        if (source == this) return; 
        if (currentState == EnemyState.Chase) return; 

        lastSeenPosition = position;
        currentState = EnemyState.Alert;
        pathFinder.BuscarNuevoCamino(position);
    }

    // ===============================================================
    // VISIÓN / LINE OF SIGHT
    // ===============================================================

    bool CanSeePlayer()
    {
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float distToTarget = Vector3.Distance(transform.position, target.position);

        playerVisible = false;

        if (distToTarget > visionRange) return false;
        if (Vector3.Angle(transform.forward, dirToTarget) > visionAngle) return false;

        if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
        {
            playerVisible = true;
            lastSeenPosition = target.position;
            AlertManager.SendAlert(lastSeenPosition, this);
            return true;
        }

        return false;
    }

    // ===============================================================
    // PATRULLA INTELIGENTE: SOLO A* SI NO TENGO LOS AL WAYPOINT
    // ===============================================================

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        Transform next = patrolPoints[patrolIndex];

        Vector3 dir = (next.position - transform.position);
        if (!Physics.Raycast(transform.position, dir.normalized, dir.magnitude, obstacleMask))
        {
            pathFinder.CancelPath();
            pathFinder.SetDirectTarget(next.position);
        }
        else
        {
            pathFinder.BuscarNuevoCamino(next.position);
        }
    }
}
