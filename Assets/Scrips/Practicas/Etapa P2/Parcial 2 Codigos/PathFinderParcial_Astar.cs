using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class PathFinderParcial_Astar : MonoBehaviour
{
    [Header("Configuración General")]
    [SerializeField] private Transform player;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float stopDistance = 0.5f; 
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Debug Visual")]
    [SerializeField] private bool showDebug = true;
    [SerializeField] private Color pathColor = Color.green;
    [SerializeField] private Color directColor = Color.cyan;
    [SerializeField] private Color nodeColor = Color.yellow;

    private List<NodeParcial_Astar> currentPath = new List<NodeParcial_Astar>();
    private int currentIndex = 0;
    private bool moving = false;

    private Vector3 lastRequestedTarget = Vector3.positiveInfinity;
    private float recalcDistanceThreshold = 0.5f;

    private Vector3 directTarget = Vector3.positiveInfinity;
    private bool hasDirectTarget = false;

    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private float stuckThreshold = 1f;

    public bool IsMoving => moving;

    // ===============================
    // MÉTODOS PÚBLICOS
    // ===============================

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

    // ===============================
    // LÓGICA DE MOVIMIENTO
    // ===============================

    private void Update()
    {
        if (!moving) return;

        MoverPorCamino();

        if (Vector3.Distance(transform.position, lastPosition) < 0.01f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > stuckThreshold)
            {
                if (hasDirectTarget)
                    BuscarNuevoCamino(directTarget);
                else if (currentPath != null && currentPath.Count > 0)
                    BuscarNuevoCamino(currentPath[currentPath.Count - 1].transform.position);

                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
    }

    private void MoverPorCamino()
    {
        if (!moving) return;

        NodeParcial_Astar nodoObjetivo = null;
        if (hasDirectTarget)
        {
            nodoObjetivo = null;
        }
        else
        {
            if (currentPath == null || currentPath.Count == 0 || currentIndex >= currentPath.Count)
            {
                moving = false;
                return;
            }
            nodoObjetivo = currentPath[currentIndex];
        }

        Vector3 targetPos = hasDirectTarget ? directTarget : nodoObjetivo.transform.position;
        Vector3 dir = targetPos - transform.position;
        float dist = dir.magnitude;

        if (dist < 0.01f)
        {
            if (hasDirectTarget)
            {
                hasDirectTarget = false;
                directTarget = Vector3.positiveInfinity;
                moving = false;
            }
            else
            {

                if (currentIndex > 0)
                {
                    NodeParcial_Astar nodoPrevio = currentPath[currentIndex - 1];
                    nodoPrevio.IsOccupied = false;
                    nodoPrevio.OccupyingNPC = null;
                }

                if (!nodoObjetivo.IsOccupied)
                {
                    nodoObjetivo.IsOccupied = true;
                    nodoObjetivo.OccupyingNPC = gameObject;
                    currentIndex++;
                }

            }
            return; 
        }

        if (!hasDirectTarget && nodoObjetivo.IsOccupied && nodoObjetivo.OccupyingNPC != gameObject)
        {
            return;
        }

        Vector3 dirNormalized = dir.normalized;

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dirNormalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f * Time.deltaTime);
        }

        Vector3 move = dirNormalized * speed * Time.deltaTime;
        if (move.magnitude > dist)
            move = dir; 
        transform.position += move;

        if (dist < stopDistance)
        {
            if (hasDirectTarget)
            {
                hasDirectTarget = false;
                directTarget = Vector3.positiveInfinity;
                moving = false;
            }
            else
            {
                if (currentIndex > 0)
                {
                    NodeParcial_Astar nodoPrevio = currentPath[currentIndex - 1];
                    nodoPrevio.IsOccupied = false;
                    nodoPrevio.OccupyingNPC = null;
                }

                if (!nodoObjetivo.IsOccupied)
                {
                    nodoObjetivo.IsOccupied = true;
                    nodoObjetivo.OccupyingNPC = gameObject;
                    currentIndex++;
                }
            }
        }
    }


    // ===============================
    // A* IMPLEMENTACIÓN
    // ===============================

    private List<NodeParcial_Astar> AStar(NodeParcial_Astar start, NodeParcial_Astar goal)
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

    private List<NodeParcial_Astar> Reconstruct(NodeParcial_Astar start, NodeParcial_Astar goal)
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

    // ===============================
    // DEBUG VISUAL 
    // ===============================

    private void OnDrawGizmos()
    {
        if (!showDebug) return;

        Gizmos.color = hasDirectTarget ? directColor : pathColor;

        if (hasDirectTarget && directTarget != Vector3.positiveInfinity)
        {
            Gizmos.DrawLine(transform.position, directTarget);
            Gizmos.DrawSphere(directTarget, 0.2f);
        }

        if (currentPath != null && currentPath.Count > 1)
        {
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                if (currentPath[i] == null || currentPath[i + 1] == null) continue;
                Gizmos.color = pathColor;
                Gizmos.DrawLine(currentPath[i].transform.position, currentPath[i + 1].transform.position);
                Gizmos.DrawSphere(currentPath[i].transform.position, 0.1f);
            }

            if (currentIndex < currentPath.Count)
            {
                Gizmos.color = nodeColor;
                Gizmos.DrawWireSphere(currentPath[currentIndex].transform.position, 0.25f);
            }
        }
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
        public PriorityPair(T d, float p) { data = d; priority = p; }
    }
}
