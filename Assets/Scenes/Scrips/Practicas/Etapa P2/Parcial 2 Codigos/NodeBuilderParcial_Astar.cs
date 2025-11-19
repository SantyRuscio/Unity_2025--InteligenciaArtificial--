using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================
public class NodeBuilderParcial_Astar : MonoBehaviour
{
    public static NodeBuilderParcial_Astar Instance;

    public List<NodeParcial_Astar> Nodes = new List<NodeParcial_Astar>();
    [SerializeField] private float connectionDistance = 5f;
    [SerializeField] private LayerMask obstacleMask;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BuildConnections();
    }

    public void BuildConnections()
    {
        Nodes.Clear();
        Nodes.AddRange(FindObjectsOfType<NodeParcial_Astar>());

        foreach (var nodeA in Nodes)
        {
            nodeA.Neighbors.Clear();

            foreach (var nodeB in Nodes)
            {
                if (nodeA == nodeB) continue;

                float dist = Vector3.Distance(nodeA.transform.position, nodeB.transform.position);

                if (dist <= connectionDistance)
                {
                    // Solo conecta si no hay obstáculo en medio
                    if (!Physics.Linecast(nodeA.transform.position, nodeB.transform.position, obstacleMask))
                    {
                        nodeA.Neighbors.Add(nodeB);
                    }
                }
            }
        }
    }
}

