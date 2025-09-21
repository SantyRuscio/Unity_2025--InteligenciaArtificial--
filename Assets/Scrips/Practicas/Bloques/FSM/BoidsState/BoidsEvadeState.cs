using IA.GenericFSM;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoidsEvadeState : BaseState
{
    private Animator _animator;

    // Steerings Valores
    Vector3 dir = Vector3.zero;
    [SerializeField] private float _movSpeed = 5f;
    [SerializeField] private float _steeringForce = 1f;
    [SerializeField] private float _ArrivingDistance = 5f;
    private float _distance = 0f;
    private float _distanceToRotate = 6f;

    // Para Obtener Transforms
    private float _detectRadius = 7f;
    private Hunter _currentRivalHunter;

    // Chequeos Para Cambios de Estado
    [SerializeField] float EscapeRangeToPatrol = 7f;


    // Topes
    [SerializeField] private Vector3 minBounds = new Vector3(-12f, 0f, -12f);
    [SerializeField] private Vector3 maxBounds = new Vector3(12f, 0f, 12f);
    private float VerticalBounds = 1.3f;


    public override void OnEnter()
    {
        Debug.Log("PRAY : entre a EvadeState"); 

        if (_myRoot != null)
            _animator = _myRoot.GetComponentInChildren<Animator>();

        if (_animator != null)
            _animator.SetBool("isWalking", true);
    }

    public override void OnUpdate()
    {
        DetectThing();
        EvadeCounts();

        if (_distance > EscapeRangeToPatrol)
        {
            Debug.Log("PRAY me escape");
            fsm.ChnageState(AgentStates.Patrol);
        }
    }

    public override void OnExit()
    {
        Debug.Log("PRAY : sali de EvadeState");

        if (_animator != null)
            _animator.SetBool("isWalking", false);
    }

    private void EvadeCounts() // EVADE
    {
        if (_currentRivalHunter == null) return;

        Vector3 rivalPosition = _currentRivalHunter.transform.position;

        dir = _myRoot.position - rivalPosition;
        _distance = dir.magnitude;

        Vector3 desired;
        if (_distance < _ArrivingDistance)
            desired = dir.normalized * _movSpeed * (_distance / _ArrivingDistance);
        else
            desired = dir.normalized * _movSpeed;

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, _steeringForce);

        velocity = Vector3.ClampMagnitude(velocity + steering, _movSpeed);

        velocity.y = 0f;

        _myRoot.position += velocity * Time.deltaTime;

        _myRoot.position = new Vector3(
            Mathf.Clamp(_myRoot.position.x, minBounds.x, maxBounds.x),
            VerticalBounds,
            Mathf.Clamp(_myRoot.position.z, minBounds.z, maxBounds.z)
        );

        // --- ROTACIÓN SUAVE ---
        if (velocity.sqrMagnitude > 0.001f)
        {
            // Direcciones
            Vector3 dirToHunter = (rivalPosition - _myRoot.position).normalized;
            dirToHunter.y = 0f;

            Vector3 dirToMove = velocity.normalized;

            float t = Mathf.InverseLerp(1f, _distanceToRotate, _distance);
     
            Vector3 blendedDir = Vector3.Lerp(dirToHunter, dirToMove, t);

            if (blendedDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(blendedDir);
                _myRoot.rotation = Quaternion.Slerp(_myRoot.rotation, targetRot, Time.deltaTime * 3f);
            }
        }
    }


    private void DetectThing()
    {
        _currentRivalHunter = HunterManager.Instance.GetClosestHunter(_myRoot.position);

        if (_currentRivalHunter != null)
        {
            _distance = Vector3.Distance(_myRoot.position, _currentRivalHunter.transform.position);
            if (_distance > _detectRadius)
            {
                _currentRivalHunter = null;
            }
        }
    }
}

