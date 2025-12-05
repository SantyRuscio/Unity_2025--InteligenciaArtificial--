using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoidFollowLeaderState : BaseBoidState
{
    public override void OnUpdate()
    {
        if (root.CanSeeEnemy(out Transform enemy))
        {
            fsm.ChangeState(BoidStateType.Attack);
            return;
        }

        Vector3 finalSteering = CalculateFlocking();
        root.transform.position += finalSteering * Time.deltaTime;
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

            if (dist < root.separationDistance)
            {
                separation += (root.transform.position - b.transform.position);
            }

            alignment += b.transform.forward;
            cohesion += b.transform.position;

            count++;
        }

        if (count > 0)
        {
            alignment /= count;
            cohesion = (cohesion / count - root.transform.position);
        }

        Vector3 leaderDir = (root.leader.position - root.transform.position).normalized;

        Vector3 final =
            alignment * root.alignmentWeight +
            cohesion * root.cohesionWeight +
            separation * root.separationWeight +
            leaderDir * 3f;

        return final.normalized * root.maxSpeed;
    }
}

