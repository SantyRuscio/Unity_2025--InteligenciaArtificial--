using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidFleeState : BaseBoidState
{
    public override void OnUpdate()
    {
        // 1. CHEQUEO DE SALIDA: Si me curaron (o subí el umbral), vuelvo a pelear/seguir
        if (!root.IsLowHealth())
        {
            fsm.ChangeState(BoidStateType.FollowLeader);
            return;
        }

        Vector3 fleeDirection = Vector3.zero;

        // 2. CALCULAR DIRECCIÓN DE HUIDA
        if (root.CanSeeEnemy(out Transform enemy))
        {
            // SI VEO ENEMIGO: Vector desde el enemigo hacia mí (Huida)
            fleeDirection = (root.transform.position - enemy.position).normalized;
        }
        else
        {
            // SI NO VEO ENEMIGO PERO ESTOY HERIDO: Voy hacia el líder (Refugio)
            fleeDirection = (root.leader.position - root.transform.position).normalized;
        }

        // 3. MEZCLAR CON OBSTÁCULOS (Para no chocarse paredes mientras huye)
        Vector3 avoidance = AvoidObstacles();
        Vector3 finalDir = fleeDirection * 2f + avoidance * 5f;
        finalDir.y = 0;

        // 4. MOVERSE
        if (finalDir != Vector3.zero)
        {
            root.transform.rotation = Quaternion.Slerp(
                root.transform.rotation,
                Quaternion.LookRotation(finalDir),
                15f * Time.deltaTime
            );

            // Velocidad aumentada un poco por el pánico (1.2x)
            root.transform.position += finalDir.normalized * (root.maxSpeed * 1.2f) * Time.deltaTime;
        }
    }

    private Vector3 AvoidObstacles()
    {
        if (Physics.Raycast(root.transform.position, root.transform.forward, out RaycastHit hit, 3f, root.obstacleMask))
        {
            Debug.DrawLine(root.transform.position, hit.point, Color.red);
            return Vector3.Reflect(root.transform.forward, hit.normal);
        }
        return Vector3.zero;
    }
}
