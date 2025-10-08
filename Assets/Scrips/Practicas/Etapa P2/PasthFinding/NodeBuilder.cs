using IA.pathFinding;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class NodeBuilder : MonoBehaviour
{
    private Node[] nodes;

    public static NodeBuilder Instance { get; private set; }

    [SerializeField] private bool update = false;
    [SerializeField] private bool bake = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    public Node[] Nodes
    {
        get
        {
            return nodes;
        }
    }

    private void OnEnable()
        => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

    private void OnDisable()
        => EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
            this.enabled = false;
    }

    void Update()
    {
        if (bake)
        {
            bake = false;
            nodes = GetComponentsInChildren<Node>();

            foreach (Node node in nodes)
            {
                node.BakeNeightbors();
            }

            Debug.Log("Bake terminado. Se generaron conexiones para " + nodes.Length + " nodos.");
        }
    }
}

