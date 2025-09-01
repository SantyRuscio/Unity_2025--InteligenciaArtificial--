using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public  class BloquesFsm
{
    public BaseState _actualState;
    public Dictionary<EnemyStates, BaseState> _possibleStates = new Dictionary<EnemyStates, BaseState>();

    public void OnUpdate()
    {
        _actualState.OnUpdate();
    }
    public void ChnageState(EnemyStates newState)
    {
        if(_possibleStates.ContainsKey(newState)) return;

        _actualState?.OnExit();
        _actualState = _possibleStates[newState];
        _actualState.OnEnter();
    }
}


    public enum EnemyStates
    {   
        Idle,
        Chase,
        Patrol,
        Attack
    }
