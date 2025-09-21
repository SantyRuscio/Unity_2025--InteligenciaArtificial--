using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    [Header("Waypoints para Patrol")]
    [SerializeField] private Transform[] _wayPoints;
    [SerializeField] HunterlLife _rivalLife;
    [SerializeField] BoidsLife _targetLife;
    [SerializeField] Animator _animator;
    [SerializeField] Transform _rivalTransform;
    [SerializeField] LayerMask _detectLayers;

    private BloquesFsm fsm;
    private Animator animator;

    [SerializeField] float speed = 1.0f;
    private Vector3 velocity;

    public Vector3 Velocity => velocity;

    public BoidsLife CurrentLife
    {
        get { return _targetLife; }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // FSM
        fsm = new BloquesFsm();

        // Estados
        var idle = new BoidsIdle().SetUp(fsm);
        var pickup = new BoidsPickUpState().SetUp(fsm).SetRoot(transform);
        var attack = new BoidsAttackState(_rivalLife, _targetLife).SetUp(fsm).SetRoot(transform);   
        var patrol = new BoidsPatrolState(_wayPoints, _targetLife, _animator).SetUp(fsm).SetRoot(transform);
        var evade = new BoidsEvadeState().SetUp(fsm).SetRoot(transform);
        var flocking = new BoidsFlocking().SetUp(fsm).SetRoot(transform);

        fsm._possibleStates.Add(AgentStates.Idle, idle);
        fsm._possibleStates.Add(AgentStates.PickUp, pickup);
        fsm._possibleStates.Add(AgentStates.Attack, attack);
        fsm._possibleStates.Add(AgentStates.Patrol, patrol);
        fsm._possibleStates.Add(AgentStates.Evade, evade);
        fsm._possibleStates.Add(AgentStates.Flocking, flocking);

        // Estado inicial
        fsm._actualState = idle;
        fsm._actualState.OnEnter();
    }

    private void OnEnable()
    {
        BoidsManager.Instance?.RegisterBoid(this); //NOS REGISTRAMOS AL MANAGER
    }

    private void OnDisable()
    {
        BoidsManager.Instance?.UnregisterBoid(this); //NOS SACAMOS DEL MANAGER
    }

    private void Update()
    {
        fsm.OnUpdate();
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
