using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rival : MonoBehaviour
{
    [Header("Waypoints para Patrol")]
    [SerializeField] private Transform[] _wayPoints;

    private BloquesFsm fsm;

    void Awake()
    {
        // Crear la FSM
        fsm = new BloquesFsm();

        // Crear estaDOS
        var idle = new IdleState().SetUp(fsm);
        var chase = new ChaseState().SetUp(fsm);
        var attack = new AttackState().SetUp(fsm);
        var patrol = new PatrolState().SetUp(fsm);

        PatrolState a = patrol as PatrolState;
        a.SetRoot(transform);
        a.SetWaypoints(_wayPoints);


        fsm._possibleStates.Add(EnemyStates.Idle, idle);
        fsm._possibleStates.Add(EnemyStates.Chase, chase);
        fsm._possibleStates.Add(EnemyStates.Attack, attack);
        fsm._possibleStates.Add(EnemyStates.Patrol, patrol);

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
