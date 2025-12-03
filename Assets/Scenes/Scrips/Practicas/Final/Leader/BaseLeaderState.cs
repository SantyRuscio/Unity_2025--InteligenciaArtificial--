using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLeaderState
{
    protected LeaderFSM root;
    protected LeaderFSMController fsm;
    protected PathFinderParcial_ThetaStar pathFinder;
    protected Transform transform;

    public BaseLeaderState SetUp(LeaderFSMController fsm, LeaderFSM root)
    {
        this.fsm = fsm;
        this.root = root;
        this.pathFinder = root.pathFinder;
        this.transform = root.transform;
        return this;
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}



