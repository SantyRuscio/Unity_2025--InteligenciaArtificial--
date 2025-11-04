using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class PathFinderParcial_Astar : MonoBehaviour
{
    [Header("Configuración General")]
    [SerializeField] private Transform player;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 0.3f;

    private List<NodeParcial_Astar> currentPath = new List<NodeParcial_Astar>();
    private int currentIndex = 0;
    private bool moving = false;

    private Vector3 lastRequestedTarget = Vector3.positiveInfinity;
    private float recalcDistanceThreshold = 0.5f;

    private Vector3 directTarget = Vector3.positiveInfinity;
    private bool hasDirectTarget = false;

    public bool IsMoving => moving;

    public void CancelPath()
    {
        moving = false;
        hasDirectTarget = false;
        directTarget = Vector3.positiveInfinity;
        currentPath.Clear();
        currentIndex = 0;
    }

    public void SetDirectTarget(Vector3 pos)
    {
        hasDirectTarget = true;
        directTarget = pos;
        currentPath.Clear();
        currentIndex = 0;
        moving = true;
        lastRequestedTarget = pos;
    }

private void Update()
{
    if (moving)

    MoverPorCamino();
}

    // ============================================
    // MÉTODOS PÚBLICOS (usados por EnemyFSM)
    // ============================================

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

    public void BuscarNuevoCamino(Vector3 objetivo)
    {
        hasDirectTarget = false;
        directTarget = Vector3.positiveInfinity;

        if (Vector3.Distance(lastRequestedTarget, objetivo) < recalcDistanceThreshold) return;
        lastRequestedTarget = objetivo;

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
        if (!moving) return;

        if (hasDirectTarget)
        {
            Vector3 dir = (directTarget - transform.position);
            Vector3 dirNormalized = dir.normalized;

            if (dirNormalized.sqrMagnitude > 0.001f)
                transform.forward = Vector3.Lerp(
                    transform.forward,
                    dirNormalized,
                    10f * Time.deltaTime
                );

            transform.position += dirNormalized * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, directTarget) < stopDistance)
            {
                hasDirectTarget = false;
                directTarget = Vector3.positiveInfinity;
                moving = false;
            }

            return;
        }

        if (currentPath == null || currentPath.Count == 0)
        {
            moving = false;
            return;
        }

        if (currentIndex >= currentPath.Count)
        {
            moving = false;
            return;
        }

        NodeParcial_Astar nodoObjetivo = currentPath[currentIndex];
        Vector3 dirNode = (nodoObjetivo.transform.position - transform.position);
        Vector3 dirNodeNormalized = dirNode.normalized;

        if (dirNodeNormalized.sqrMagnitude > 0.001f)
            transform.forward = Vector3.Lerp(
                transform.forward,
                dirNodeNormalized,
                10f * Time.deltaTime
            );

        transform.position += dirNodeNormalized * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, nodoObjetivo.transform.position) < stopDistance)
            currentIndex++;
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

                if (newCost < neighbor.costo)
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

