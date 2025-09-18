using IA.GenericFSM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsPatrolState : BaseState
{
    // Asignaciones
    private Transform[] _wayPoints;
    private BoidsLife _targetLife;
    private Animator _animator;

    // Para Obtener Transforms
    private float detectRadius = 2f;
    private float _attackRange = 1.5f;
    private Hunter _currentRivalHunter;
    private Transform _currentApple;

    // Steerings Valores
    private float movSpeed = 3f;
    private float steeringForce = 2f;
    private float ArrivingDistance = 0.5f;
    private int currentWaypoint = 0;

    // Chequeos Para Cambios de Estado
    private float _safeDamage = 50f;
    private float _applePickUpRange = 5f;

    float distance = 0f;

    public BoidsPatrolState(Transform[] _wayPoints, BoidsLife _targetLife, Animator anim)
    {
        this._wayPoints = _wayPoints;
        this._targetLife = _targetLife;
        this._animator = anim;
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

        // Si hay manzana cerca
        if (_currentApple != null)
        {
            Debug.Log("Prey: Voy a la manzana");
            fsm.ChnageState(AgentStates.PickUp);
            return;
        }

        // Si hay hunter a la vista
        if (_currentRivalHunter != null)
        {
            float distToRival = Vector3.Distance(_myRoot.position, _currentRivalHunter.transform.position);

            if (distToRival > detectRadius)
            {
                // demasiado lejos, lo descarto
                _currentRivalHunter = null;
            }
            else
            {
                // está dentro del rango de detección
                Debug.Log("Prey: ENEMIGO A LA VISTA");

                if (distToRival <= _attackRange)
                {
                    Debug.Log("Prey: Attack");
                    fsm.ChnageState(AgentStates.Attack);
                    return;
                }
                else
                {
                    // si todavía no está lo bastante cerca como para atacar
                    if (_targetLife._currentLife < _safeDamage)
                    {
                        Debug.Log("Prey: Evade");
                        fsm.ChnageState(AgentStates.Evade);
                    }
                    else
                    {
                        // lo ves, pero todavía está lejos: empezá a seguirlo
                        Debug.Log("Prey: Chasing enemy...");
                        fsm.ChnageState(AgentStates.Chase);
                    }
                    return;
                }
            }
        }

        // Si no hay nada patrullo
        SeekArriveCount();
        WayPointsLoop();
    }

    public override void OnExit()
    {
        Debug.Log("Prey: Saliendo de Patrol");

        if (_animator != null)
            _animator.SetBool("isWalking", false);
    }

    private void SeekArriveCount()
    {
        Vector3 dir = _wayPoints[currentWaypoint].position - _myRoot.position;
        distance = dir.magnitude;

        // Seek + Arrive
        Vector3 desired;
        if (distance < ArrivingDistance)
            desired = dir.normalized * movSpeed * (distance / ArrivingDistance);
        else
            desired = dir.normalized * movSpeed;

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, steeringForce);
        velocity = Vector3.ClampMagnitude(velocity + steering, movSpeed);

        _myRoot.position += velocity * Time.deltaTime;

        // Rotación suave
        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
            _myRoot.rotation = Quaternion.Slerp(_myRoot.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void WayPointsLoop()
    {
        if (distance < ArrivingDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % _wayPoints.Length;
        }
    }

    private void DetectThings()
    {
        _currentApple = AppleManager.instance.GetClosestApple(_myRoot.position, _applePickUpRange);

        _currentRivalHunter = HunterManager.Instance.GetClosestHunter(_myRoot.position);

        if (_currentRivalHunter != null)
        {
            float dist = Vector3.Distance(_myRoot.position, _currentRivalHunter.transform.position);
        }
        else
        {
            Debug.Log("No hay hunter cerca");
        }
    }
}

