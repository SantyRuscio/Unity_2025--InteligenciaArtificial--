using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseState
{
    public Transform[] _wayPoints;
    public Transform _myRoot;

    private int currentWaypoint = 0;
    private Vector3 desired = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 steering = Vector3.zero;

    private float movSpeed = 3f;
    private float steeringForce = 0.1f;
    private float ArrivingDistance = 1f;

    private float _chaseRange = 5f;

    float distance = 0f;

    public override void OnEnter()
    {
        Debug.Log("Entré a Patrol");
        currentWaypoint = 0;
    }

    public override void OnUpdate()      ////recorrido de waypoints mediante Seek + Arrive///
    {
        if (_wayPoints.Length == 0) return;

        Vector3 dir = _wayPoints[currentWaypoint].position - _myRoot.position;
        distance = dir.magnitude;

        // Seek + Arrive
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

        _myRoot.position += velocity * Time.deltaTime
            ;
        _myRoot.forward = velocity;

        // lOOP DE WAYPOINTS 
        if (distance < ArrivingDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % _wayPoints.Length;
        }

        if (Vector3.Distance(Target.Position, _myRoot.position) < _chaseRange)
        {
            Debug.Log("cambiando a chase");
            fsm.ChnageState(EnemyStates.Chase);
        }

    }

    public override void OnExit()
    {
        Debug.Log("Saliendo de Patrol");
    }


    public void SetRoot(Transform newroot)
    {
        _myRoot = newroot;
    }

    public void SetWaypoints(Transform[] newroot)
    {
        _wayPoints = newroot;
    }

}