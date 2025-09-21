using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BoidsFlocking : BaseState
{
    // Asignaciones
    private Transform[] _wayPoints;
    private BoidsLife _targetLife;
    private Animator _animator;

    // Parámetros de percepción
    private float _detectRadius = 2f;
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

    // Chequeos
    private float _safeDamage = 50f;
    private float _applePickUpRange = 5f;

    // Límites verticales
    [SerializeField] private float _minVertical = 0f;
    [SerializeField] private float _topVertical = 1.3f;

    // Variables internas
    private float _distance = 0f;
    private Vector3 velocity = Vector3.zero;

    public override void OnEnter()
    {
        Debug.Log("Prey: Entré a Flocking");
        _currentWaypoint = 0;

        if (_myRoot != null)
            _animator = _myRoot.GetComponentInChildren<Animator>();

        if (_animator != null)
            _animator.SetBool("isWalking", true);
    }

    public override void OnUpdate()
    {
        DetectThings();
    }

    public override void OnExit()
    {
        Debug.Log("Prey: Saliendo de Flocking");

        if (_animator != null)
            _animator.SetBool("isWalking", false);
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
    }

    private void DetectThings()
    {
        // Buscar vecinos
        List<Boids> neighbors = BoidsManager.Instance.GetNeighbors(_myRoot.position, _flockingRadius);

        // Buscar apple y hunter
        _currentApple = AppleManager.instance.GetClosestApple(_myRoot.position, _applePickUpRange);
        _currentRivalHunter = HunterManager.Instance.GetClosestHunter(_myRoot.position);

        if (_currentRivalHunter != null && Vector3.Distance(_myRoot.position, _currentRivalHunter.transform.position) <= _attackRange)
        {
            Debug.Log("Prey: Detecté enemigo, paso a Attack");
            fsm.ChnageState(AgentStates.Attack);
            return;
        }

        if (_currentApple != null && Vector3.Distance(_myRoot.position, _currentApple.position) <= _applePickUpRange)
        {
            Debug.Log("Prey: Detecté manzana, paso a Attack");
            fsm.ChnageState(AgentStates.PickUp);
            return;
        }

        if (neighbors.Count > 0)
        {
            FlockingMove(neighbors);
        }
        else
        {
            _isFlocking = false;
            Debug.Log("Prey: No hay boids ni targets, vuelvo a Patrol");
            fsm.ChnageState(AgentStates.Patrol);
        }
    }
}
