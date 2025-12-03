using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================
// public class EnemyIdleState : BaseEnemyState
// {
//     private float idleDuration = 1.5f;
//     private float timer;
// 
//     public override void OnEnter()
//     {
//         base.OnEnter();
//         pathFinder.CancelPath();
//         timer = idleDuration;
// 
//         Debug.Log($"{root.name} está en IDLE");
//     }
// 
//     public override void OnUpdate()
//     {
//         if (root.CanSeePlayer())
//         {
//             fsm.ChangeState(EnemyStateType.Chase);
//             return;
//         }
// 
//         timer -= Time.deltaTime;
//         if (timer <= 0f)
//         {
//             fsm.ChangeState(EnemyStateType.Patrol);
//             return;
//         }
//     }
// 
//     public override void OnExit()
//     {
//         base.OnExit();
//         Debug.Log($"{root.name} sale de IDLE");
//     }
// }

