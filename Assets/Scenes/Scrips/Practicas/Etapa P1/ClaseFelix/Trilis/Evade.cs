using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : MonoBehaviour
{
    Vector3 desired = Vector3.zero; 
    Vector3 velocity = Vector3.zero;
    Vector3 steering = Vector3.zero;

    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 5f;
    [SerializeField] float steeringForce = 0.1f;

    [SerializeField] float ArrivingDistance = 5f;

    float distance = 0f;

    private void Update()
    {

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

        transform.position += velocity * Time.deltaTime;
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

    }

}
