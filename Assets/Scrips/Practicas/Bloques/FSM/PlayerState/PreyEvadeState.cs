using IA.GenericFSM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyEvadeState : BaseState
{
    //Steerings Valores
    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 5f;
    [SerializeField] float steeringForce = 1f;
    [SerializeField] float ArrivingDistance = 5f;
    float distance = 0f;

    //Para Obtener Transforms
    private float detectRadius = 15f;
    private LayerMask _detectLayers;
    private Transform _currentRival;

    //Chequeos Para Cambios de Estado
    [SerializeField] float EscapeRangeToPatrol = 5f;

    public PreyEvadeState(LayerMask _detectLayers)
    {
        this._detectLayers = _detectLayers;
    }

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
        if (_currentRival == null) return; 

        Vector3 rivalPosition = _currentRival.position;

        // Dirección hacia donde evadirnos
        dir = _myRoot.position - rivalPosition;
        distance = dir.magnitude;

        Vector3 desired;
        if (distance < ArrivingDistance)
        {
            desired = dir.normalized * movSpeed * (distance / ArrivingDistance);
        }
        else
        {
            desired = dir.normalized * movSpeed;
        }

        // Steering
        Vector3 steering = desired - velocity;
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
            if (hit.CompareTag("Rival"))
            {
                float dist = Vector3.Distance(_myRoot.position, hit.transform.position);
                if (dist < minRivalDist)
                {
                    minRivalDist = dist;
                    closestRival = hit.transform;
                }
            }
        }
        _currentRival = closestRival;
    }
}
