using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace IA.pathFinding
{
    public class PathFinder : MonoBehaviour
    {
        [SerializeField] float detection_radius;
        [SerializeField] LayerMask nodemask;

        List<Node> path = new List<Node>();
        [SerializeField]  private bool walk = false;

        [SerializeField] float closeDist = 0.5f;
        [SerializeField] float Speed = 5f;

        public float costo;

        [SerializeField] Transform PlayerTR; // PUEDE SER UN INSTACE

        Vector3 desired = Vector3.zero;
        Vector3 steering = Vector3.zero;
        Vector3 velocity = Vector3.zero;
        [SerializeField] float steeringForce = 0.1f;


        void Start()
        {

        }

        void Activar()
        {
            // Node initial = FindeMostClosestNode(transform.position);
            //Node final = FindeMostClosestNode(NodeBuilder.postion .position);          no tengo nada asignado todavia

            //path = BFS(initial, final);

            //if(path != null)
            //{
            //   index = 0;
            //    walk = true;
            //}
        }

        int index = 0;

        private void Update()
        {
            if(walk)
            {
                desired = path[index].transform.position - transform.position;

                if(desired.magnitude < closeDist)
                {
                    index = index + 1;
                    if(index >= path.Count)
                    {
                        index = 0;
                        walk = false;
                    }
                    else
                    {
                        desired = desired.normalized * Speed;

                        desired += Avoid() * avoidForce;

                        steering = desired - velocity;

                        steering = Vector3.ClampMagnitude(steering, steeringForce);

                        velocity += steering;

                        velocity = Vector3.ClampMagnitude(velocity, Speed);

                        transform.position += velocity * Time.deltaTime;
                    }
                }
            }
        }

        [SerializeField] float avoidForce = 1.5f;
        [SerializeField] float AvodiRadious = 3f;
        [SerializeField] float AvodiCastRadious = 1f;
        [SerializeField] LayerMask avoidObstacles;
        [SerializeField] Vector3 avoidDir = Vector3.zero;

        Vector3 Avoid()
        {
            if (Physics.SphereCast(transform.position, AvodiCastRadious, velocity, out RaycastHit hit , AvodiRadious, avoidObstacles))
            {
                avoidDir = Vector3.Reflect(velocity.normalized, hit.normal);

                float magnitude = Math.Max(avoidDir.magnitude , 0.01f);

                return avoidDir.normalized * (AvodiRadious - magnitude);
            }

            return avoidDir;
        }

        List <Node> BFS(Node initial, Node final)
        {
            foreach (var node in NodeBuilder.Instance.Nodes)
            {
                node.Clean();
            }
            Queue<Node> open = new Queue<Node>();
            List<Node> visited= new List<Node>();

            open.Enqueue(initial);
            visited.Add(initial);


            while(open.Count > 0)
            {
                Node current = open.Dequeue();

                if(current == null) 
                { 
                   return Reconstruct(initial, final);  
                }

                foreach(Node n in current.Neighbors)
                {
                    if (visited.Contains(n)) continue;

                    n.SetParent(current);
                    visited.Add(n);
                    open.Enqueue(n);
                }
            }

            return null;
        }

        Dictionary<Tuple<Node, Node>, List<Node>> caminosCocinados;

        List<Node> Astar(Node initial, Node final)
        {
            foreach (Node node in NodeBuilder.Instance.Nodes) { node.Clean(); }

            List<Node> visited = new List<Node>();
            PriorityQueue<Node> abiertos = new PriorityQueue<Node>();

            initial.costo = 0;
            initial.costoFinal = initial.costo + Vector3.Distance(initial.transform.position, final.transform.position);
            abiertos.Enqueue(initial, initial.costoFinal);


            while (abiertos.Count > 0)
            {
                Node current = abiertos.Dequeue();

                if (current == final)
                {
                    return Reconstruct(initial, final);
                }

                visited.Add(current);

                foreach (Node n in current.Neighbors)
                {
                    if (visited.Contains(n)) continue;

                    float newCost = current.costo + Vector3.Distance(current.transform.position, n.transform.position);

                    if (newCost > n.costo)
                    {
                        n.SetParent(current);
                        n.costo = newCost;
                        float H = Vector3.Distance(n.transform.position, final.transform.position);
                        n.costoFinal = n.costo + H;
                        abiertos.Enqueue(n, n.costoFinal);
                    }

                }
            }

            return null;
        }

        List<Node>  Dijkstra(Node initial, Node final)
        {
            foreach(Node node in NodeBuilder.Instance.Nodes) { node.Clean(); }

            List<Node> visited = new List<Node>();

            PriorityQueue<Node> abiertos = new PriorityQueue<Node>();

            initial.costo = 0;  
            abiertos.Enqueue(initial, initial.costo);

            while(abiertos.Count > 0)
            {
                Node current = abiertos.Dequeue();

                if(current == final)
                {
                    return Reconstruct(initial, final);
                }

                visited.Add(current);

                foreach(Node n in current.Neighbors)
                {
                    if (visited.Contains(n)) continue;

                    float newCost = current.costo + Vector3.Distance(current.transform.position, n.transform.position);

                    if(newCost > n.costo)
                    {
                        n.SetParent(current);
                        n.costo = newCost;  
                        abiertos.Enqueue(n,newCost);
                    }

                }
            }

            return null;
        }

        List<Node> Reconstruct(Node initial, Node final)
        {
            List<Node> list = new List<Node>();

            Node curretn = final;

            while (curretn != null && curretn != initial)
            {
                list.Add(curretn);

                curretn = curretn.Parent;
            }
            list.Add(initial);
            list.Reverse();
            return list;
        }

        List<Node> ReconstructTheta(Node initial, Node final)
        {
            List<Node> list = new List<Node>();

            Node curretn = final;

            while (curretn != null && curretn != initial)
            {
                list.Add(curretn);

                //nodo a la vista
                var prev = curretn.Parent;
                var best = prev;

                //itero preguntando si lo tengo a la vista y lo voy seteando como el mejor
                while (best != null && OnSight(curretn, prev)) //lo tengo a la vista y repito
                {
                    best = prev; 
                    prev = prev.Parent;
                }

                curretn = best;
            }
            list.Add(initial);
            list.Reverse();
            return list;
        }

        [SerializeField] LayerMask ThetaObstacleMask; //obstacle o wall
        [SerializeField] float SphereCastRadio = 0.2f;

        public bool OnSight(Node a, Node b)
         {
            Vector3 offset = Vector3.up * SphereCastRadio;

            if (a == null) throw new System.Exception("Node a No Existe");
            if (b == null) throw new System.Exception("Node b No Existe");

            Vector3 dir = (b.transform.position + offset) - (a.transform.position + offset);
            float magnitude = dir.magnitude;

            Ray ray = new Ray(a.transform.position , dir);

            return !Physics.SphereCast(ray, radius: SphereCastRadio, magnitude, ThetaObstacleMask);

         }

        float mostClose;
        Node bestNode;

        Node FindeMostClosestNode(Vector3 point)
        {
            Collider[] cols =  Physics.OverlapSphere(point, detection_radius, nodemask);

            bestNode = null;
            mostClose = float.MaxValue;

            for(int i = 0; i < cols.Length; i++)
            {
                Node node = cols[i].GetComponent<Node>();

                if(node != null)
                {
                    Vector3 dir = point - node.transform.position;

                    if(dir.magnitude < mostClose)
                    {
                        mostClose = dir.magnitude;
                        bestNode = node;    
                    }
                }
            }
            return bestNode;
        }
    }

    public class PriorityQueue<T>
    {
        private List<PriorityPair> list;

        public int Count { get { return list.Count; } }

        public PriorityQueue()
        {
            list = new List<PriorityPair>();
        }

        public void Enqueue(T data, float priority)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].data.Equals(data))
                {
                    list[i].UpdatePriority(priority);
                    list = list.OrderBy(a => a.priority).ToList();
                    return;
                }
            }

            PriorityPair pair = new PriorityPair(data, priority);
            list.Add(pair);
            list = list.OrderBy(a => a.priority).ToList();
        }

        public T Dequeue()
        {
            T element = list[0].data;
            list.RemoveAt(0);
            return element;
        }

        struct PriorityPair
        {
            public T data;
            public float priority;

            public PriorityPair(T _data, float prior)
            {
                data = _data;
                priority = prior;
            }

            public void UpdatePriority(float _priority)
            {
                priority = _priority;
            }
        }
    }

}