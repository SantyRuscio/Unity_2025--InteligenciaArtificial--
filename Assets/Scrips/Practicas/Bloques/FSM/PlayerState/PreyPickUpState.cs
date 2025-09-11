using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PreyPickUpState : BaseState
{

    private TargetLife _targetLife;
    private Animator _animator;

    //Para Obtener Transforms
    private float detectRadius = 10f;
    private LayerMask _detectLayers;
    private Transform _currentApple;

    //Steerings Valores
    private float movSpeed = 3f;
    private float steeringForce = 0.1f;
    private float ArrivingDistance = 1f;

    //Chequeos Para Cambios de Estado
    private float _applePickUpRange = 5f;


    public override void OnEnter()
    {
        Debug.Log("Prey: Entré a PreyPickUpState");

        if (_myRoot != null)
            _animator = _myRoot.GetComponentInChildren<Animator>();

        // if (_animator != null)
        //     _animator.SetBool("isWalking", true);
    }

    public override void OnUpdate()
    {
        DetectThings();

        if (_currentApple != null)
        {
            SeekArriveCount();

            float distToApple = Vector3.Distance(_myRoot.position, _currentApple.position); //TODAVIA NO PUSE LA APPLE
            if (distToApple <= ArrivingDistance)
            {
                Debug.Log("Prey: Manzana recogida, volvemos a Patrol");
                fsm.ChnageState(AgentStates.Patrol);
            }
        }
        else
        {
            fsm.ChnageState(AgentStates.Patrol);
        }
    }


    public override void OnExit()
    {
        Debug.Log("PRAY : sali de PreyPickUpState");
    }

    // Seek + Arrive usando la manzana detectada
    private void SeekArriveCount()
    {
        if (_currentApple == null) return; // Protección extra

        Vector3 dir = _currentApple.position - _myRoot.position;
        float distance = dir.magnitude;

        // Seek + Arrive
        Vector3 desired;
        if (distance < ArrivingDistance)
        {
            desired = dir.normalized * movSpeed * (distance / ArrivingDistance);
        }
        else
        {
            desired = dir.normalized * movSpeed;
        }

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, steeringForce);
        velocity = Vector3.ClampMagnitude(velocity + steering, movSpeed);

        _myRoot.position += velocity * Time.deltaTime;

        // Rotación solo si hay movimiento
        if (velocity.sqrMagnitude > 0.001f)
            _myRoot.forward = velocity.normalized;
    }

    // Detectar manzanas cercanas
    private void DetectThings()
    {
        Collider[] hits = Physics.OverlapSphere(_myRoot.position, detectRadius, _detectLayers);

        float minAppleDist = Mathf.Infinity;
        Transform closestApple = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Apple"))
            {
                float dist = Vector3.Distance(_myRoot.position, hit.transform.position);
                if (dist < _applePickUpRange && dist < minAppleDist)
                {
                    minAppleDist = dist;
                    closestApple = hit.transform;
                }
            }
        }

        _currentApple = closestApple;
    }

}
