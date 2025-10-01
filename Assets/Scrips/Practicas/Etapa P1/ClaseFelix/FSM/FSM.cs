using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class FSM : MonoBehaviour
{
    [SerializeField] State first;
    State current;


    void Start()
    {
        ChangeState(first);
    }

    public void SendInput(string input)
    {
       State state = current.GetSatateByInput(input);
        if (state == null) return;

        ChangeState(state);
    }

    void ChangeState(State newState)
    {   
        if (current != null)
        {   
            current?.End();
        }

        current = newState;

        current.Begin();
    }

    void Update()
    {
        current.RefresUpdate();
    }

}
