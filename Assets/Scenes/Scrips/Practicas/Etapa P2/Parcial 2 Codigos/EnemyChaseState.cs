using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================
// public class EnemyChaseState : BaseEnemyState
// {
//     private float attackRange = 2f;
//     private float loseRange = 15f;
//     private float directFollowRange = 5f;
//     private float pathUpdateTimer = 0f;
//     private float pathUpdateInterval = 0.5f;
// 
//     public override void OnEnter()
//     {
//         base.OnEnter();
//         Debug.Log($"{root.name} CHASE");
//         pathFinder.BuscarNuevoCamino(target.position);
//     }
// 
//     public override void OnUpdate()
//     {
//         float distanceToPlayer = Vector3.Distance(root.transform.position, target.position);
// 
//         if (root.CanSeePlayer())
//         {
//             if (distanceToPlayer <= directFollowRange)
//             {
//                 pathFinder.SetDirectTarget(target.position);
//             }
//             else
//             {
//                 pathUpdateTimer -= Time.deltaTime;
//                 if (pathUpdateTimer <= 0f)
//                 {
//                     pathFinder.BuscarNuevoCamino(target.position);
//                     pathUpdateTimer = pathUpdateInterval;
//                 }
//             }
// 
//             if (distanceToPlayer <= attackRange)
//             {
//                 fsm.ChangeState(EnemyStateType.Attack);
//                 return;
//             }
//         }
//         else
//         {
//             fsm.ChangeState(EnemyStateType.Alert);
//             pathFinder.BuscarNuevoCamino(root.lastSeenPosition);
//             return;
//         }
// 
//         if (distanceToPlayer > loseRange)
//         {
//             fsm.ChangeState(EnemyStateType.Patrol);
//             return;
//         }
//     }
// 
//     public override void OnExit()
//     {
//         Debug.Log($"{root.name} CHASE terminado");
//         pathFinder.CancelPath();
//     }
// }
