using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyAttackState : BaseState
{
    //Asignaciones
    private Animator _animator;
    private RivalLife _rivalLife;

    //Para Obtener Transforms
    private float detectRadius = 15f;
    private LayerMask _detectLayers;
    private Transform _currentRival;

    //Chequeos Para Cambios de Estado
    public float _attackRange = 1f;
    [SerializeField] private float _chaseRange = 6f;

    //Variables del estado
    private float _dmg = 20f;
    private float _attackCooldown = 1.5f;
    private float _lastAttackTime = -999f;

    public PreyAttackState(RivalLife rivalLife, LayerMask detectLayers)
    {
        this._rivalLife = rivalLife;
        this._detectLayers = detectLayers;
    }

    public override void OnEnter()
    {
        Debug.Log("Prey : Entre AttackState");
    }

    public override void OnUpdate()
    {
        if (_myRoot == null) return;

        DetectThing();

        // vuelvo a patrullar
        if (_currentRival == null)
        {
            fsm.ChnageState(AgentStates.Patrol);
            return;
        }

        // Calcular dirección y distancia al rival
        Vector3 dir = (_currentRival.position - _myRoot.position).normalized;
        Vector3 stopBeforeTarget = _currentRival.position - dir * 1f;
        float distanceToTarget = Vector3.Distance(_myRoot.position, stopBeforeTarget);

        //rango de ataque
        if (distanceToTarget <= _attackRange)
        {
            AttackCount();

            // Si el rival muere → patrulla
            if (_rivalLife._currentLife <= 0)
            {
                Debug.Log("Prey : Rival muerto, vuelvo a patrulla");
                fsm.ChnageState(AgentStates.Patrol);
            }
        }
        else if (distanceToTarget > _chaseRange)
        {
            // Si se escapa Patrol
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
        Collider[] hits = Physics.OverlapSphere(_myRoot.position, detectRadius, _detectLayers);

        float minRivalDist = Mathf.Infinity;
        Transform closestRival = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Rival"))
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

