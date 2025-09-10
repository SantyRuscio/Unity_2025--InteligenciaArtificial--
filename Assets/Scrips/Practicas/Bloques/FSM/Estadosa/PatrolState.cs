using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PatrolState : BaseState
{
    //Asiganciones
    public Transform[] _wayPoints;
    RivalLife _rivalLife;

    private Animator _animator; 
    private int currentWaypoint = 0;

    //Steerings Valores
    private float movSpeed = 3f;
    private float steeringForce = 0.1f;
    private float ArrivingDistance = 1f;

    //Chequeos Para Cambios de Estado
    private float _safeDamage = 50f;
    private float _chaseRange = 5f;

    float distance = 0f;

    public PatrolState(Transform[] _wayPoints, RivalLife _rivalLife, Animator anim)
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

        SeekArriveCount();
        wayPointsLoop();

        if (Vector3.Distance(Target.Position, _myRoot.position) < _chaseRange && _rivalLife._currentLife > _safeDamage)
        {
            Debug.Log("cambiando a chase");
            fsm.ChnageState(EnemyStates.Chase);
        }
       else if (_rivalLife._currentLife < _safeDamage)
       {
           Debug.Log("No tengo vida, Cambio a Evade");
           fsm.ChnageState(EnemyStates.Evade); 
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

        // Rotación solo si hay movimiento, igual que en Chase
        if (velocity.sqrMagnitude > 0.001f)
            _myRoot.forward = velocity.normalized;
    }

    private void wayPointsLoop() // WayLoops //
    {
        Debug.Log("Entré a LoopWay");
        if (distance < ArrivingDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % _wayPoints.Length;
        }
    }
}