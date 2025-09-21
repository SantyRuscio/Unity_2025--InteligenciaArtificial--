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
    private float _movSpeed = 3f;
    private float _steeringForce = 2f;
    private float _arrivingDistance = 1.5f;
    private int _currentWaypoint = 0;

    // Flocking
    private float _flockingRadius = 5f;
    private float _flockingForce = 2f;
    private bool _isFlocking = false;

    // Chequeos Para Cambios de Estado
    private float _safeDamage = 50f;
    private float _applePickUpRange = 5f;

    // Variables internas
    private float _distance = 0f;

    // Límites verticales
    [SerializeField] private float _minVertical = 0f;
    [SerializeField] private float _topVertical = 1.3f;

    public BoidsPatrolState(Transform[] _wayPoints, BoidsLife _targetLife, Animator anim)
    {
        this._wayPoints = _wayPoints;
        this._targetLife = _targetLife;
        this._animator = anim;
    }

    public override void OnEnter()
    {
        Debug.Log("Prey: Entré a Patrol");
        _currentWaypoint = 0;

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

        if (!_isFlocking)
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
        Vector3 dir = _wayPoints[_currentWaypoint].position - _myRoot.position;
        _distance = dir.magnitude;

        Vector3 desired;
        if (_distance < _arrivingDistance)
            desired = dir.normalized * _movSpeed * (_distance / _arrivingDistance);
        else
            desired = dir.normalized * _movSpeed;

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, _steeringForce);
        velocity = Vector3.ClampMagnitude(velocity + steering, _movSpeed);

        _myRoot.position += velocity * Time.deltaTime;

        // 🔒 Clamp en Y
        _myRoot.position = new Vector3(
            _myRoot.position.x,
            Mathf.Clamp(_myRoot.position.y, _minVertical, _topVertical),
            _myRoot.position.z
        );

        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
            _myRoot.rotation = Quaternion.Slerp(_myRoot.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void WayPointsLoop()
    {
        if (_distance < _arrivingDistance)
        {
            _currentWaypoint = (_currentWaypoint + 1) % _wayPoints.Length;
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

                alignment += boid.Velocity;
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

            Vector3 desired = flockingForce.normalized * _movSpeed;
            Vector3 steering = desired - velocity;
            steering = Vector3.ClampMagnitude(steering, _flockingForce);

            velocity = Vector3.ClampMagnitude(velocity + steering, _movSpeed);
            _myRoot.position += velocity * Time.deltaTime;

            // 🔒 Clamp en Y
            _myRoot.position = new Vector3(
                _myRoot.position.x,
                Mathf.Clamp(_myRoot.position.y, _minVertical, _topVertical),
                _myRoot.position.z
            );

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
