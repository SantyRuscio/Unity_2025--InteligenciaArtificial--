using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlertState : BaseEnemyState
{
    public override void OnUpdate()
    {
        if (root.CanSeePlayer())
        {
            fsm.ChangeState(EnemyStateType.Chase);
            return;
        }

        if (!pathFinder.IsMoving)
        {
            fsm.ChangeState(EnemyStateType.Patrol);
        }
    }
}

