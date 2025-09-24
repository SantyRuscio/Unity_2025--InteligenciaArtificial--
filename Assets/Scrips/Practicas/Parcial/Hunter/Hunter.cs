using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Hunter : MonoBehaviour
{
    [Header("Waypoints para Patrol")]
    [SerializeField] private Transform[] _wayPoints;
    [SerializeField] HunterlLife _myLife;
    [SerializeField] Animator _animator;
    [SerializeField] LayerMask _layerMask;

    private BloquesFsm fsm;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // Crear la FSM
        fsm = new BloquesFsm();

        // Crear estaDOS
        var idle = new HunterIdleState(_myLife, _wayPoints).SetUp(fsm).SetRoot(transform);
        var chase = new HunterChaseState(_animator).SetUp(fsm).SetRoot(transform);
        var attack = new HunterAttackState(_myLife).SetUp(fsm).SetRoot(transform);
        var patrol = new HunterPatrolState(_wayPoints, _myLife, _animator).SetUp(fsm).SetRoot(transform);
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
    private void OnEnable()
    {
        HunterManager.Instance?.RegisterHunter(this); //NOS REGISTRAMOS AL MANAGER
    }

    private void OnDisable()
    {
        HunterManager.Instance?.UnregisterHunter(this); //NOS SACAMOS DEL MANAGER
    }

    private void OnDestroy()
    {
        OnDisable();
    }

    public void ChangeState(AgentStates newState)
    {
        fsm.ChnageState(newState); 
    }

}
