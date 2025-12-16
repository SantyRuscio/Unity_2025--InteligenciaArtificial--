using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===============================
// Ruscio - Beghin
// ===============================

public class PathFinderParcial_ThetaStar : MonoBehaviour
{
    [Header("Movimiento")]
    public float movementSpeed = 5f;
    public float stopDistance = 0.3f;

    [Header("Detección")]
    public float obstacleCheckDistance = 0.6f;

    [Header("Masks")]
    public LayerMask obstacleMask;

    private List<NodeParcial_Astar> finalPath;
    private int currentIndex;
    private bool moving;

    private Vector3 directTarget;
    private bool hasDirectTarget;

    private Vector3 lastGoal;

    void Update()
    {
        if (IsMoving && DetectObstacleAhead())
        {
            RecalculatePath();
            return;
        }

        if (hasDirectTarget)
        {
            MoveDirect();
            return;
        }

        if (moving && finalPath != null && currentIndex < finalPath.Count)
        {
            MoveAlongPath();
        }
    }

    public bool IsMoving => moving || hasDirectTarget;

    public void SetDirectTarget(Vector3 pos)
    {
        directTarget = pos;
        hasDirectTarget = true;
        moving = false;
    }

    private void MoveDirect()
    {
        Vector3 dir = directTarget - transform.position;
        dir.y = 0;

        if (dir.magnitude <= stopDistance)
        {
            hasDirectTarget = false;
            return;
        }

        transform.position += dir.normalized * movementSpeed * Time.deltaTime;
    }

    public void BuscarNuevoCamino(Vector3 objetivo)
    {
        hasDirectTarget = false;
        lastGoal = objetivo;

        NodeParcial_Astar start = NodeParcial_Astar.GetClosestNode(transform.position);
        NodeParcial_Astar goal = NodeParcial_Astar.GetClosestNode(objetivo);

        if (start == null || goal == null)
        {
            Debug.LogWarning("No hay nodos cercanos.");
            return;
        }

        finalPath = ThetaStar(start, goal);
        currentIndex = 0;
        moving = finalPath != null;
    }

    private void RecalculatePath()
    {
        CancelPath();
        BuscarNuevoCamino(lastGoal);
    }

    private List<NodeParcial_Astar> ThetaStar(NodeParcial_Astar start, NodeParcial_Astar goal)
    {
        var open = new SimplePriorityQueue<NodeParcial_Astar>();
        var cameFrom = new Dictionary<NodeParcial_Astar, NodeParcial_Astar>();
        var gScore = new Dictionary<NodeParcial_Astar, float>();
        var fScore = new Dictionary<NodeParcial_Astar, float>();

        foreach (var n in NodeParcial_Astar.AllNodes)
        {
            gScore[n] = Mathf.Infinity;
            fScore[n] = Mathf.Infinity;
        }

        gScore[start] = 0f;
        fScore[start] = Vector3.Distance(start.Position, goal.Position);

        open.Enqueue(start, fScore[start]);
        cameFrom[start] = start;

        while (open.Count > 0)
        {
            NodeParcial_Astar current = open.Dequeue();

            if (current == goal)
                return ReconstructThetaPath(cameFrom, start, goal);

            foreach (var neighbor in current.Connections)
            {
                if (!HasLineOfSight(current.Position, neighbor.Position))
                    continue;

                float tentativeCost =
                    gScore[current] + Vector3.Distance(current.Position, neighbor.Position);

                if (tentativeCost < gScore[neighbor])
                {
                    gScore[neighbor] = tentativeCost;
                    fScore[neighbor] = tentativeCost +
                        Vector3.Distance(neighbor.Position, goal.Position);

                    cameFrom[neighbor] = current;
                    open.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        return null;
    }

    private List<NodeParcial_Astar> ReconstructThetaPath(
        Dictionary<NodeParcial_Astar, NodeParcial_Astar> cameFrom,
        NodeParcial_Astar start,
        NodeParcial_Astar goal)
    {
        List<NodeParcial_Astar> path = new List<NodeParcial_Astar>();
        NodeParcial_Astar current = goal;

        while (current != start)
        {
            path.Add(current);

            NodeParcial_Astar parent = cameFrom[current];
            NodeParcial_Astar grandParent =
                cameFrom.ContainsKey(parent) ? cameFrom[parent] : null;

            if (grandParent != null &&
                HasLineOfSight(grandParent.Position, current.Position))
            {
                cameFrom[current] = grandParent;
            }

            current = cameFrom[current];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }

    private bool HasLineOfSight(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        return !Physics.Raycast(from, dir.normalized, dir.magnitude, obstacleMask);
    }

    private bool DetectObstacleAhead()
    {
        Vector3 dir = TargetDirection;
        if (dir == Vector3.zero) return false;

        return Physics.Raycast(
            transform.position + Vector3.up * 0.2f,
            dir,
            obstacleCheckDistance,
            obstacleMask
        );
    }

    private void MoveAlongPath()
    {
        Vector3 target = finalPath[currentIndex].Position;
        target.y = transform.position.y;

        Vector3 dir = target - transform.position;

        if (dir.magnitude <= stopDistance)
        {
            currentIndex++;
            if (currentIndex >= finalPath.Count)
            {
                moving = false;
                return;
            }
            return;
        }

        transform.position += dir.normalized * movementSpeed * Time.deltaTime;
    }

    public void CancelPath()
    {
        moving = false;
        hasDirectTarget = false;
        finalPath = null;
    }

    public Vector3 TargetDirection
    {
        get
        {
            if (hasDirectTarget)
                return (directTarget - transform.position).normalized;

            if (finalPath != null && currentIndex < finalPath.Count)
                return (finalPath[currentIndex].Position - transform.position).normalized;

            return Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        if (IsMoving)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                transform.position + Vector3.up * 0.2f,
                transform.position + Vector3.up * 0.2f +
                TargetDirection * obstacleCheckDistance
            );
        }

        if (finalPath == null || finalPath.Count == 0)
            return;

        Gizmos.color = Color.green;
        for (int i = 0; i < finalPath.Count - 1; i++)
            Gizmos.DrawLine(finalPath[i].Position, finalPath[i + 1].Position);

        if (currentIndex < finalPath.Count)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(finalPath[currentIndex].Position, 0.35f);

            if (currentIndex + 1 < finalPath.Count)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(finalPath[currentIndex + 1].Position, 0.25f);
            }
        }
    }
}



public class SimplePriorityQueue<T>
{
    private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add(new KeyValuePair<T, float>(item, priority));
    }

    public T Dequeue()
    {
        if (elements.Count == 0) return default;

        int bestIndex = 0;
        float bestPriority = elements[0].Value;

        for (int i = 1; i < elements.Count; i++)
        {
            if (elements[i].Value < bestPriority)
            {
                bestPriority = elements[i].Value;
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }

}



