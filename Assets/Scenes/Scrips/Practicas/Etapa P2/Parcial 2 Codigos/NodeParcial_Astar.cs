using UnityEngine;
using System.Collections.Generic;

// ===============================
// Ruscio - Beghin
// ===============================
    
public class NodeParcial_Astar : MonoBehaviour
{
    public static List<NodeParcial_Astar> AllNodes = new List<NodeParcial_Astar>();

    public List<NodeParcial_Astar> Connections = new List<NodeParcial_Astar>();

    public Vector3 Position => transform.position;

    void Awake()
    {
        if (!AllNodes.Contains(this))
            AllNodes.Add(this);
    }

    public static NodeParcial_Astar GetClosestNode(Vector3 pos)
    {
        NodeParcial_Astar best = null;
        float minDist = Mathf.Infinity;

        foreach (var n in AllNodes)
        {
            float d = Vector3.Distance(pos, n.Position);
            if (d < minDist)
            {
                minDist = d;
                best = n;
            }
        }
        return best;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);

        Gizmos.color = Color.cyan;
        foreach (var c in Connections)
        {
            if (c != null)
                Gizmos.DrawLine(transform.position, c.transform.position);
        }
    }
}
