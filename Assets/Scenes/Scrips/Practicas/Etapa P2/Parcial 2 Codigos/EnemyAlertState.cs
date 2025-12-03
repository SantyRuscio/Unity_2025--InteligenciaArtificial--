using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================
// public class EnemyAlertState : BaseEnemyState
// {
//     public override void OnEnter()
//     {
//         base.OnEnter();
//         Debug.Log("Entró en ALERT");
//         pathFinder.BuscarNuevoCamino(root.lastSeenPosition);
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
//         if (!pathFinder.IsMoving)
//         {
//             fsm.ChangeState(EnemyStateType.Patrol);
//             return;
//         }
//     }
// 
//     public override void OnExit()
//     {
//         base.OnExit();
//         Debug.Log("Salió de ALERT");
//     }
// }


