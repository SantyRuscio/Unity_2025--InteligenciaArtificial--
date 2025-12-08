using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LeaderAttackState : BaseLeaderState
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.2f;

    private float nextAttackTime;
    private Transform currentTarget;

    public override void OnEnter()
    {
        base.OnEnter();
        currentTarget = null;

        if (pathFinder != null)
            pathFinder.CancelPath();

        Debug.Log($"{root.name} → ENTER ATTACK");
    }

    public override void OnUpdate()
    {
        if (!root.CanSeeEnemy(out Transform enemy))
        {
            fsm.ChangeState(LeaderStateType.Idle);
            return;
        }

        currentTarget = enemy;
        float dist = Vector3.Distance(root.transform.position, currentTarget.position);
        Vector3 lookDir = (currentTarget.position - root.transform.position);
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            root.transform.rotation = Quaternion.Slerp(
                root.transform.rotation,
                Quaternion.LookRotation(lookDir),
                10f * Time.deltaTime
            );
        }


        if (dist <= attackRange)
        {

            if (pathFinder != null) pathFinder.CancelPath(); 

            if (Time.time >= nextAttackTime)
            {
                DoAttack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {

            Vector3 moveDir = (currentTarget.position - root.transform.position).normalized;
            root.transform.position += moveDir * 4f * Time.deltaTime; 
        }
    }

    private void DoAttack()
    {
        if (currentTarget == null) return;

        Debug.Log($"{root.name} ATACA a {currentTarget.name}");

        IDamageable dmg = currentTarget.GetComponent<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(20f);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log($"{root.name} → EXIT ATTACK");
    }
}


