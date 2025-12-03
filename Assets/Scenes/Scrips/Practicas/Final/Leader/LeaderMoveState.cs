using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderMoveState : BaseLeaderState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log($"{root.name} entró en LEADER MOVE");
    }

    public override void OnUpdate()
    {
        if (root.CanSeeEnemy(out Transform enemy))
        {
            fsm.ChangeState(LeaderStateType.Attack);
            return;
        }

        if (!pathFinder.IsMoving)
        {
            fsm.ChangeState(LeaderStateType.Idle);
            return;
        }

        Vector3 dir = pathFinder.TargetDirection;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 8f
            );
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log($"{root.name} salió de LEADER MOVE");
    }
}


