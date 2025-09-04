using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Wander : MonoBehaviour
{
    Vector3 desired = Vector3.zero; //vector deseado que apunta el target
    Vector3 velocity = Vector3.zero; //direccion y Magnitud del vector
    Vector3 steering = Vector3.zero; // Vector de ajueste/steeroing

    Vector3 dir = Vector3.zero;
    [SerializeField] float movSpeed = 5f;
    [SerializeField] float steeringForce = 0.1f;

    [SerializeField] float wanderDistance = 1f;

    [SerializeField] float maxRatioX = 23f;
    [SerializeField] float minRatioX = -23f;

    [SerializeField] float maxRatioZ =  23f;
    [SerializeField] float minRatioZ = -23f;


    Vector3 targetPos = Vector3.zero;
    private void Update()
    {
        dir = targetPos - transform.position;

        if(dir.magnitude < wanderDistance)
        {
            targetPos = new Vector3(Random.Range(maxRatioX, minRatioX), 0, Random.Range(maxRatioZ, minRatioZ));
            // a esto se lo puede igualar a un sistema de waypoints
        }

        desired = dir.normalized * movSpeed;

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

    }
}
