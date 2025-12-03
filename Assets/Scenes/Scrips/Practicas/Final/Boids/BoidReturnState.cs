using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoidReturnState : BaseBoidState
{
    public override void OnUpdate()
    {
        Vector3 target = root.leader.position;
        root.transform.position = Vector3.MoveTowards(root.transform.position, target, 4f * Time.deltaTime);

        if (!root.CanSeeEnemy(out _))
            fsm.ChangeState(BoidStateType.FollowLeader);
    }
}

