using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HunterAttackState : BaseState
{
    private Animator _animator;
    private Boids _currentRivalBoid; 
    private Transform _currentRivalTransform; 

    // Rango de ataque y persecución
    public float _attackRange = 2f;
    [SerializeField] private float _chaseRange = 5f;

    // Variables de ataque
    private float _dmg = 20f;
    private float _attackCooldown = 2f;
    private float _lastAttackTime = -999f;

    public HunterAttackState() { }

    public override void OnEnter()
    {
        Debug.Log("Entered AttackState");

        if (_myRoot != null)
            _animator = _myRoot.GetComponentInChildren<Animator>();

        if (_animator != null)
            _animator.SetBool("isAttack", true);
    }

    public override void OnUpdate()
    {
        if (_myRoot == null) return;

        DetectThing();

        if (_currentRivalTransform == null) return;

        Vector3 dir = (_currentRivalTransform.position - _myRoot.position).normalized;
        Vector3 stopBeforeTarget = _currentRivalTransform.position - dir * 1f;
        float distanceToTarget = Vector3.Distance(_myRoot.position, stopBeforeTarget);

        if (distanceToTarget <= _attackRange)
        {
            AttackCount();
        }
        else if (distanceToTarget <= _chaseRange)
        {
            Debug.Log("El Boid se alejó, vuelvo a perseguir");
            fsm.ChnageState(AgentStates.Chase);
        }
        else
        {
            _currentRivalBoid = null;
            _currentRivalTransform = null;

            fsm.ChnageState(AgentStates.Idle);
        }
    }

    public override void OnExit()
    {
        if (_animator != null)
            _animator.SetBool("isAttack", false);
    }

    private void AttackCount()
    {
        if (_currentRivalBoid == null) return;

        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            _lastAttackTime = Time.time;

            BoidsLife rivalLife = _currentRivalBoid.CurrentLife;
            if (rivalLife != null)
            {
                rivalLife.DamageTaken(_dmg);
                Debug.Log("Atacando al Boid: " + _currentRivalBoid.name);
            }

            // Limpiar referencia si muere
            if (rivalLife != null && rivalLife._currentLife <= 0)
            {
                _currentRivalBoid = null;
                _currentRivalTransform = null;

                fsm.ChnageState(AgentStates.Idle);
            }
        }
    }


    private void DetectThing()
    {
        _currentRivalBoid = BoidsManager.Instance.GetClosestBoid(_myRoot.position);

        if (_currentRivalBoid != null)
        {
            _currentRivalTransform = _currentRivalBoid.transform;
        }
        else
        {
            _currentRivalTransform = null;
        }
    }
}

