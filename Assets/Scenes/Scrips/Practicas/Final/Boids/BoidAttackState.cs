using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAttackState : BaseBoidState
{
    private Transform target;
    private float attackRange = 1.2f;
    private float cooldown = 1f;
    private float nextTime;

    public override void OnUpdate()
    {
        if (!root.CanSeeEnemy(out target))
        {
            fsm.ChangeState(BoidStateType.ReturnToFormation);
            return;
        }

        float dist = Vector3.Distance(root.transform.position, target.position);

        if (dist <= attackRange && Time.time >= nextTime)
        {
            IDamageable dmg = target.GetComponent<IDamageable>();
            if (dmg != null) dmg.TakeDamage(10);
            nextTime = Time.time + cooldown;
        }
        else
        {
            root.transform.position = Vector3.MoveTowards(
                root.transform.position,
                target.position,
                4f * Time.deltaTime
            );
        }
    }
}

