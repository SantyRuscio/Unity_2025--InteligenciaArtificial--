using System.Collections;
using UnityEngine;
public class EnemyFSM : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;
    public PathFinderParcial_Astar pathFinder;

    [Header("Patrulla")]
    public Transform[] patrolPoints;

    [Header("Parámetros de Visión")]
    public float visionRange = 10f;
    public float visionAngle = 60f;
    public LayerMask obstacleMask;

    [Header("Debug")]
    public bool playerVisible = false;
    public Vector3 lastSeenPosition;

    private EnemyFSMController fsm;

    // EVENTOS
    void OnEnable() => AlertManager.OnAlert += ReceiveAlert;
    void OnDisable() => AlertManager.OnAlert -= ReceiveAlert;

    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        fsm = new EnemyFSMController();

        var patrol = new EnemyPatrolState().SetUp(fsm, this);
        var chase = new EnemyChaseState().SetUp(fsm, this);
        var alert = new EnemyAlertState().SetUp(fsm, this);

        fsm.possibleStates.Add(EnemyStateType.Patrol, patrol);
        fsm.possibleStates.Add(EnemyStateType.Chase, chase);
        fsm.possibleStates.Add(EnemyStateType.Alert, alert);

        fsm.currentState = patrol;
        fsm.currentState.OnEnter();

        Debug.Log($"{name} entró al estado inicial: {fsm.currentState.GetType().Name}");
    }

    void Update() => fsm.OnUpdate();

    public bool CanSeePlayer()
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

    void ReceiveAlert(Vector3 position, EnemyFSM source)
    {
        if (source == this) return;
        if (fsm.currentState == fsm.possibleStates[EnemyStateType.Chase]) return;

        lastSeenPosition = position;
        fsm.ChangeState(EnemyStateType.Alert);

        Debug.Log($"{name} recibí alerta, cambio a  chase");

        if (pathFinder != null)
            pathFinder.BuscarNuevoCamino(position);
        else
            Debug.LogWarning($"{name}: PathFinder no asignado en EnemyFSM");
    }



    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = playerVisible ? Color.red : Color.yellow;

        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * visionRange);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * visionRange);

        if (lastSeenPosition != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(lastSeenPosition, 0.2f);
            Gizmos.DrawLine(transform.position, lastSeenPosition);
        }
    }
}