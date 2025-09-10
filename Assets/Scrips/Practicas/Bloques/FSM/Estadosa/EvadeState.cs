using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EvadeState : BaseState
{
    //Steerings Valores
    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 2f;
    [SerializeField] float steeringForce = 0.1f;
    [SerializeField] float ArrivingDistance = 7f;
    float distance = 0f;

    public override void OnEnter()
    {   
        Debug.Log("entre a EvadeState");
    }

    public override void OnUpdate()
    {
        EvadeCounts();
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
