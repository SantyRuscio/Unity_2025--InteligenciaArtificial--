using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    // IDLE LO USAMOS PARA INICIALIZAR PATORL O CHEQUEAR
    public override void OnEnter()
    {
        Debug.Log("entre a idle");
        fsm.ChnageState(EnemyStates.Patrol);
    }

        // Si veo un enemigo
        // Esta lejos? -> cambio a Chase
        // Esta cerca -> cambio a attack
    

    public override void OnExit()
    {
        Debug.Log("sali de idle");
    }
}