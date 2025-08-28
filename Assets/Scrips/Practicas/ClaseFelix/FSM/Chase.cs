using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : State
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
           // SendInput("calm");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            //SendInput("In Range");
        }
    }
}