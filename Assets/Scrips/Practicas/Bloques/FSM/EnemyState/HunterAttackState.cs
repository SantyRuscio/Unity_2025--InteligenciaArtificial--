using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HunterAttackState : BaseState
{
    //Asiganciones
    private Animator _animator;
    private TargetLife _targetLife;

    //Chequeos Para Cambios de Estado
    public float _attackRange = 1f;
    [SerializeField] private float _chaseRange = 6f;

    //Variables del estado
    private float _dmg = 20f;
    private float _attackCooldown = 5f; 
    private float _lastAttackTime = -999f; 

    public HunterAttackState(TargetLife _targetLife)
    {
        this._targetLife = _targetLife;
    }
    public override void OnEnter()
    {
        Debug.Log("Entered AttackState");
    }

    public override void OnUpdate()
    {
        if (_myRoot == null) return;

        Vector3 dir = (Target.Position - _myRoot.position).normalized;

        Vector3 stopBeforeTarget = Target.Position - dir * 1f; 

        float distanceToTarget = Vector3.Distance(_myRoot.position, stopBeforeTarget);

        if (distanceToTarget <= _attackRange)
        {
            Debug.Log("Atacando al jugador");
            AttackCount();

            if (_targetLife._currentLife >= 0)
            {
                fsm.ChnageState(AgentStates.Idle);
            }
        }
        else if (distanceToTarget <= _chaseRange)
        {
            Debug.Log("El jugador se alejó, vuelvo a perseguir");
            fsm.ChnageState(AgentStates.Chase);
        }
    }

    private void AttackCount()
    {
        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            _lastAttackTime = Time.time;
            Debug.Log("Atacando al jugador");

            _targetLife.DamageTaken(_dmg);

        }
    }

}
