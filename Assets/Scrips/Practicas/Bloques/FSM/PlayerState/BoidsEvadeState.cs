using IA.GenericFSM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsEvadeState : BaseState
{
    // Steerings Valores
    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 5f;
    [SerializeField] float steeringForce = 1f;
    [SerializeField] float ArrivingDistance = 5f;
    float distance = 0f;

    // Para Obtener Transforms
    private float detectRadius = 7f;
    private Hunter _currentRivalHunter;

    // Chequeos Para Cambios de Estado
    [SerializeField] float EscapeRangeToPatrol = 5f;

    [SerializeField] private Vector3 minBounds = new Vector3(-12f, 0f, -12f);
    [SerializeField] private Vector3 maxBounds = new Vector3(12f, 0f, 12f);

    public override void OnEnter()
    {
        Debug.Log("PRAY : entre a EvadeState");
    }

    public override void OnUpdate()
    {
        DetectThing();
        EvadeCounts();

        if (distance > EscapeRangeToPatrol)
        {
            Debug.Log("PRAY me escape");
            fsm.ChnageState(AgentStates.Patrol);
        }
    }

    public override void OnExit()
    {
        Debug.Log("PRAY : sali de EvadeState");
    }

    private void EvadeCounts() //EVADE
    {
        if (_currentRivalHunter == null) return;

        Vector3 rivalPosition = _currentRivalHunter.transform.position;

        dir = _myRoot.position - rivalPosition;
        distance = dir.magnitude;

        Vector3 desired;
        if (distance < ArrivingDistance)
            desired = dir.normalized * movSpeed * (distance / ArrivingDistance);
        else
            desired = dir.normalized * movSpeed;


        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, steeringForce);

        velocity = Vector3.ClampMagnitude(velocity + steering, movSpeed);


        _myRoot.position += velocity * Time.deltaTime;

        _myRoot.position = new Vector3(
            Mathf.Clamp(_myRoot.position.x, minBounds.x, maxBounds.x),
            _myRoot.position.y,
            Mathf.Clamp(_myRoot.position.z, minBounds.z, maxBounds.z)
        );
    }

    private void DetectThing()
    {
        _currentRivalHunter = HunterManager.Instance.GetClosestHunter(_myRoot.position);

        if (_currentRivalHunter != null)
        {
            distance = Vector3.Distance(_myRoot.position, _currentRivalHunter.transform.position);
            if (distance > detectRadius)
            {
                _currentRivalHunter = null;
            }
        }
    }
}

