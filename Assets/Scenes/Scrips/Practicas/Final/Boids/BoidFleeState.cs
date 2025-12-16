using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ===============================
// Ruscio - Beghin
// ===============================

public class BoidFleeState : BaseBoidState
{
    public override void OnUpdate()
    {
        // 1. SALIR SI YA NO ESTÁ HERIDO
        if (!root.IsLowHealth())
        {
            fsm.ChangeState(BoidStateType.FollowLeader);
            return;
        }

        Vector3 fleeDirection = Vector3.zero;

        // 2. DIRECCIÓN DE HUIDA (enemigo o líder)
        if (root.CanSeeEnemy(out Transform enemy))
        {
            fleeDirection = (root.transform.position - enemy.position).normalized;
        }
        else
        {
            fleeDirection = (root.leader.position - root.transform.position).normalized;
        }

        // 3. EVITAR PAREDES (raycast múltiple)
        Vector3 avoidance = AvoidObstacles();

        // 4. MEZCLA FINAL
        Vector3 finalDir = fleeDirection.normalized * 1.5f + avoidance.normalized * 8f;
        finalDir.y = 0;

        // 5. MOVIMIENTO
        if (finalDir != Vector3.zero)
        {
            // rotación suave
            root.transform.rotation = Quaternion.Slerp(
                root.transform.rotation,
                Quaternion.LookRotation(finalDir),
                15f * Time.deltaTime
            );

            // movimiento con boost
            root.transform.position += finalDir.normalized *
                                       (root.maxSpeed * 1.2f) *
                                       Time.deltaTime;
        }
    }


    private Vector3 AvoidObstacles()
    {
        Vector3 avoid = Vector3.zero;
        Transform t = root.transform;

        float rayDist = 3f;

        Vector3[] dirs = new Vector3[]
        {
            t.forward,                                            // centro
            Quaternion.AngleAxis(30, Vector3.up) * t.forward,     // derecha
            Quaternion.AngleAxis(-30, Vector3.up) * t.forward     // izquierda
        };

        foreach (var dir in dirs)
        {
            if (Physics.Raycast(t.position, dir, out RaycastHit hit, rayDist, root.obstacleMask))
            {
                Debug.DrawLine(t.position, hit.point, Color.red, 0.05f);

                // Rebote
                avoid += Vector3.Reflect(dir, hit.normal) * 1.5f;

                // Empuje extra si está pegado
                if (hit.distance < 1f)
                {
                    avoid += hit.normal * 3f;
                }
            }
        }

        return avoid;
    }
}


