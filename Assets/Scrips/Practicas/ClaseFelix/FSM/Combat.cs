using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : State
{
    protected override void OnBegin()
    {
       
    }

    protected override void OnEnd()
    {

    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // SendInput("player died");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            // SendInput("out of range ");
        }
    }
}