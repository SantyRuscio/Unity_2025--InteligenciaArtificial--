using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSMController
{
    public BaseEnemyState currentState;
    public Dictionary<EnemyStateType, BaseEnemyState> possibleStates = new();

    public void OnUpdate()
    {
        currentState?.OnUpdate();
    }

    public void ChangeState(EnemyStateType newState)
    {
        if (!possibleStates.ContainsKey(newState)) return;
        if (currentState == possibleStates[newState]) return;

        currentState?.OnExit();
        currentState = possibleStates[newState];
        currentState.OnEnter();
    }
}

public enum EnemyStateType { Patrol, Chase, Alert }

