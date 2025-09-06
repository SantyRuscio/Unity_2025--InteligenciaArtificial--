using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    public Transform _myRoot;

    Vector3 desired = Vector3.zero; //vector deseado que apunta el target
    Vector3 velocity = Vector3.zero; //direccion y Magnitud del vector
    Vector3 steering = Vector3.zero; // Vector de ajueste/steeroing

    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 5f;
    [SerializeField] float steeringForce = 0.1f;

    [SerializeField] float ArrivingDistance = 5f;

    float _atackRange = 2f;
    float _chaseRange = 4f;
    float distance = 0f;

    public override void OnEnter()
    {
        Debug.Log("entre a Chase");
    }

    public override void OnUpdate() //// perseguir al personaje mediante Pursuit ///
    {
        if (_myRoot == null) return;  //TIRA ERROR RARO HAY QUE PREFGUNTAR

        // Vector hacia donde estará el target
        dir = (Target.Position + Target.Velocity) - _myRoot.position;
        distance = dir.magnitude;

        // volver a Patrol
        if (distance > _chaseRange)
        {
            Debug.Log("Target fuera de rango, volver a Patrol");
            fsm.ChnageState(EnemyStates.Patrol);
            return;
        }

        // cambiar a Attack
        if (distance <= _atackRange)
        {
            Debug.Log("En rango de ataque, cambiar a Attack");
            fsm.ChnageState(EnemyStates.Attack);
            return;
        }

        // Pursuit
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
        Debug.Log("sali de Chase");
    }

    public void SetRoot(Transform newroot)
    {
        _myRoot = newroot;
    }
}
