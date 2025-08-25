namespace IA.DecisionTreeNoUnity
{
    using System;
    public  class NodeDecision : Node
    {
        //PREGUNTA , QUE DA A 2 CAMINOS :S CAMINO 1 Y CAMINO 2

        Func <bool> predicate;
        public NodeDecision(Func<bool> _predicate, Node _Way1, Node _Way2) 
        {
            predicate = _predicate;
            Way1 = _Way1;
            Way2 = _Way2;
        }   
         Node Way1;  //camino 1
         Node Way2;  //camino 2
        public override void Execute()
        {
            if (predicate.Invoke())
            {
                Way1.Execute();  //camino 1
            }
            else
            {
                Way1.Execute(); //camino 2
            }
        }
    }
}