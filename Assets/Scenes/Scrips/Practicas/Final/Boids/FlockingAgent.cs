using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FlockingAgent : MonoBehaviour
{
    [Header("Flocking Settings")]
    public float neighborRadius = 4f;
    public float separationRadius = 2f;

    public float separationWeight = 1.5f;
    public float cohesionWeight = 0.8f;
    public float alignmentWeight = 1.0f;
    public float followLeaderWeight = 2f;

    public float moveSpeed = 4f;
    public float rotationSpeed = 6f;

    [Header("References")]
    public Transform leader;

    private List<FlockingAgent> allAgents;

    void Start()
    {
        allAgents = new List<FlockingAgent>(FindObjectsOfType<FlockingAgent>());
    }

    void Update()
    {
        Vector3 flockDir = ComputeFlocking();
        Move(flockDir);
    }

    Vector3 ComputeFlocking()
    {
        Vector3 separation = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 alignment = Vector3.zero;

        int neighborCount = 0;

        foreach (var agent in allAgents)
        {
            if (agent == this) continue;

            float dist = Vector3.Distance(transform.position, agent.transform.position);

            if (dist <= neighborRadius)
            {
                neighborCount++;

                // Separaci�n
                if (dist <= separationRadius)
                {
                    separation += (transform.position - agent.transform.position).normalized / dist;
                }

                
                cohesion += agent.transform.position;

                
                alignment += agent.transform.forward;
            }
        }

        if (neighborCount > 0)
        {
            cohesion = (cohesion / neighborCount) - transform.position;
            alignment /= neighborCount;
        }

        // Seguir al l�der
        Vector3 followLeader = Vector3.zero;
        if (leader != null)
        {
            followLeader = (leader.position - transform.position).normalized;
        }

        // F�rmula final del movimiento
        Vector3 direction =
              separation * separationWeight
            + cohesion * cohesionWeight
            + alignment * alignmentWeight
            + followLeader * followLeaderWeight;

        return direction.normalized;
    }
    void Move(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * rotationSpeed
        );

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
}
