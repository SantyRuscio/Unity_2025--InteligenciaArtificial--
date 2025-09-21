using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCollision : MonoBehaviour
{
    public float radioSeparacion = 1.5f; 

    public BoidCollision[] todosLosBoids;

    public float fuerzaEmpuje = 5f;

    void Start()
    {
        todosLosBoids = FindObjectsOfType<BoidCollision>();
    }

    void Update()
    {
        Vector3 empuje = Vector3.zero;

        foreach (BoidCollision boid in todosLosBoids)
        {
            if (boid == this) continue;

            Vector3 diff = transform.position - boid.transform.position;
            float distancia = diff.magnitude;

            if (distancia < radioSeparacion && distancia > 0f)
            {
                empuje += diff.normalized * (radioSeparacion - distancia);
            }
        }
        transform.position += empuje * fuerzaEmpuje * Time.deltaTime;
    }
}