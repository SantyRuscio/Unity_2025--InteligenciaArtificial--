using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] Transform root;
    [SerializeField] Transform target;

    [SerializeField] float _distMin = 10f;
    [SerializeField] float _angleMin = 90f;

    [SerializeField] LayerMask toDetect;
    [SerializeField] bool isFov = false;

    private void Update()
    {
        Vector3 dir = target.position - root.position;



        float dist = Vector3.SqrMagnitude(dir);
        if (dist < _distMin * _distMin) 
        {
            print("Dist");
            Vector3 forward = root.forward;
            float angle = Vector3.Angle(forward, dir);

            if (angle < _angleMin * 0.5f) 

                print("Angle");
            Ray ray = new Ray(root.position, dir);

            RaycastHit hit = new RaycastHit();

            Physics.Raycast(ray, out hit, dist, toDetect); 
            { print("ray");
                if ((toDetect & 1 << hit.collider.gameObject.layer) != 0)
                {
                    print("Lo veo");
                    isFov = true;
                }
            }
        }
        isFov = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(root.position, target.position);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(root.position, _distMin);

        Vector3 Left = Quaternion.Euler(0, -_angleMin / 2f, 0) * transform.forward;
        Vector3 Right = Quaternion.Euler(0, _angleMin / 2f, 0) * transform.forward;

        Gizmos.color = Color.red;

        Gizmos.DrawRay(root.position, Left * _distMin);
        Gizmos.DrawRay(root.position, Right * _distMin);

    }
}