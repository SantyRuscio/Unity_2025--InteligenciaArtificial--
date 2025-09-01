using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    public override void OnEnter()
    {
        Debug.Log("entre a AttackState ");
    }

    public override void OnUpdate()
    {
        Debug.Log("if lo tengo en rango lo cago a palos");
        Debug.Log("ELse if se me fue del rango ataque vuevlo a chase"); //fsm.ChnageState(EnemyStates.Chase);

    }

    public override void OnExit()
    {
        Debug.Log("sali de AttackState");
    }
}
