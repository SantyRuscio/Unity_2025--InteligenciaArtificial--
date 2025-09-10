using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    private Animator _animator; 

    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 5f;
    [SerializeField] float steeringForce = 0.1f;

    [SerializeField] float ArrivingDistance = 5f;

    float _atackRange = 2f;
    float _chaseRange = 6f;
    float distance = 0f;

    public ChaseState(Animator _animator)
    {
        this._animator = _animator; 
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

        PursuitCoutn(); // Pursuit Cuentas

        // volver a Patrol si target fuera de rango
        if (distance > _chaseRange)
        {
            Debug.Log("Target fuera de rango, volver a Patrol");
            fsm.ChnageState(EnemyStates.Patrol);
            return;
        }

        if (distance <= _atackRange)
        {
            Debug.Log("En rango de ataque, cambiar a Attack");
            fsm.ChnageState(EnemyStates.Attack);
            return;
        }
    }

    private void PursuitCoutn() // Pursuit Cuentas
    {
        dir = (Target.Position + Target.Velocity) - _myRoot.position;
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

    public void SetRoot(Transform newroot)
    {
        _myRoot = newroot;
    }
}
