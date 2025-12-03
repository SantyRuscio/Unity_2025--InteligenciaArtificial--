using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderFSMController
{
    public BaseLeaderState currentState;
    public Dictionary<LeaderStateType, BaseLeaderState> possibleStates = new Dictionary<LeaderStateType, BaseLeaderState>();

    public void ChangeState(LeaderStateType newState)
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
public enum LeaderStateType
{
    Idle,
    MoveToPoint,
    Attack
}


