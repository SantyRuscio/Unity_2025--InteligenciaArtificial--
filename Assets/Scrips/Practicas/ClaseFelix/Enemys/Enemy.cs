using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.DecisionTree;

public class Enemy : MonoBehaviour
{
    [SerializeField] Node firstNode;

    private void Update()
    {
        firstNode.Execute();
    }
}
