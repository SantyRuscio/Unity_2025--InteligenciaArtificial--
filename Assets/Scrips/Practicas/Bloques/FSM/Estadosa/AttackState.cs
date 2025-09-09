using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackState : BaseState
{
    // referencias
    public Transform _myRoot;
    private RivalLife _rivalLife;

    private Animator _animator;

    [Header("Ranges")]
    public float _attackRange = 0.5f;
    [SerializeField] private float _safeDamage = 50f;
    [SerializeField] private float _chaseRange = 6f; 

    [Header("Evade params")]

    [SerializeField] private float arrivingDistance = 1f;
    Vector3 desired = Vector3.zero; //vector deseado que apunta el target
    Vector3 velocity = Vector3.zero; //direccion y Magnitud del vector
    Vector3 steering = Vector3.zero; // Vector de ajueste/steeroing
    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 5f;
    [SerializeField] float steeringForce = 0.1f;

    [SerializeField] float ArrivingDistance = 5f;

    float distance = 0f;

    public void SetRootAndAnimator(Transform root, Animator anim)
    {
        _myRoot = root;
        _animator = anim;
    }

    public override void OnEnter()
    {
        // intentar obtener RivalLife si no est� seteado
        if (_rivalLife == null && _myRoot != null)
            _rivalLife = _myRoot.GetComponent<RivalLife>();

        Debug.Log("Entered AttackState");
    }

    public override void OnUpdate()
    {
        if (_myRoot == null) return; // seguridad

        float distanceToTarget = Vector3.Distance(_myRoot.position, Target.Position);

        if (distanceToTarget <= _attackRange)
        {
            if (_rivalLife._currentLife < _safeDamage)
            {
                Evade(); // evade
            }
            else
            {
                // l�gica de ataque 
                Debug.Log("Atacando al jugador");
                _rivalLife.DamageTaken(20f);
            }
        }
        else
        {
            if (distanceToTarget > _chaseRange)
            {
                fsm.ChnageState(EnemyStates.Patrol);
                return;
            }
        }
    }

    private void Evade()
    {

        Debug.Log("EVADE");

        dir = _myRoot.position - (Target.Position + Target.Velocity);
        distance = dir.magnitude;


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
    }
   


    // SETTERS desde Rival
    public void SetRivalLife(RivalLife life)
    {
        _rivalLife = life;
    }

    public void SetRoot(Transform newroot)
    {
        _myRoot = newroot;
    }
}
