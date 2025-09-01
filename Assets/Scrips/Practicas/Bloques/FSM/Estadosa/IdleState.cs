using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    public override void OnEnter()
    {
        Debug.Log("entre a idle");
    }

    public override void OnExit()
    {
        Debug.Log("sali de idle");
    }

}
