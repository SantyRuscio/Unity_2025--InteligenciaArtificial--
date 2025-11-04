using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathFinderParcial_Astar : MonoBehaviour
{
    [Header("Configuración General")]
    [SerializeField] private Transform player;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 0.3f;

    private List<NodeParcial_Astar> currentPath = new List<NodeParcial_Astar>();
    private int currentIndex = 0;
    private bool moving = false;

    private void Update()
    {
        if (moving && currentPath != null && currentPath.Count > 0)
            MoverPorCamino();
    }

    // ============================================
    // MÉTODOS PÚBLICOS (usados por EnemyFSM)
    // ============================================

    // Devuelve el nodo más cercano a una posición
    public NodeParcial_Astar GetClosestNode(Vector3 position)
    {
        if (NodeBuilderParcial_Astar.Instance == null) return null;

        NodeParcial_Astar best = null;
        float minDist = Mathf.Infinity;

        foreach (var node in NodeBuilderParcial_Astar.Instance.Nodes)
        {
            if (node == null) continue;
            float dist = Vector3.Distance(position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                best = node;
            }
        }
        return best;
    }

    // Calcula un nuevo camino hacia un objetivo
    public void BuscarNuevoCamino(Vector3 objetivo)
    {
        NodeParcial_Astar start = GetClosestNode(transform.position);
        NodeParcial_Astar goal = GetClosestNode(objetivo);

        if (start == null || goal == null)
        {
            moving = false;
            return;
        }

        currentPath = AStar(start, goal);
        currentIndex = 0;
        moving = currentPath != null && currentPath.Count > 0;
    }

    // ============================================
    // LÓGICA DE MOVIMIENTO
    // ============================================

    void MoverPorCamino()
    {
        if (currentIndex >= currentPath.Count)
        {
            moving = false;
            return;
        }

        NodeParcial_Astar objetivo = currentPath[currentIndex];
        Vector3 dir = (objetivo.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, objetivo.transform.position) < stopDistance)
        {
            currentIndex++;
        }
    }

    // ============================================
    // A* IMPLEMENTACIÓN
    // ============================================

    List<NodeParcial_Astar> AStar(NodeParcial_Astar start, NodeParcial_Astar goal)
    {
        if (NodeBuilderParcial_Astar.Instance == null) return null;

        foreach (var node in NodeBuilderParcial_Astar.Instance.Nodes)
        {
            if (node == null) continue;
            node.Clean();
        }

        var open = new PriorityQueue<NodeParcial_Astar>();
        var closed = new List<NodeParcial_Astar>();

        start.costo = 0f;
        start.costoFinal = Vector3.Distance(start.transform.position, goal.transform.position);
        open.Enqueue(start, start.costoFinal);

        while (open.Count > 0)
        {
            NodeParcial_Astar current = open.Dequeue();
            if (current == null) break;

            if (current == goal)
                return Reconstruct(start, goal);

            closed.Add(current);

            if (current.Neighbors == null) continue;

            foreach (NodeParcial_Astar neighbor in current.Neighbors)
            {
                if (neighbor == null) continue;
                if (closed.Contains(neighbor)) continue;

                float newCost = current.costo + Vector3.Distance(current.transform.position, neighbor.transform.position);

                if (neighbor.costo == 0f || newCost < neighbor.costo)
                {
                    neighbor.SetParent(current);
                    neighbor.costo = newCost;
                    neighbor.costoFinal = newCost + Vector3.Distance(neighbor.transform.position, goal.transform.position);
                    open.Enqueue(neighbor, neighbor.costoFinal);
                }
            }
        }

        return null;
    }

    List<NodeParcial_Astar> Reconstruct(NodeParcial_Astar start, NodeParcial_Astar goal)
    {
        List<NodeParcial_Astar> path = new List<NodeParcial_Astar>();
        NodeParcial_Astar current = goal;

        while (current != null && current != start)
        {
            path.Add(current);
            current = current.Parent;
        }

        if (start != null) path.Add(start);
        path.Reverse();
        return path;
    }
}

// ---- Cola de prioridad (A* necesita esto) ----
public class PriorityQueue<T>
{
    private List<PriorityPair> list = new List<PriorityPair>();

    public int Count => list.Count;

    public void Enqueue(T data, float priority)
    {
        list.Add(new PriorityPair(data, priority));
        list = list.OrderBy(x => x.priority).ToList();
    }

    public T Dequeue()
    {
        if (list.Count == 0) return default;
        T data = list[0].data;
        list.RemoveAt(0);
        return data;
    }

    private struct PriorityPair
    {
        public T data;
        public float priority;
        public PriorityPair(T d, float p)
        {
            data = d;
            priority = p;
        }
    }
}

