using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IA.pathFinding
{
    [ExecuteInEditMode]
    public class Node : MonoBehaviour
    {
        [SerializeField] float detectionRadius = 2f;
        [SerializeField] List<Node> neighbors;

        public float costo;//con el que comparamos 
        public float costoFinal; //se va a encolar


        public List<Node> Neighbors { get { return neighbors; } }

        Node parent = null;
        public Node Parent { get { return parent; } }

        //para reconectar camino de regreso
        public void SetParent(Node p)
        {
            parent = p;
        }

        public void Clean()
        {
            parent = null;
            costo = float.MaxValue;
            costoFinal = float.MaxValue;    

        }

        [SerializeField] LayerMask nodeMask;
        [SerializeField] LayerMask maskView;
        [SerializeField] LayerMask floorAndObstacles;

        [SerializeField] float maxSlope = 0.5f;

        [Header("GIZMOS")]
        [SerializeField] bool drawRadius = false;
        [SerializeField] bool drawSphere = false;
        [SerializeField] bool drawConnections = false;

        [ContextMenu("Bake Neighbors")]

        public void BakeNeightbors()
        {
            Adjust();
            Detect();
        }

         void Detect()
         {
            neighbors = new List<Node>();

            Collider[] colls = Physics.OverlapSphere(transform.position, detectionRadius, nodeMask);

            for (int i = 0; i < colls.Length; i++)
            {
                Node node = colls[i].GetComponent<Node>();
                if (node != null && node != this)
                {
                    Vector3 dir = node.transform.position - transform.position;

                    Ray ray = new Ray();
                    ray.origin = transform.position;
                    ray.direction = dir;

                    if (Physics.Raycast(ray, out RaycastHit hit, dir.magnitude, maskView))
                    {
                        Node hitNode = hit.collider.GetComponent<Node>();

                        if (hitNode != null && hitNode == node)
                        {
                            float h = node.transform.position.y - transform.position.y ;

                            if(Mathf.Abs(h) < maxSlope)
                            {
                                neighbors.Add(node);
                            }
                        }
                    }
                }
            }
         }
        void Adjust()
        {
            if (Physics.Raycast(transform.position + Vector3.up * 10, Vector3.down, out  RaycastHit hit , floorAndObstacles))
            {
                transform.position = hit.point + Vector3.up /4;
            }
        }

        private void OnDrawGizmos()
        {
            if (drawRadius)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, detectionRadius);
            }

            if (drawConnections && neighbors != null)
            {
                Gizmos.color = Color.white;
                for (int i = 0; i < neighbors.Count; i++)
                {
                    Gizmos.DrawLine(transform.position, neighbors[i].transform.position);
                }
            }

            if (drawSphere)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(transform.position, 0.05f);
            }
        }
    }
}

