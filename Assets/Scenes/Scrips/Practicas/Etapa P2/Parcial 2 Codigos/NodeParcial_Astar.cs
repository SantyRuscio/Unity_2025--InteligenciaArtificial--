using UnityEngine;
using System.Collections.Generic;

// ===============================
// Ruscio - Beghin
// ===============================
public class NodeParcial_Astar : MonoBehaviour
{
    public List<NodeParcial_Astar> Neighbors = new List<NodeParcial_Astar>();
    public NodeParcial_Astar Parent;
    public float costo;
    public float costoFinal;

    public bool IsOccupied = false;
    public GameObject OccupyingNPC = null; 
    public void Clean()
    {
        Parent = null;
        costo = Mathf.Infinity;
        costoFinal = Mathf.Infinity;
        IsOccupied = false;
        OccupyingNPC = null;
    }

    public void SetParent(NodeParcial_Astar p)
    {
        Parent = p;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsOccupied ? Color.red : Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.15f);

        if (Neighbors != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var n in Neighbors)
            {
                if (n != null)
                    Gizmos.DrawLine(transform.position, n.transform.position);
            }
        }
    }
}