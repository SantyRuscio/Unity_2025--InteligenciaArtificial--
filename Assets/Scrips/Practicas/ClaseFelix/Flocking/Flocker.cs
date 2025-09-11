using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Flocker : MonoBehaviour
{
    Vector3 velocity = Vector3.zero;
    public Vector3 Velocity
    {
        get
        {
            return velocity;
        }
    }

    [SerializeField] private float steeringforce = 1.5f;
    [SerializeField] private float flockSpeed = 5f;

    [SerializeField] float separation_force = 1.0f;
    [SerializeField] float aligment_force = 1.0f;
    [SerializeField] float cohesion_force = 1.0f;

    [SerializeField] float separation_radius = 1f;
    [SerializeField] float aligment_radius = 1f;
    [SerializeField] float cohesion_radiuse = 1f;

    [SerializeField] bool leader = false;

    Vector3 dirRandom = Vector3.zero;   
    void Start()
    {
        FlockManager.instance.AddFlocker(this);
        if(leader)
        {
            FlockManager.instance.SetLeader(this);
        }
        velocity = Vector3.forward;
        dirRandom = RandomV();
    }

    Vector3 RandomV()
    {
        return new Vector3(Random.Range(-23, 24), 0, Random.Range(-23, 24)) - transform.position;
    }

    Vector3 desired = Vector3.zero;  

    void Update()
    {
        if (leader)
        {
            desired += Seek(Target.Position);
        }
        else
        {
            var contextSepar = FlockManager.instance.GetFlockers(transform.position, separation_radius);
            var contextAling = FlockManager.instance.GetFlockers(transform.position, aligment_radius, this);
            var contextCohes = FlockManager.instance.GetFlockers(transform.position, cohesion_radiuse, this);

            desired +=
                Separation(contextSepar) * separation_force +
                Aligment(contextAling) * aligment_force +
                Cohesion(contextCohes) * cohesion_force +
                FollowLeader();

            if (desired.magnitude > 0.1f)
            {
                desired = RandomV();
            }
        }

        desired = desired.normalized * flockSpeed;
        velocity += Steering(desired);

        velocity = Vector3.ClampMagnitude(velocity, flockSpeed);    
        transform.position = transform.position + velocity * Time.deltaTime;
        transform.forward = velocity;   

        //if(transform.position.x > 14) transform.position = new Vector3 (-14,0,transform.position.z);
        //if (transform.position.x > -14) transform.position = new Vector3(14, 0, transform.position.z);
        //
        //if (transform.position.z > 14) transform.position = new Vector3(transform.position.x, 0, 14);
        //if (transform.position.z > -14) transform.position = new Vector3(transform.position.x, 0, 14);
    }   

    Vector3 FollowLeader()
    {
        Vector3 dirToLeader = FlockManager.Leader.transform.position + FlockManager.Leader.transform.forward * -2 - transform.position;
        dirToLeader = dirToLeader.normalized;   
        return Vector3.zero;
    }

    Vector3 Separation(List<Flocker> boids) //devuelve direccion
    {
        Vector3 diff = Vector3.zero;
        foreach(var boid in boids)
        {
            Vector3 dir = transform.position - boid.transform.position;

            if (diff.magnitude > 0)
            {
                diff += dir.normalized / dir.magnitude;
            }
        }

        if(boids.Count == 0) return Vector3.zero;
        return diff.normalized;
    }

    Vector3 Aligment(List<Flocker> boids) //devuelve direccion
    {
        Vector3 aling = Vector3.zero;
        
        foreach(var boid in boids)
        {
            aling += boid.Velocity;
        }

        if(boids.Count == 0) return Vector3.zero;

        aling = aling / boids.Count;

        return aling.normalized;
    }

    Vector3 Cohesion(List<Flocker> boids) //devuelve posicion
    {
        Vector3 center = Vector3.zero;

        foreach(var f in boids)
        {
            center += f.transform.position;
        }
        if(boids.Count == 0) return Vector3.zero; 

        center = center / boids.Count;  


        Vector3 dir = center - transform.position; //como devolvia posiscion hacemos este calculo para pasar una dir

        return dir.normalized;
    }


    Vector3 Seek(Vector3 target) //paso posicion todo el calculo
    {
        Vector3 desired = (target - transform.position).normalized * flockSpeed;

        Vector3 steering = desired - velocity;

        steering = Vector3.ClampMagnitude(steering, steeringforce);

        return steering;
    }

    Vector3 Steering(Vector3 desired) // solo el steering
    {

        Vector3 steering = desired - velocity;

        steering = Vector3.ClampMagnitude(steering, steeringforce);

        return steering;
    }

}
