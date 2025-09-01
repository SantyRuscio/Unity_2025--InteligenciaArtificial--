using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    public override void OnEnter()
    {
        Debug.Log("entre a Chase");
    }

    public override void OnUpdate()
    {
        Debug.Log("Si esta en mi rango de perseguir  y tengo vida lo persigo y lo ataco");
        Debug.Log("Si no esta en mi rango lo dejo libre y vuelvo a patrol");//fsm.ChnageState(EnemyStates.Patrol);
    }

    public override void OnExit()
    {
        Debug.Log("sali de Chase");
    }
}
