using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
   public static Vector3 Position { get => instance.transform.position; }
    [SerializeField] float speed = 1.0f;

    Vector3 velocity;

    public static Vector3 Velocity
    {
        get 
        {
            return instance.velocity;
        }
    }

    public static Target instance;

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    Vector3 dir = Vector3.zero;
    Vector3 adjust = Vector3.zero;

}
