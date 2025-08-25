namespace IA.DecisionTreeNoUnity
{
    using System;
    public class NodeAccion : Node
    {
        Action doAction;
        public NodeAccion(Action _doAction)
        {
            doAction = _doAction;
        }

        public override void Execute()
        {
            doAction.Invoke();  
        }
    }

}