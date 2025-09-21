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
    private float _attackRange = 4f;
    private Hunter _currentRivalHunter;
    private Transform _currentApple;

    // Steerings Valores
    private float movSpeed = 3f;
    private float steeringForce = 2f;
    private float ArrivingDistance = 1.5f;
    private int currentWaypoint = 0;

    // Flocking
    private float _flockingRadius = 5f;   // radio de percepción
    private float _flockingForce = 2f;    // fuerza máxima de steer
    private bool _isFlocking = false;     // flag para saber si estoy flockeando

    // Chequeos Para Cambios de Estado
    private float _safeDamage = 50f;
    private float _applePickUpRange = 5f;

    // Variables internas
    private float distance = 0f;

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

        if (_animator != null)
            _animator.SetBool("isWalking", true);
    }

    public override void OnUpdate()
    {
        DetectThings();

        if (_currentApple != null)
        {
            Debug.Log("Prey: Voy a la manzana");
            fsm.ChnageState(AgentStates.PickUp);
            return;
        }

        if (_currentRivalHunter != null)
        {
            float distToRival = Vector3.Distance(_myRoot.position, _currentRivalHunter.transform.position);

            if (distToRival > detectRadius)
            {
                _currentRivalHunter = null;
            }
            else
            {
                Debug.Log("Prey: ENEMIGO A LA VISTA");

                if (distToRival <= _attackRange)
                {
                    Debug.Log("Prey: Attack");
                    fsm.ChnageState(AgentStates.Attack);
                    return;
                }
                else if (_targetLife._currentLife < _safeDamage)
                {
                    Debug.Log("Prey: Evade");
                    fsm.ChnageState(AgentStates.Evade);
                    return;
                }
            }
        }

        // --- Movimiento ---
        if (!_isFlocking)   // si no está flockeando, patrulla
        {
            SeekArriveCount();
            WayPointsLoop();
        }
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

        Vector3 desired;
        if (distance < ArrivingDistance)
            desired = dir.normalized * movSpeed * (distance / ArrivingDistance);
        else
            desired = dir.normalized * movSpeed;

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, steeringForce);
        velocity = Vector3.ClampMagnitude(velocity + steering, movSpeed);

        _myRoot.position += velocity * Time.deltaTime;

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

    private void FlockingMove(List<Boids> neighbors)
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        int count = 0;

        foreach (var boid in neighbors)
        {
            if (boid == null || boid.transform == _myRoot) continue;

            float dist = Vector3.Distance(_myRoot.position, boid.transform.position);

            if (dist < _flockingRadius)
            {
                if (dist < 1.5f)
                    separation += (_myRoot.position - boid.transform.position).normalized / dist;

                alignment += boid.Velocity; // ⚠️ tu clase Boids debe tener public Vector3 velocity
                cohesion += boid.transform.position;

                count++;
            }
        }

        if (count > 0)
        {
            separation /= count;
            alignment /= count;
            cohesion = ((cohesion / count) - _myRoot.position).normalized;

            Vector3 flockingForce =
                  separation * 1.5f
                + alignment.normalized * 1.0f
                + cohesion * 1.0f;

            Vector3 desired = flockingForce.normalized * movSpeed;
            Vector3 steering = desired - velocity;
            steering = Vector3.ClampMagnitude(steering, _flockingForce);

            velocity = Vector3.ClampMagnitude(velocity + steering, movSpeed);
            _myRoot.position += velocity * Time.deltaTime;

            if (velocity.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
                _myRoot.rotation = Quaternion.Slerp(_myRoot.rotation, targetRotation, Time.deltaTime * 5f);
            }

            _isFlocking = true;
            Debug.Log("Prey: Flockeando con " + count + " vecinos 🕊️");
        }
        else
        {
            _isFlocking = false;
        }
    }

    private void DetectThings()
    {
        _currentApple = AppleManager.instance.GetClosestApple(_myRoot.position, _applePickUpRange);
        _currentRivalHunter = HunterManager.Instance.GetClosestHunter(_myRoot.position);

        List<Boids> neighbors = BoidsManager.Instance.GetNeighbors(_myRoot.position, _flockingRadius);
        FlockingMove(neighbors);
    }
}