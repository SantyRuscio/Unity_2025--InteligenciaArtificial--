using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringTest : MonoBehaviour
{
    Vector3 desired = Vector3.zero; //vector deseado que apunta el target
    Vector3 velocity = Vector3.zero; //direccion y Magnitud del vector
    Vector3 steering = Vector3.zero; // Vector de ajueste/steeroing

    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 5f;
    [SerializeField] float steeringForce = 0.1f;

    [SerializeField] float ArrivingDistance = 3f;
    [SerializeField] float PursuitMinDistance = 5f;

    float distance = 0f;

    private void Update()
    {

        dir = Target.Position - transform.position;
        distance = dir.magnitude;


        if (distance > PursuitMinDistance)
        {
            dir = (Target.Position + Target.Velocity) - transform.position; //Pusuit
            desired = dir.normalized * movSpeed;
        }
        else
        {
            if (distance < ArrivingDistance)
            {
                desired = dir.normalized * movSpeed * (distance / ArrivingDistance);//Arriving
            }
            else
            {
                desired = dir.normalized * movSpeed; //Seek
            }
        }

        steering = desired - velocity;

        steering = Vector3.ClampMagnitude(steering, steeringForce);

        velocity = Vector3.ClampMagnitude(velocity + steering, movSpeed);

        transform.position += velocity * Time.deltaTime;

        transform.forward = velocity; //Para que rote mirando al targetr
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; //desired
        Gizmos.DrawLine(transform.position, transform.position + desired);

        Gizmos.color = Color.red; //Velocity
        Gizmos.DrawLine(transform.position, transform.position + velocity);

        Gizmos.color = Color.blue; //Steering
        Gizmos.DrawLine(transform.position, transform.position + steering);

        Gizmos.color = Color.white; //ArrivingDistance
        Gizmos.DrawWireSphere(transform.position, ArrivingDistance);


        Gizmos.color = Color.yellow; //PursuitMinDistance
        Gizmos.DrawWireSphere(transform.position, PursuitMinDistance);

    }
}
