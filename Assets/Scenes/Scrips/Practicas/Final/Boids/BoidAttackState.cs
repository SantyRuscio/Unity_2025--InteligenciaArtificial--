using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAttackState : BaseBoidState
{
    private Transform target;
    private float attackRange = 2.5f;
    private float cooldown = 1f;
    private float nextTime;

    public override void OnUpdate()
    {
        if (root.IsLowHealth())
        {
            fsm.ChangeState(BoidStateType.Flee);
            return;
        }

        if (target == null || target.gameObject == null)
        {
            fsm.ChangeState(BoidStateType.ReturnToFormation);
            return;
        }

        if (!root.CanSeeEnemy(out Transform t))
        {
            fsm.ChangeState(BoidStateType.ReturnToFormation);
            return;
        }
        target = t;

        float dist = Vector3.Distance(root.transform.position, target.position);

        if (dist <= attackRange)
        {
            Vector3 lookDir = target.position - root.transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                root.transform.rotation = Quaternion.Slerp(root.transform.rotation, Quaternion.LookRotation(lookDir), 10f * Time.deltaTime);

            if (Time.time >= nextTime)
            {
                Health vida = target.GetComponent<Health>();
                if (vida != null) vida.TakeDamage(10);
                nextTime = Time.time + cooldown;
            }
            if (dist < 1.5f)
            {
                Vector3 pushBack = (root.transform.position - target.position).normalized;
                pushBack.y = 0;
                root.transform.position += pushBack * 2f * Time.deltaTime;
            }
        }
        else
        {
            Vector3 dirToEnemy = (target.position - root.transform.position).normalized;
            Vector3 separation = GetSeparationVector();

            Vector3 finalDir = (dirToEnemy * 2f) + (separation * 5f);

            finalDir.y = 0; 

            if (finalDir != Vector3.zero)
            {
                root.transform.rotation = Quaternion.Slerp(
                    root.transform.rotation,
                    Quaternion.LookRotation(finalDir),
                    10f * Time.deltaTime
                );
                root.transform.position += finalDir.normalized * 4f * Time.deltaTime;
            }
        }
    }

    private Vector3 GetSeparationVector()
    {
        Vector3 separation = Vector3.zero;
        Collider[] neighbors = Physics.OverlapSphere(root.transform.position, 1.5f);

        foreach (var col in neighbors)
        {
            if (col.transform != root.transform && col.CompareTag(root.tag))
            {
                Vector3 push = root.transform.position - col.transform.position;
                push.y = 0; 
                separation += push.normalized;
            }
        }
        return separation.normalized;
    }
}

