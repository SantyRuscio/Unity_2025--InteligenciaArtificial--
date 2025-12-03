using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                8f * Time.deltaTime
            );
        }

        if (dist <= attackRange)
        {
            if (Time.time >= nextAttackTime)
            {
                DoAttack();
                nextAttackTime = Time.time + attackCooldown;
            }
            return;
        }


        if (pathFinder != null)
        {
            Vector3 pos = currentTarget.position;

            // Si hay línea de visión → movimiento directo
            if (!Physics.Linecast(root.transform.position, pos, root.obstacleMask))
                pathFinder.SetDirectTarget(pos);

            // Si no hay visión → usar Theta*
            else
                pathFinder.BuscarNuevoCamino(pos);

            fsm.ChangeState(LeaderStateType.MoveToPoint);
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


