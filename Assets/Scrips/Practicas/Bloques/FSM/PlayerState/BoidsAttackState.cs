using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsAttackState : BaseState
{
    //Asignaciones
    private Animator _animator;
    private HunterlLife _rivalLife;

    //Para Obtener Transforms
    private float detectRadius = 10f; // lo subí un poco
    private Hunter _currentRivalHunter;

    //Chequeos Para Cambios de Estado
    public float _attackRange = 0.5f;      // rango para pegar
    [SerializeField] private float _chaseSpeed = 3f; // velocidad de persecución
    [SerializeField] private float _chaseRange = 12f; // hasta dónde persigue

    //Variables del estado
    private float _dmg = 10f;
    private float _attackCooldown = 2f;
    private float _lastAttackTime = -999f;

    public BoidsAttackState(HunterlLife rivalLife)
    {
        this._rivalLife = rivalLife;
    }

    public override void OnEnter()
    {
        Debug.Log("Prey : Entré a AttackState");
    }

    public override void OnUpdate()
    {
        if (_myRoot == null) return;

        DetectThing();

        // Si no hay enemigo -> vuelvo a patrullar
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
                _myRoot.rotation = Quaternion.Slerp(_myRoot.rotation, targetRot, Time.deltaTime * 5f);
            }
        }

        // --- Si está a rango, ataco ---
        if (distanceToTarget <= _attackRange)
        {
            AttackCount();

            if (_rivalLife._currentLife <= 0)
            {
                Debug.Log("Prey : Rival muerto, vuelvo a patrulla");
                fsm.ChnageState(AgentStates.Patrol);
            }
        }
        else if (distanceToTarget > _chaseRange)
        {
            Debug.Log("Prey : Rival se escapó");
            fsm.ChnageState(AgentStates.Patrol);
        }
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