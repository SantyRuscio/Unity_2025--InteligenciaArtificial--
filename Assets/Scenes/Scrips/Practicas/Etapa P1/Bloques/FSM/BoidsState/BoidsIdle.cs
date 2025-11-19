using IA.GenericFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsIdle : BaseState
{
    private Animator _animator;

    public override void OnEnter()
    {
        Debug.Log("PREY: IDLE");

        fsm.ChnageState(AgentStates.Patrol);

    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }
}


