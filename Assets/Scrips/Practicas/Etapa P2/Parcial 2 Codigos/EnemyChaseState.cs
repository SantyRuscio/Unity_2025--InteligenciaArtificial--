using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : BaseEnemyState
{
    public override void OnUpdate()
    {
        if (root.CanSeePlayer())
        {
            root.lastSeenPosition = target.position;
            pathFinder.BuscarNuevoCamino(root.lastSeenPosition);
        }
        else
        {
            fsm.ChangeState(EnemyStateType.Alert);
            pathFinder.BuscarNuevoCamino(root.lastSeenPosition);
        }
    }
}

