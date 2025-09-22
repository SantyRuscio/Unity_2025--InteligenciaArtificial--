using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoidsPickUpState : BaseState
{
    private Animator _animator;

    private float detectRadius = 10f;
    private Transform _currentApple;

    // Steerings Valores
    private float movSpeed = 3f;
    private float steeringForce = 0.3f;
    private float ArrivingDistance = 1.2f;

    // Chequeos Para Cambios de Estado
    private float _applePickUpRange = 5f;

    public BoidsPickUpState() { }

    public override void OnEnter()
    {
        Debug.Log("Prey: Entr� a PreyPickUpState");

        if (_myRoot != null)
            _animator = _myRoot.GetComponentInChildren<Animator>();

        if (_animator != null)
            _animator.SetBool("isWalking", true);
    }

    public override void OnUpdate()
    {
        DetectThings();

        if (_currentApple != null)
        {
            SeekArriveCount();

            float distToApple = Vector3.Distance(_myRoot.position, _currentApple.position);
            if (distToApple <= ArrivingDistance)
            {
                Debug.Log("Prey: Manzana recogida, volvemos a Patrol");
                fsm.ChnageState(AgentStates.Patrol);
            }
        }
        else
        {
            fsm.ChnageState(AgentStates.Patrol);
        }
    }

    public override void OnExit()
    {
        Debug.Log("PRAY : sali de PreyPickUpState");

        if (_animator != null)
            _animator.SetBool("isWalking", false);
    }

    // Seek + Arrive usando la manzana detectada
    private void SeekArriveCount()
    {
        if (_currentApple == null) return;

        Vector3 dir = _currentApple.position - _myRoot.position;
        float distance = dir.magnitude;

        // Seek + Arrive
        Vector3 desired;
        if (distance < ArrivingDistance)
        {
            desired = dir.normalized * movSpeed * (distance / ArrivingDistance);
        }
        else
        {
            desired = dir.normalized * movSpeed;
        }

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, steeringForce);
        velocity = Vector3.ClampMagnitude(velocity + steering, movSpeed);

        _myRoot.position += velocity * Time.deltaTime;

        // Rotaci�n solo si hay movimiento
        if (velocity.sqrMagnitude > 0.001f)
            _myRoot.forward = velocity.normalized;
    }

    private void DetectThings()
    {
        Debug.Log("Prey: Buscando manzanas con AppleManager");

        _currentApple = AppleManager.instance.GetClosestApple(_myRoot.position, detectRadius);

        if (_currentApple != null)
        {
            float dist = Vector3.Distance(_myRoot.position, _currentApple.position);
            if (dist > detectRadius) 
                _currentApple = null;
        }
    }
}

