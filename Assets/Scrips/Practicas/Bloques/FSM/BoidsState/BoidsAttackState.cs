using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsAttackState : BaseState
{
    private Animator _animator;
    private HunterlLife _rivalLife;
    private BoidsLife _targetLife;

    private float detectRadius = 10f;
    private Hunter _currentRivalHunter;

    public float _attackRange = 0.5f;
    [SerializeField] private float _chaseSpeed = 3f;
    [SerializeField] private float _chaseRange = 12f;
    private float _safeDamage = 50f;

    private float _dmg = 6f;
    private float _attackCooldown = 2f;
    private float _lastAttackTime = -999f;

    private float _topVertical = 1.3f;

    public BoidsAttackState(HunterlLife rivalLife, BoidsLife _targetLife)
    {
        this._rivalLife = rivalLife;
        this._targetLife = _targetLife;
    }

    public override void OnEnter()
    {
        Debug.Log("Prey : Entré a AttackState");

        if (_myRoot != null)
            _animator = _myRoot.GetComponentInChildren<Animator>();

        if (_animator != null)
            _animator.SetBool("isAttack", true);
    }

    public override void OnUpdate()
    {
        if (_myRoot == null) return;

        DetectThing();

        if (_currentRivalHunter == null)
        {
            fsm.ChnageState(AgentStates.Patrol);
            return;
        }

        Vector3 dir = (_currentRivalHunter.transform.position - _myRoot.position).normalized;
        float distanceToTarget = Vector3.Distance(_myRoot.position, _currentRivalHunter.transform.position);

        if (distanceToTarget > _attackRange)
        {
            _myRoot.position += dir * Time.deltaTime * _chaseSpeed;

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                _myRoot.rotation = Quaternion.Slerp(_myRoot.rotation, targetRot, Time.deltaTime * _topVertical);
            }
        }

        if (distanceToTarget <= _attackRange)
        {
            AttackCount();

            if (_rivalLife._currentLife <= 0)
            {
                Debug.Log("Prey : Rival muerto, vuelvo a patrulla");
                fsm.ChnageState(AgentStates.Patrol);
            }
            else if (_targetLife._currentLife < _safeDamage)
            {
                Debug.Log("Prey: Evade");
                fsm.ChnageState(AgentStates.Evade);
                return;
            }
        }
    }

    public override void OnExit()
    {
        Debug.Log("Prey Saliendo de BoidsAttackState");

        if (_animator != null)
            _animator.SetBool("isAttack", false);
    }

    private void AttackCount()
    {
        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            _lastAttackTime = Time.time;
            Debug.Log("Prey : Atacando al jugador");
            _rivalLife.DamageTaken(_dmg);
        }
    }

    private void DetectThing()
    {
        _currentRivalHunter = HunterManager.Instance.GetClosestHunter(_myRoot.position);

        if (_currentRivalHunter != null)
        {
            if (Vector3.Distance(_myRoot.position, _currentRivalHunter.transform.position) > detectRadius)
            {
                _currentRivalHunter = null;
            }
        }
    }
}
