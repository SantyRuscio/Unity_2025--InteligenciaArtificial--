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

    // Topes del mapa
    [SerializeField] private Vector3 minBounds = new Vector3(-12f, 0f, -12f);
    [SerializeField] private Vector3 maxBounds = new Vector3(12f, 0f, 12f);

    // Variables internas
    private Vector3 velocity = Vector3.zero;
    private int _currentWaypoint = 0;

    public BoidsFlocking(Transform[] waypoints, BoidsLife targetLife, Animator anim)
    {
        _wayPoints = waypoints;
        _targetLife = targetLife;
        _animator = anim;
    }

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

        float separationRadius = 1.5f;
        float cohesionRadius = 5f;

        foreach (var boid in neighbors)
        {
            if (boid == null || boid.transform == _myRoot) continue;

            float dist = Vector3.Distance(_myRoot.position, boid.transform.position);

            if (dist < _flockingRadius)
            {
                if (dist < separationRadius)
                    separation += (_myRoot.position - boid.transform.position).normalized / dist;

                alignment += boid.Velocity;

                if (dist < cohesionRadius)
                    cohesion += boid.transform.position;

                count++;
            }
        }

        Vector3 flockingForce = Vector3.zero;
        if (count > 0)
        {
            separation /= count;
            alignment /= count;
            cohesion = ((cohesion / count) - _myRoot.position);

            flockingForce = separation * 1.5f + alignment.normalized * 1.0f + cohesion.normalized * 1.0f;
            flockingForce = Vector3.ClampMagnitude(flockingForce, _flockingForce);
        }

        // 🔹 Seek hacia waypoint
        Transform currentWaypointTransform = _wayPoints[_currentWaypoint];
        Vector3 dirToWaypoint = currentWaypointTransform.position - _myRoot.position;
        Vector3 desiredWaypoint = dirToWaypoint.normalized * _movSpeed;
        Vector3 steeringWaypoint = desiredWaypoint - velocity;
        steeringWaypoint = Vector3.ClampMagnitude(steeringWaypoint, _steeringForce);

        // 🔹 Combinar ambas fuerzas
        velocity = Vector3.ClampMagnitude(velocity + flockingForce + steeringWaypoint, _movSpeed);
        _myRoot.position += velocity * Time.deltaTime;

        // 🔹 Avanzar waypoint si llega
        if (dirToWaypoint.magnitude < 1.5f) // distancia de llegada al waypoint
            _currentWaypoint = (_currentWaypoint + 1) % _wayPoints.Length;

        // 🔹 Limites mapa
        _myRoot.position = new Vector3(
            Mathf.Clamp(_myRoot.position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(_myRoot.position.y, _minVertical, _topVertical),
            Mathf.Clamp(_myRoot.position.z, minBounds.z, maxBounds.z)
        );

        // Rotación
        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
            _myRoot.rotation = Quaternion.Slerp(_myRoot.rotation, targetRotation, Time.deltaTime * 5f);
        }

        _isFlocking = true;
    }

    private void DetectThings()
    {
        // Buscar vecinos
        List<Boids> neighbors = BoidsManager.Instance.GetNeighbors(_myRoot.position, _flockingRadius);

        // Buscar apple y hunter
        _currentApple = AppleManager.instance.GetClosestApple(_myRoot.position, _applePickUpRange);
        _currentRivalHunter = HunterManager.Instance.GetClosestHunter(_myRoot.position);

        // Chequeos de ataque / pickup
        if (_currentRivalHunter != null && Vector3.Distance(_myRoot.position, _currentRivalHunter.transform.position) <= _attackRange)
        {
            Debug.Log("Prey: Detecté enemigo, paso a Attack");
            fsm.ChnageState(AgentStates.Attack);
            return;
        }

        if (_currentApple != null && Vector3.Distance(_myRoot.position, _currentApple.position) <= _applePickUpRange)
        {
            Debug.Log("Prey: Detecté manzana, paso a PickUp");
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
