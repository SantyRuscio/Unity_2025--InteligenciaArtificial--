using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderIdleState : BaseLeaderState
{
    public override void OnEnter()
    {
        pathFinder.CancelPath();
    }

    public override void OnUpdate()
    {
        if (root.CanSeeEnemy(out Transform enemy))
        {
            fsm.ChangeState(LeaderStateType.Attack);
            return;
        }
    }


    public override void OnExit()
    {
        base.OnExit();
        Debug.Log($"{root.name} salió de LEADER IDLE");
    }
}


