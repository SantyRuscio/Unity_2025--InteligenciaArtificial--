using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterChaseState : BaseState
{
    private Animator _animator; 

    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 5f;
    [SerializeField] float steeringForce = 0.1f;
    [SerializeField] float ArrivingDistance = 5f;

    private float detectRadius = 15f;
    private Transform _currentRival;
    private LayerMask _detectLayers;

    float _atackRange = 2f;
    float _chaseRange = 6f;
    float distance = 0f;

    public HunterChaseState(Animator _animator, LayerMask _detectLayers)
    {
        this._animator = _animator; 
        this._detectLayers = _detectLayers; 
    }

    public override void OnEnter()
    {
        if (_myRoot != null)
            _animator = _myRoot.GetComponent<Animator>();

        if (_animator != null)
            _animator.SetBool("isWalking", true); 

        Debug.Log("entre a Chase");
    }

    public override void OnUpdate() //// perseguir al personaje mediante Pursuit ///
    {
        if (_myRoot == null) return;
        DetectThing();
        PursuitCount(); // Pursuit Cuentas

        // volver a Patrol si target fuera de rango
        if (distance > _chaseRange)
        {
            Debug.Log("Target fuera de rango, volver a Patrol");
            fsm.ChnageState(AgentStates.Patrol);
            return;
        }

        if (distance <= _atackRange)
        {
            Debug.Log("En rango de ataque, cambiar a Attack");
            fsm.ChnageState(AgentStates.Attack);
            return;
        }
    }

    private void PursuitCount() // Pursuit Cuentas
    {
        if (_currentRival == null) return;

        dir = _currentRival.position - _myRoot.position;
        distance = dir.magnitude;

        desired = dir.normalized * movSpeed;

        steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, steeringForce);

        velocity = Vector3.ClampMagnitude(velocity + steering, movSpeed);

        _myRoot.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.001f)
            _myRoot.forward = velocity.normalized;
    }

    public override void OnExit()
    {
        if (_animator != null)
            _animator.SetBool("isWalking", false);

        Debug.Log("sali de Chase");
    }

    private void DetectThing()
    {
        Collider[] hits = Physics.OverlapSphere(_myRoot.position, detectRadius, _detectLayers);

        float minRivalDist = Mathf.Infinity;
        Transform closestRival = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float dist = Vector3.Distance(_myRoot.position, hit.transform.position);
                if (dist < minRivalDist)
                {
                    minRivalDist = dist;
                    closestRival = hit.transform;
                }
            }
        }

        _currentRival = closestRival;
    }
}