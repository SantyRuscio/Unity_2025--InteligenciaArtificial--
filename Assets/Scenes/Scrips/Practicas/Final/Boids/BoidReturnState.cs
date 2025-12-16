using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================
public class BoidReturnState : BaseBoidState
{
    public override void OnUpdate()
    {
        if (root.IsLowHealth())
        {
            fsm.ChangeState(BoidStateType.Flee);
            return;
        }

        Vector3 target = root.leader.position - (root.leader.forward * 2f);

        root.transform.position = Vector3.MoveTowards(root.transform.position, target, 4f * Time.deltaTime);

        if (!root.CanSeeEnemy(out _))
            fsm.ChangeState(BoidStateType.FollowLeader);
    }
}

