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
                Vector3 direction = path[index].transform.position - transform.position;

                if(direction.magnitude < closeDist)
                {
                    index = index + 1;
                    if(index >= path.Count)
                    {
                        index = 0;
                        walk = false;
                    }
                    else
                    {
                        transform.position = transform.position + direction * Speed * Time.deltaTime;
                    }
                }
            }
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