using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterChaseState : BaseState
{
    private Animator _animator;

    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 5f;
    [SerializeField] float steeringForce = 0.01f;
    [SerializeField] float ArrivingDistance = 5f;

    private Boids _currentRivalBoid;

    private float _atackRange = 2f;
    private float _chaseRange = 10f; // lo aumenté para pruebas
    private float distance = 0f;

    private Vector3 lastRivalPos;
    private Vector3 rivalVelocity;

    public HunterChaseState(Animator _animator)
    {
        this._animator = _animator;
    }

    public override void OnEnter()
    {
        if (_myRoot != null)
            _animator = _myRoot.GetComponent<Animator>();

        if (_animator != null)
            _animator.SetBool("isWalking", true);

        Debug.Log("Entre a Chase");
    }

    public override void OnUpdate()
    {
        if (_myRoot == null) return;

        DetectThing();
        PursuitCount();

        // Volver a Patrol si target fuera de rango
        if (distance > _chaseRange)
        {
            Debug.Log("Target fuera de rango, volver a Patrol");
            fsm.ChnageState(AgentStates.Patrol);
            return;
        }

        if (distance <= _atackRange)
        {
            Debug.Log("En rango de ataque, cambiar a Attack");
            fsm.ChnageState(AgentStates.Attack);
            return;
        }
    }

    private void PursuitCount()
    {
        if (_currentRivalBoid == null) return;

        Vector3 dir = _currentRivalBoid.transform.position - _myRoot.position;
        distance = dir.magnitude;

        // Seek + Steering
        Vector3 desired = dir.normalized * movSpeed;
        Vector3 steering = Vector3.ClampMagnitude(desired - velocity, steeringForce);
        velocity = Vector3.ClampMagnitude(velocity + steering, movSpeed);

        _myRoot.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.001f)
            _myRoot.forward = velocity.normalized;
    }


    public override void OnExit()
    {
        if (_animator != null)
            _animator.SetBool("isWalking", false);

        Debug.Log("Salí de Chase");
    }

    private void DetectThing()
    {
        _currentRivalBoid = BoidsManager.Instance.GetClosestBoid(_myRoot.position);

        if (_currentRivalBoid != null)
        {
            distance = Vector3.Distance(_myRoot.position, _currentRivalBoid.transform.position);
        }
        else
        {
            _currentRivalBoid = null;
            distance = Mathf.Infinity;
        }
    }

}