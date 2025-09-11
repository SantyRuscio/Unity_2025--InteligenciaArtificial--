using IA.GenericFSM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyPatrolState : BaseState
{
    //Asiganciones
    private Transform[] _wayPoints;
    private TargetLife _targetLife;
    private Animator _animator;

    //Para Obtener Transforms
    private float detectRadius = 10f;
    private LayerMask _detectLayers;
    private Transform _currentRival;
    private Transform _currentApple;

    //Steerings Valores
    private float movSpeed = 3f;
    private float steeringForce = 0.1f;
    private float ArrivingDistance = 1f;
    private int currentWaypoint = 0;

    //Chequeos Para Cambios de Estado
    private float _safeDamage = 50f;
    private float _applePickUpRange = 5f;
    private float _HunterPickUpRange = 5f;

    float distance = 0f;

    public PreyPatrolState(Transform[] _wayPoints, TargetLife _targetLife, Animator anim, LayerMask _detectLayers )
    {
        this._wayPoints = _wayPoints;
        this._targetLife = _targetLife;
        this._animator = anim;
        this._detectLayers = _detectLayers;
    }

    public override void OnEnter()
    {
        Debug.Log("Prey: Entré a Patrol");
        currentWaypoint = 0;

        if (_myRoot != null)
            _animator = _myRoot.GetComponentInChildren<Animator>();

       // if (_animator != null)
       //     _animator.SetBool("isWalking", true);
    }

    public override void OnUpdate()
    {
        DetectThings();

        if (_currentApple != null)  // manzana cerca
        {
            Debug.Log("Prey: Voy a la manzana");
           // fsm.ChnageState(AgentStates.ChaseApple);
            return;
        }

        if (_currentRival != null) // enemigo a la vista
        {
            Debug.Log("Prey: ENEMIGO A LA VISTA");

            if (_targetLife._currentLife < _safeDamage)
            {
                Debug.Log("Prey: No tengo vida Evade");
                fsm.ChnageState(AgentStates.Evade);
            }
            else
            {
                Debug.Log("Prey: Tengo vida Attack");
                fsm.ChnageState(AgentStates.Attack);
            }
            return;
        } 

        // Si no hay nada patrullo
        SeekArriveCount();
        wayPointsLoop();
    }


    public override void OnExit()
    {
        Debug.Log("pray : Saliendo de Patrol");

        if (_animator != null)
            _animator.SetBool("isWalking", false);
    }

    private void SeekArriveCount()  // Seek + Arrive Cuentas // 
    {
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
        Debug.Log("pray : Entré a LoopWay");
        if (distance < ArrivingDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % _wayPoints.Length;
        }
    }

    private void DetectThings()
    {
        Collider[] hits = Physics.OverlapSphere(_myRoot.position, detectRadius, _detectLayers);

        float minRivalDist = Mathf.Infinity;
        float minAppleDist = Mathf.Infinity;

        Transform closestRival = null;
        Transform closestApple = null;

        foreach (Collider hit in hits)
        {
            float dist = Vector3.Distance(_myRoot.position, hit.transform.position);

            // Rival detectado dentro del rango
            if (hit.CompareTag("Rival") && dist < _HunterPickUpRange && dist < minRivalDist)
            {
                minRivalDist = dist;
                closestRival = hit.transform;
            }

            // Apple detectada dentro del rango
            if (hit.CompareTag("Apple") && dist < _applePickUpRange && dist < minAppleDist)
            {
                minAppleDist = dist;
                closestApple = hit.transform;
            }
        }

        _currentRival = closestRival;
        _currentApple = closestApple;
    }

}
