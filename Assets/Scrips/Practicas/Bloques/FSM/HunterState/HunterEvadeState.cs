using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HunterEvadeState : BaseState
{
    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 3f;
    [SerializeField] float steeringForce = 1f;
    [SerializeField] float ArrivingDistance = 5f;

    private Vector3 lastRivalPos;
    private Vector3 rivalVelocity;

    float distance = 0f;

    private Animator _animator;
    private Boids _currentRivalBoid;
    private Transform _currentRival;
    private float detectRadius = 15f;


    [SerializeField] float EscapeRangeToPatrol = 5f;

    public override void OnEnter()
    {   
        Debug.Log("entre a EvadeState");
    }

    public override void OnUpdate()
    {
        DetectThing();
        EvadeCounts();
        if( distance > EscapeRangeToPatrol)
        {
            Debug.Log("me escape");
            fsm.ChnageState(AgentStates.Patrol);
        }
    }

    public override void OnExit()
    {
        Debug.Log("Saliendo de HunterEvadeState");

        if (_animator != null)
            _animator.SetBool("isWalking", false);
    }

    private void EvadeCounts()
    {
        if (_currentRival == null) return;

        dir = _myRoot.position - (_currentRival.position + rivalVelocity);
        distance = dir.magnitude;

        if (distance < ArrivingDistance)
        {
            desired = dir.normalized * movSpeed * (distance / ArrivingDistance);
        }
        else
        {
            desired = dir.normalized * movSpeed;
        }

        steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, steeringForce);
        velocity = Vector3.ClampMagnitude(velocity + steering, movSpeed);

        _myRoot.position += velocity * Time.deltaTime;
    }

    private void DetectThing()
    {
        _currentRivalBoid = BoidsManager.Instance.GetClosestBoid(_myRoot.position);

        if (_currentRival != null)
        {
            rivalVelocity = (_currentRivalBoid.transform.position - lastRivalPos) / Time.deltaTime;
            lastRivalPos = _currentRivalBoid.transform.position;

            distance = Vector3.Distance(_myRoot.position, _currentRivalBoid.transform.position);
        }
        else
        {
            rivalVelocity = Vector3.zero;
            distance = Mathf.Infinity;
        }
    }
}
