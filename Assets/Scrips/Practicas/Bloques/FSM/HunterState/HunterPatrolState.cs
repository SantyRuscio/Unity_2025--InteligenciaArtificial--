using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HunterPatrolState : BaseState
{
    //Asiganciones
    public Transform[] _wayPoints;
    HunterlLife _rivalLife;

    private Boids _currentRivalBoid;

    private Animator _animator; 
    private int currentWaypoint = 0;

    //Steerings Valores
    private float movSpeed = 3f;
    private float steeringForce = 0.01f;
    private float ArrivingDistance = 1f;
    private float CurveSpeed = 6f;

    //Chequeos Para Cambios de Estado
    private float _safeDamage = 50f;
    private float _chaseRange = 5f;

    float distance = 0f;

    public HunterPatrolState(Transform[] _wayPoints, HunterlLife _rivalLife, Animator anim)
    {
        this._wayPoints = _wayPoints;
        this._rivalLife = _rivalLife;
        this._animator = anim;
    }

    public override void OnEnter()
    {
        Debug.Log("Entré a Patrol");
        currentWaypoint = 0;

        if (_myRoot != null)
            _animator = _myRoot.GetComponentInChildren<Animator>();

        if (_animator != null)
            _animator.SetBool("isWalking", true);
    }

    public override void OnUpdate() //// recorrido de waypoints mediante Seek + Arrive ///
    {
        if (_wayPoints.Length == 0) return;
        DetectThing();

        SeekArriveCount();
        wayPointsLoop();

        if (_currentRivalBoid != null)
        {
            // if(_rivalLife._currentLife < _safeDamage)
            // {
            //    Debug.Log("No tengo vida, Cambio a Evade");
            //    fsm.ChnageState(AgentStates.Evade);
            //
            // }

             Debug.Log("cambiando a chase");
             fsm.ChnageState(AgentStates.Chase);
            
        }
        else if (_rivalLife._currentLife < _safeDamage)
        {
        }
    }

    public override void OnExit()
    {
        Debug.Log("Saliendo de Patrol");

        if (_animator != null)
            _animator.SetBool("isWalking", false); 
    }

    private void SeekArriveCount()  // Seek + Arrive Cuentas // 
    {
        Debug.Log("Entré a patrol");
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

        _myRoot.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
            _myRoot.rotation = Quaternion.Slerp(_myRoot.rotation, targetRotation, Time.deltaTime * CurveSpeed);
        }
    }

    private void wayPointsLoop() // WayLoops //
    {
        Debug.Log("Entré a LoopWay");
        if (distance < ArrivingDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % _wayPoints.Length;
        }
    }

    private void DetectThing()
    {
        _currentRivalBoid = BoidsManager.Instance.GetClosestBoid(_myRoot.position);

        if (_currentRivalBoid != null)
        {
            distance = Vector3.Distance(_myRoot.position, _currentRivalBoid.transform.position);

            if (distance >_chaseRange)
            {
                distance = Mathf.Infinity;
                _currentRivalBoid = null;
            }
        }
        else
            distance = Mathf.Infinity;
    }
}