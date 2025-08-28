using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    Transition[] transitions;
 
    FSM fsm;

    [SerializeField] bool Debug;
    private void Awake()
    {
        fsm = GetComponentInParent<FSM>();  
        transitions = GetComponentsInChildren<Transition>();
        Initialize();
    }
    public State GetSatateByInput(string input) 
    {
        for (int i = 0; i < transitions.Length; i++)
        {
            if (transitions[i].input.Equals(input)) // el equales es como poner : == null
            {
                return transitions[i].state;    
            }
        }
        return null;    
    }
    public void Begin()
    {
        OnBegin();
    }
    public void End()
    {
        OnEnd();
    }
    public void RefresUpdate()
    {
        OnUpdate();
    }


    //Obligatorios Por Intscia 
    protected abstract void OnBegin();
    protected abstract void OnEnd();
    protected abstract void OnUpdate();


    //Opcionales
    protected virtual void Initialize()
    {

    }
    
    protected virtual void OnFixedUpdate()
    {

    }

}