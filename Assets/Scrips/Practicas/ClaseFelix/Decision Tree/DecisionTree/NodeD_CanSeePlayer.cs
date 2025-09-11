using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.DecisionTree;

public class NodeD_CanSeePlayer : NodeDecision
{
    [SerializeField] Transform target;
    [SerializeField] Transform root;
    [SerializeField] float minDistance;
    public override bool Predicate()
    {
        return Vector3.Distance(root.position, target.position) < minDistance;
    }
}
