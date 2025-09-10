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
        var idle = new IdleState().SetUp(fsm);
        var chase = new ChaseState(_animator).SetUp(fsm).SetRoot(transform);
        var attack = new AttackState(_targetLife).SetUp(fsm).SetRoot(transform);
        var patrol = new PatrolState(_wayPoints, _rivalLife, _animator).SetUp(fsm).SetRoot(transform);
        var evade = new EvadeState().SetUp(fsm).SetRoot(transform);

        fsm._possibleStates.Add(EnemyStates.Idle, idle);
        fsm._possibleStates.Add(EnemyStates.Chase, chase);
        fsm._possibleStates.Add(EnemyStates.Attack, attack);
        fsm._possibleStates.Add(EnemyStates.Patrol, patrol);
        fsm._possibleStates.Add(EnemyStates.Evade, evade);

        // Estado inicial
        fsm._actualState = idle;
        fsm._actualState.OnEnter();
    }

    void Update()
    {
        // Delegar update a la FSM
        fsm.OnUpdate();
    }

    public void ChangeState(EnemyStates newState)
    {
        fsm.ChnageState(newState); 
    }

}
