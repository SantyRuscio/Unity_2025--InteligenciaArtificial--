using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================
public class LeaderFSM : MonoBehaviour
{
    [Header("Referencias")]
    public PathFinderParcial_ThetaStar pathFinder;

    [Header("Input")]
    public LayerMask groundMask;
    public Camera mainCamera;

    [Header("Visión")]
    public float visionRange = 12f;
    public float visionAngle = 90f;
    public LayerMask obstacleMask;

    [Header("Debug")]
    public bool enemyVisible = false;
    public Vector3 lastSeenEnemyPos;

    private LeaderFSMController fsm;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        fsm = new LeaderFSMController();

        var idle = new LeaderIdleState().SetUp(fsm, this);
        var move = new LeaderMoveState().SetUp(fsm, this);
        var attack = new LeaderAttackState().SetUp(fsm, this);

        fsm.possibleStates.Add(LeaderStateType.Idle, idle);
        fsm.possibleStates.Add(LeaderStateType.MoveToPoint, move);
        fsm.possibleStates.Add(LeaderStateType.Attack, attack);

        fsm.currentState = idle;
        fsm.currentState.OnEnter();

        if (pathFinder == null)
            pathFinder = GetComponent<PathFinderParcial_ThetaStar>();
    }

    void Update()
    {
        HandleInput();
        fsm.OnUpdate();
    }

    private void HandleInput()
    {
        if (mainCamera == null) return;

        if (Input.GetMouseButtonDown(0)) // click izquierdo
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, groundMask))
            {
                Vector3 clickPos = hit.point;

                if (pathFinder != null)
                {
                    // Movimiento directo si no hay obstáculo
                    if (!Physics.Linecast(transform.position, clickPos, obstacleMask))
                        pathFinder.SetDirectTarget(clickPos);
                    else
                        pathFinder.BuscarNuevoCamino(clickPos);

                    fsm.ChangeState(LeaderStateType.MoveToPoint);
                }
            }
        }
    }




    //VISIÓN
    public bool CanSeeEnemy(out Transform enemyTransform)
    {
        enemyTransform = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange);
        foreach (var col in hits)
        {
            if (col == null) continue;

            // Ignorar si es del MISMO TEAM
            if (col.CompareTag(this.tag)) continue;

            // Solo detectar teamA y teamB
            if (!(col.CompareTag("teamA") || col.CompareTag("teamB"))) continue;

            Vector3 dirTo = (col.transform.position - transform.position).normalized;
            float dist = Vector3.Distance(transform.position, col.transform.position);

            // Fuera del ángulo de visión
            if (Vector3.Angle(transform.forward, dirTo) > visionAngle / 2f)
                continue;

            // Línea de visión limpia
            if (!Physics.Raycast(transform.position, dirTo, dist, obstacleMask))
            {
                enemyVisible = true;
                lastSeenEnemyPos = col.transform.position;
                enemyTransform = col.transform;
                return true;
            }
        }

        enemyVisible = false;
        return false;
    }


    // DEBUG
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = enemyVisible ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * visionRange);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * visionRange);

        if (lastSeenEnemyPos != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(lastSeenEnemyPos, 0.2f);
            Gizmos.DrawLine(transform.position, lastSeenEnemyPos);
        }
    }
}





