using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{  
    public BloquesFsm fsm;

    public BaseState SetUp(BloquesFsm newFsm)
    {
        fsm = newFsm;
        return this;
    }
 
    public virtual void OnEnter(){}
    public virtual void OnUpdate(){}
    public virtual void OnExit(){}
}
