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
    private LayerMask _detectLayers;
    private Transform _currentRival;

    //Chequeos Para Cambios de Estado
    public float _attackRange = 4f;
    [SerializeField] private float _chaseRange = 6f;
    private float detectRadius = 15f;

    //Variables del estado
    private float _dmg = 20f;
    private float _attackCooldown = 5f; 
    private float _lastAttackTime = -999f; 

    public HunterAttackState(TargetLife _targetLife, LayerMask _detectLayers)
    {
        this._targetLife = _targetLife;
        this._detectLayers = _detectLayers;
    }
    public override void OnEnter()
    {
        Debug.Log("Entered AttackState");
    }

    public override void OnUpdate()
    {
        if (_myRoot == null) return;

        DetectThing();

        Vector3 dir = (_currentRival.position - _myRoot.position).normalized;

        Vector3 stopBeforeTarget = _currentRival.position - dir * 1f; 

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

    private void DetectThing()
    {
        Collider[] hits = Physics.OverlapSphere(_myRoot.position, detectRadius, _detectLayers);

        float minRivalDist = Mathf.Infinity;
        Transform closestRival = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float dist = Vector3.Distance(_myRoot.position, hit.transform.position);
                if (dist < minRivalDist)
                {
                    minRivalDist = dist;
                    closestRival = hit.transform;
                }
            }
        }

        _currentRival = closestRival;
    }

}
