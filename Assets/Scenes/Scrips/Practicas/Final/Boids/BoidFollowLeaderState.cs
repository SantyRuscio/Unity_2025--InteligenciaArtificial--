using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoidFollowLeaderState : BaseBoidState
{

    public override void OnUpdate()
    {
        if (root.IsLowHealth())
        {
            fsm.ChangeState(BoidStateType.Flee);
            return;
        }

        if (root.CanSeeEnemy(out Transform enemy))
        {
            fsm.ChangeState(BoidStateType.Attack);
            return;
        }

        Vector3 finalSteering = CalculateFlocking();

        finalSteering.y = 0;

        if (finalSteering != Vector3.zero)
        {
            root.transform.rotation = Quaternion.Slerp(
                root.transform.rotation,
                Quaternion.LookRotation(finalSteering),
                10f * Time.deltaTime
            );

            root.transform.position += finalSteering * Time.deltaTime;
        }
    }

    private Vector3 CalculateFlocking()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        List<AllyBoidFSM> allBoids = BoidManager.Instance.boids;
        int count = 0;

        foreach (var b in allBoids)
        {
            if (b == root) continue;

            float dist = Vector3.Distance(root.transform.position, b.transform.position);

            if (dist < root.neighborDistance)
            {
                if (dist < root.separationDistance)
                {
                    Vector3 pushForce = root.transform.position - b.transform.position;
                    pushForce.y = 0;
                    separation += pushForce.normalized / dist;
                }

                Vector3 alignFlat = b.transform.forward;
                alignFlat.y = 0;
                alignment += alignFlat;

                Vector3 cohesionFlat = b.transform.position;
                cohesionFlat.y = 0;
                cohesion += cohesionFlat;

                count++;
            }
        }

        float distToLeader = Vector3.Distance(root.transform.position, root.leader.position);

        float leaderPersonalSpace = 2.5f;

        if (distToLeader < leaderPersonalSpace)
        {
            Vector3 pushFromLeader = root.transform.position - root.leader.position;
            pushFromLeader.y = 0;
            separation += (pushFromLeader.normalized / distToLeader) * 5f;
        }

        if (count > 0)
        {
            alignment /= count;
            Vector3 center = cohesion / count;
            Vector3 myPosFlat = root.transform.position;
            myPosFlat.y = 0;
            cohesion = (center - myPosFlat).normalized;
        }

        Vector3 leaderForce = Vector3.zero;
        Vector3 leaderPosFlat = root.leader.position; leaderPosFlat.y = 0;
        Vector3 myPos = root.transform.position; myPos.y = 0;
        Vector3 offsetToLeader = leaderPosFlat - myPos;

        if (distToLeader > leaderPersonalSpace)
        {

            leaderForce = offsetToLeader.normalized;
        }
        else
        {

            leaderForce = Vector3.zero;
        }

        Vector3 final =
            (alignment * root.alignmentWeight) +
            (cohesion * root.cohesionWeight) +
            (separation * root.separationWeight * 3f) +
            (leaderForce * 2f) + 
            (ObstacleAvoidance() * 15f);

        final.y = 0;

        return final.normalized * root.maxSpeed;
    }
    private Vector3 ObstacleAvoidance()
    {
        if (Physics.Raycast(root.transform.position, root.transform.forward, out RaycastHit hit, 3f, root.obstacleMask))
        {
            Debug.DrawLine(root.transform.position, hit.point, Color.red);
            return Vector3.Reflect(root.transform.forward, hit.normal);
        }
        return Vector3.zero;
    }
}

