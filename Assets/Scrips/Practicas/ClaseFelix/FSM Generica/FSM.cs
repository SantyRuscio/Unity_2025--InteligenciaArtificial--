namespace IA.GenericFSM
{
    using System;
    using UnityEngine;
    public class FSM <T>
    {
        State<T> current;
        public FSM(State<T> first)
        {
            this.current = first;   
        }

        public void StartFSM()
        {
            ChangeState(current);
        }
        public void SendInput(T input) 
        { 
            for (int i = 0; i < current.transitions.Length; i++)
            {
               // if (current.transitions[i].GetInput().Equals(input))
                {
               //     ChangeState(current.transitions[i].GetState());
                }
            }

        }

        private void ChangeState(State<object> state)
        {
            throw new NotImplementedException();
        }

        void ChangeState(State<T> newState)
        {
            if(current != null)
            {
                current.OnEnd();
            }
            current = newState;
            current.OnBegin();  
        }
    }

    public abstract class State<T>
    {
        public State (params Transition[] transitions)
        {
            transitions = transitions;
        }
        public Transition[] transitions;

        public void OnBegin()
        {

        }
        public void OnEnd()
        {

        }
        public void OnUpdate()
        {

        }

    }

    public class Transition<T>
    {
        T input;
        State<T> state;

        public Transition(T input, State<T> state)
        {
            this.input = input;
            this.state = state;
        }
        
        public T GetInput()
        {
            return input;   
        }
        public State<T> GetState()
        {
            return state;
        }
    }

}