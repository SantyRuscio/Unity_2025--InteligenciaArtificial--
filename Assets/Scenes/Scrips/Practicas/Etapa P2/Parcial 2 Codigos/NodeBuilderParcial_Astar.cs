using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================

public class NodeBuilderParcial_Astar : MonoBehaviour
{
    public float connectionDistance = 5f;
    public LayerMask obstacleMask;

    void Start()
    {
        BuildConnections();
    }

    public void BuildConnections()
    {
        var nodes = FindObjectsOfType<NodeParcial_Astar>();

        // Limpia conexiones previas
        foreach (var n in nodes)
            n.Connections.Clear();

        // Crea conexiones nuevas
        foreach (var a in nodes)
        {
            foreach (var b in nodes)
            {
                if (a == b) continue;

                float dist = Vector3.Distance(a.Position, b.Position);

                if (dist <= connectionDistance)
                {
                    if (!Physics.Linecast(a.Position, b.Position, obstacleMask))
                    {
                        a.Connections.Add(b);
                    }
                }
            }
        }
    }
}


