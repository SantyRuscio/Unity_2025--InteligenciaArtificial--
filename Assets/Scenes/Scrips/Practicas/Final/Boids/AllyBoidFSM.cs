using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class AllyBoidFSM : MonoBehaviour
{
    [Header("Referencias")]
    public Transform leader;
    public LayerMask obstacleMask;

    [Header("Flocking")]
    public float separationDistance = 2f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 2f;
    public float maxSpeed = 5f;

    [Header("Vision")]
    public float detectionRange = 10f;
    public float visionAngle = 120f;

    private AllyBoidFSMController fsm;

    void Start()
    {
        fsm = new AllyBoidFSMController();

        var follow = new BoidFollowLeaderState().SetUp(fsm, this);
        var attack = new BoidAttackState().SetUp(fsm, this);
        var ret = new BoidReturnState().SetUp(fsm, this);

        fsm.states.Add(BoidStateType.FollowLeader, follow);
        fsm.states.Add(BoidStateType.Attack, attack);
        fsm.states.Add(BoidStateType.ReturnToFormation, ret);

        fsm.currentState = follow;
        fsm.currentState.OnEnter();
    }

    void Update()
    {
        fsm.OnUpdate();
    }

    public bool CanSeeEnemy(out Transform enemy)
    {
        enemy = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var col in hits)
        {
            if (col.CompareTag(this.tag)) continue;
            if (!(col.CompareTag("teamA") || col.CompareTag("teamB"))) continue;

            Vector3 dir = (col.transform.position - transform.position).normalized;
            float dist = Vector3.Distance(transform.position, col.transform.position);

            if (Vector3.Angle(transform.forward, dir) > visionAngle / 2f)
                continue;

            if (!Physics.Raycast(transform.position, dir, dist, obstacleMask))
            {
                enemy = col.transform;
                return true;
            }
        }

        return false;
    }
}

