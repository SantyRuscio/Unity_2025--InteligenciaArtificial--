using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HunterEvadeState : BaseState
{
    //Steerings Valores
    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 3f;
    [SerializeField] float steeringForce = 1f;
    [SerializeField] float ArrivingDistance = 5f;

    private Vector3 lastRivalPos;
    private Vector3 rivalVelocity;

    float distance = 0f;

    private TargetLife _targetLife;
    private LayerMask _detectLayers;
    private Transform _currentRival;
    private float detectRadius = 15f;


    //Chequeos Para Cambios de Estado
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
        Debug.Log("sali de EvadeState");
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
        Collider[] hits = Physics.OverlapSphere(_myRoot.position, detectRadius, _detectLayers);

        float minRivalDist = Mathf.Infinity;
        Transform closestRival = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float dist = Vector3.Distance(_myRoot.position, hit.transform.position);
                if (dist < minRivalDist)
                {
                    minRivalDist = dist;
                    closestRival = hit.transform;
                }
            }
        }

        if (closestRival != null)
        {
            rivalVelocity = (closestRival.position - lastRivalPos) / Time.deltaTime;
            lastRivalPos = closestRival.position;

            _currentRival = closestRival;
        }
    }

}
