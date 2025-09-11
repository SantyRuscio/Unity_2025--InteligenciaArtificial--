namespace IA.DecisionTree
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class NodeAccion : Node
    {
        public abstract void DoAction();
        public override void Execute()
        {
            DoAction();
        }
    }

}