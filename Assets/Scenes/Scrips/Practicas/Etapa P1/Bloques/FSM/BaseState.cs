using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    protected Transform _myRoot;
    protected Vector3 desired = Vector3.zero; 
    protected Vector3 velocity = Vector3.zero;
    protected Vector3 steering = Vector3.zero;

    public BloquesFsm fsm;

    public BaseState SetUp(BloquesFsm newFsm)
    {
        fsm = newFsm;
        return this;
    }
    public BaseState SetRoot(Transform newRoot)
    {
        _myRoot = newRoot;
        return this;
    }


    public virtual void OnEnter(){}
    public virtual void OnUpdate(){}
    public virtual void OnExit(){}
}
