using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AllyBoidFSMController
{
    public BaseBoidState currentState;
    public Dictionary<BoidStateType, BaseBoidState> possibleStates = new Dictionary<BoidStateType, BaseBoidState>();

    public void ChangeState(BoidStateType newState)
    {
        if (possibleStates.ContainsKey(newState))
        {
            currentState?.OnExit();
            currentState = possibleStates[newState];
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


