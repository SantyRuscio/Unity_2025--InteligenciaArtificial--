using IA.GenericFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsIdle : BaseState
{
    //private Animator _animator;

    public override void OnEnter()
    {
        Debug.Log("PREY : IDLE");

       // if (_animator != null)
       //     _animator.SetBool("Walk", false); // 
       //
        // apenas entra en Idle, cambia a Patrol
        fsm.ChnageState(AgentStates.Patrol);
    }

    public override void OnExit()
    {
        Debug.Log("PREY: sali de idle");
    }
}
