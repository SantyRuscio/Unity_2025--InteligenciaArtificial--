using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    private Animator _animator;

    public override void OnEnter()
    {
        Debug.Log("entre a idle");

        if (_animator != null)
            _animator.SetBool("Walk", false); // 🔥 se asegura de apagar caminata

        // apenas entra en Idle, cambia a Patrol
        fsm.ChnageState(EnemyStates.Patrol);
    }

    public override void OnExit()
    {
        Debug.Log("sali de idle");
    }
}