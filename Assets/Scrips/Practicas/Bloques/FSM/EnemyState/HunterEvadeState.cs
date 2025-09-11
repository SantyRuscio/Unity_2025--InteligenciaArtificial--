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
    float distance = 0f;

    //Chequeos Para Cambios de Estado
    [SerializeField] float EscapeRangeToPatrol = 5f;

    public override void OnEnter()
    {   
        Debug.Log("entre a EvadeState");
    }

    public override void OnUpdate()
    {
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

        dir = _myRoot.position - (Target.Position + Target.Velocity);
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

}
