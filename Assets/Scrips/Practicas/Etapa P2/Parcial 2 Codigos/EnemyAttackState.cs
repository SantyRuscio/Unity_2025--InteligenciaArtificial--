using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================
public class EnemyAttackState : BaseEnemyState
{
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float chaseRange = 4f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float nextAttackTime;

    public override void OnEnter()
    {
        base.OnEnter();
        pathFinder.CancelPath();
        Debug.Log("Entró en ATTACK");
    }

    public override void OnUpdate()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > chaseRange)
        {
            fsm.ChangeState(EnemyStateType.Chase);
            return;
        }

        if (!root.CanSeePlayer())
        {
            fsm.ChangeState(EnemyStateType.Alert);
            return;
        }

        //ataque
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }

        Vector3 lookDir = (target.position - transform.position);
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);
    }

    private void Attack()
    {
        Debug.Log("Ataca al jugador!");
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Salió de ATTACK");
    }
}

