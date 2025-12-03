using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseBoidState
{
    protected AllyBoidFSM root;
    protected AllyBoidFSMController fsm;

    public BaseBoidState SetUp(AllyBoidFSMController fsm, AllyBoidFSM root)
    {
        this.fsm = fsm;
        this.root = root;
        return this;
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}
