using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using IA.DecisionTree;

public class NodA_Follow : NodeAccion
{
    [SerializeField] Transform target;
    [SerializeField] Transform root;

    [SerializeField] float speed = 5f;
    public override void DoAction()
    {
        Debug.Log("Follow");    

        Vector3 dir = target.position - root.position;  
        dir.Normalize();

        root.position += dir * speed * Time.deltaTime  ;
        root.forward = Vector3.Lerp(root.forward, dir, 1f);
    }

}
