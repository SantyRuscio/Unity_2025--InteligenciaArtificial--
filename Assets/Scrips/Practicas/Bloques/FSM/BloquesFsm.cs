using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public  class BloquesFsm 
{
    public BaseState _actualState;
    public Dictionary<AgentStates, BaseState> _possibleStates = new Dictionary<AgentStates, BaseState>();

    public void OnUpdate()
    {
        _actualState.OnUpdate();
    }
    public void ChnageState(AgentStates newState)
    {
        if(!_possibleStates.ContainsKey(newState)) return;

        _actualState?.OnExit();
        _actualState = _possibleStates[newState];
        _actualState.OnEnter();
    }
}


    public enum AgentStates
    {   
        Idle,
        Chase,
        Patrol,
        Attack,
        Evade,
        PickUp
    }
