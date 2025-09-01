using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    public override void OnEnter()
    {
        Debug.Log("entre a idle");

        // si tengo un waypoint -> cambio a patrol 

        // Si veo un enemigo
        // Esta lejos? -> cambio a Chase
        // Esta cerca -> cambio a attack
    }

    public override void OnExit()
    {
        Debug.Log("sali de idle");
    }
}