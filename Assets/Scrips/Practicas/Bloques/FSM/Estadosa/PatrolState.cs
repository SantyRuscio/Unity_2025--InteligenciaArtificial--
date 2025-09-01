using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseState
{
    public override void OnEnter()
    {
        Debug.Log("entre a Patrol ");
    }

    public override void OnUpdate()
    {
        Debug.Log("if no veo a nadie sigo los waypointss");
        Debug.Log("ELse if veo al enemigo = fsm.ChnageState(EnemyStates.Chase)"); //fsm.ChnageState(EnemyStates.Chase);

    }

    public override void OnExit()
    {
        Debug.Log("sali de Patrol");
    }
}
