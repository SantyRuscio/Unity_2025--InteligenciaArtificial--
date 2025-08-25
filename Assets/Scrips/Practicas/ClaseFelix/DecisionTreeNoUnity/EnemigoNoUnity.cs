using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.DecisionTreeNoUnity;
public class EnemigoNoUnity : MonoBehaviour
{
    Node firstNode; 
    // Start is called before the first frame update
    void Start()
    {
        firstNode = new NodeDecision
            (
                  CanSeePlayer,
                  new NodeDecision
                  (
                    InRange,
                    new NodeAccion(AtackwAction),
                    new NodeAccion(FollowAction)
                  ),
                   new NodeAccion(PatrolAction)
            );
               
    }


    void FollowAction()
    {
        Debug.Log("Follow");
    }
    void AtackwAction()
    {
        Debug.Log("Atack");
    }

    void PatrolAction()
    {
        Debug.Log("Patrol");
    }

    bool CanSeePlayer()
    {
        return true;    
    }

    bool InRange()
    {
        return true;
    }



    // Update is called once per frame
    void Update()
    {
        firstNode.Execute();    
    }
}
