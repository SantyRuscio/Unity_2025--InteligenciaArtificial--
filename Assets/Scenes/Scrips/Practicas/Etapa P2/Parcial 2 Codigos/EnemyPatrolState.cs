using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================
public class EnemyPatrolState : BaseEnemyState
{
    private int patrolIndex = 0;

    public override void OnEnter()
    {
        GoToNextPatrolPoint();
    }

    public override void OnUpdate()
    {
        if (root.CanSeePlayer())
        {
            fsm.ChangeState(EnemyStateType.Chase);
            pathFinder.BuscarNuevoCamino(target.position);
            return;
        }

        if (!pathFinder.IsMoving)
        {
            patrolIndex = (patrolIndex + 1) % root.patrolPoints.Length;
            GoToNextPatrolPoint();
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (root.patrolPoints.Length == 0) return;

        Transform next = root.patrolPoints[patrolIndex];
        Vector3 dir = (next.position - transform.position);

        if (!Physics.Raycast(transform.position, dir.normalized, dir.magnitude, root.obstacleMask))
        {
            pathFinder.CancelPath();
            pathFinder.SetDirectTarget(next.position);
        }
        else
        {
            pathFinder.BuscarNuevoCamino(next.position);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Salió de PATROL");
    }
}

