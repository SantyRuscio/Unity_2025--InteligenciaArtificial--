using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBoidFSMController
{
    public BaseBoidState currentState;
    public Dictionary<BoidStateType, BaseBoidState> states = new();

    public void ChangeState(BoidStateType newState)
    {
        if (states.ContainsKey(newState))
        {
            currentState?.OnExit();
            currentState = states[newState];
            currentState.OnEnter();
        }
    }

    public void OnUpdate()
    {
        currentState?.OnUpdate();
    }
}

public enum BoidStateType
{
    FollowLeader,
    Attack,
    ReturnToFormation
}

