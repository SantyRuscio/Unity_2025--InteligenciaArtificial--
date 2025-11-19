namespace IA.DecisionTree
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class NodeDecision : Node
    {
        //PREGUNTA , QUE DA A 2 CAMINOS :S CAMINO 1 Y CAMINO 2

        public Node Way1;  //camino 1
        public Node Way2;  //camino 2
        public abstract bool Predicate();
        public override void Execute()
        {
            if (Predicate())
            {
                Way1.Execute();  //camino 1
            }
            else
            {
                Way2.Execute(); //camino 2
            }
        }
    }
}