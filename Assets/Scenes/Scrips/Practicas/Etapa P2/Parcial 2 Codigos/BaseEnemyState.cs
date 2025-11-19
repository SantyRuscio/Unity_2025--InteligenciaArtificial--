using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================

public abstract class BaseEnemyState
{
    protected EnemyFSMController fsm;
    protected EnemyFSM root;
    protected Transform transform;
    protected PathFinderParcial_Astar pathFinder;
    protected Transform target;

    public BaseEnemyState SetUp(EnemyFSMController _fsm, EnemyFSM _root)
    {
        fsm = _fsm;
        root = _root;
        transform = _root.transform;
        pathFinder = _root.pathFinder;
        target = _root.target;
        return this;
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}

