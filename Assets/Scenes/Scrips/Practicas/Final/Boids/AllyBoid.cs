using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AllyBoid : MonoBehaviour
{
    [Header("Pesos del Flocking")]
    public float separationWeight = 1.5f;
    public float cohesionWeight = 1f;
    public float alignmentWeight = 1f;
    public float followLeaderWeight = 2f;

    [Header("Distancias")]
    public float neighborRadius = 5f;
    public float separationRadius = 1.5f;

    [Header("Referencias")]
    public LeaderFSM myLeader;
    public LayerMask allyMask;

    private Vector3 velocity;

    void Update()
    {
        Vector3 finalForce = Vector3.zero;

        finalForce += Separation() * separationWeight;
        finalForce += Cohesion() * cohesionWeight;
        finalForce += Alignment() * alignmentWeight;
        finalForce += FollowLeader() * followLeaderWeight;

        velocity = finalForce.normalized;

        transform.position += velocity * Time.deltaTime * 3f;

        if (velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(velocity);
    }

    private Vector3 Separation()
    {
        Collider[] allies = Physics.OverlapSphere(transform.position, separationRadius, allyMask);
        Vector3 move = Vector3.zero;

        foreach (var a in allies)
        {
            if (a.transform == this.transform) continue;

            move += (transform.position - a.transform.position);
        }

        return move;
    }

    private Vector3 Cohesion()
    {
        Collider[] allies = Physics.OverlapSphere(transform.position, neighborRadius, allyMask);
        if (allies.Length == 0) return Vector3.zero;

        Vector3 center = Vector3.zero;

        foreach (var a in allies)
            center += a.transform.position;

        center /= allies.Length;

        return (center - transform.position);
    }

    private Vector3 Alignment()
    {
        Collider[] allies = Physics.OverlapSphere(transform.position, neighborRadius, allyMask);
        if (allies.Length == 0) return Vector3.zero;

        Vector3 alignDir = Vector3.zero;

        foreach (var a in allies)
            alignDir += a.transform.forward;

        alignDir /= allies.Length;

        return alignDir;
    }

    private Vector3 FollowLeader()
    {
        if (myLeader == null) return Vector3.zero;

        Vector3 dir = (myLeader.transform.position - transform.position);
        return dir;
    }
}

