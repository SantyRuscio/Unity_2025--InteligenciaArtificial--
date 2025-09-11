using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rival : MonoBehaviour
{
    [Header("Waypoints para Patrol")]
    [SerializeField] private Transform[] _wayPoints;
    [SerializeField] RivalLife _rivalLife;
    [SerializeField] TargetLife _targetLife;
    [SerializeField] Animator _animator;

    private BloquesFsm fsm;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // Crear la FSM
        fsm = new BloquesFsm();

        // Crear estaDOS
        var idle = new HunterIdleState().SetUp(fsm);
        var chase = new HunterChaseState(_animator).SetUp(fsm).SetRoot(transform);
        var attack = new HunterAttackState(_targetLife).SetUp(fsm).SetRoot(transform);
        var patrol = new HunterPatrolState(_wayPoints, _rivalLife, _animator).SetUp(fsm).SetRoot(transform);
        var evade = new HunterEvadeState().SetUp(fsm).SetRoot(transform);

        fsm._possibleStates.Add(AgentStates.Idle, idle);
        fsm._possibleStates.Add(AgentStates.Chase, chase);
        fsm._possibleStates.Add(AgentStates.Attack, attack);
        fsm._possibleStates.Add(AgentStates.Patrol, patrol);
        fsm._possibleStates.Add(AgentStates.Evade, evade);

        // Estado inicial
        fsm._actualState = idle;
        fsm._actualState.OnEnter();
    }

    void Update()
    {
        // Delegar update a la FSM
        fsm.OnUpdate();
    }

    public void ChangeState(AgentStates newState)
    {
        fsm.ChnageState(newState); 
    }

}
