using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Waypoints para Patrol")]
    [SerializeField] private Transform[] _wayPoints;
    [SerializeField] RivalLife _rivalLife;
    [SerializeField] TargetLife _targetLife;
    [SerializeField] Animator _animator;
    [SerializeField] Transform _rivalTransform;
    [SerializeField] LayerMask _detectLayers;

    private BloquesFsm fsm;

    private Animator animator;
    public static Vector3 Position { get => instance.transform.position; }
    [SerializeField] float speed = 1.0f;

    Vector3 velocity;

    public static Vector3 Velocity
    {
        get 
        {
            return instance.velocity;
        }
    }

    public static Target instance;

    private void Awake()
    {
        if(instance == null) instance = this;

        animator = GetComponent<Animator>();

        // Crear la FSM
        fsm = new BloquesFsm();

        // Crear estaDOS
        var idle = new PreyIdleState().SetUp(fsm);
        var pickup = new PreyPickUpState(_detectLayers).SetUp(fsm).SetRoot(transform);
        var attack = new PreyAttackState(_rivalLife, _detectLayers).SetUp(fsm).SetRoot(transform);
        var patrol = new PreyPatrolState(_wayPoints, _targetLife, _animator, _detectLayers).SetUp(fsm).SetRoot(transform);
        var evade = new PreyEvadeState(_detectLayers).SetUp(fsm).SetRoot(transform);

        fsm._possibleStates.Add(AgentStates.Idle, idle);
        fsm._possibleStates.Add(AgentStates.PickUp, pickup);
        fsm._possibleStates.Add(AgentStates.Attack, attack);
        fsm._possibleStates.Add(AgentStates.Patrol, patrol);
        fsm._possibleStates.Add(AgentStates.Evade, evade);

        // Estado inicial
        fsm._actualState = idle;
        fsm._actualState.OnEnter();
    }

    Vector3 dir = Vector3.zero;
    Vector3 adjust = Vector3.zero;

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
