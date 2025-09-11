using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Vector3 direccion;
    [SerializeField] private Vector3 position;
    private Vector3 _actualDirSpeed;

    [SerializeField] private float aceleration;
    [SerializeField] private float _maxSpeed;
    private float _actualVelocity;

    [SerializeField] private Rigidbody _rb;


    private void Awake()
    {
        //transform.position = position;  
    }

    private void Update()
    {
        _actualVelocity += aceleration * Time.deltaTime  ;

        _actualVelocity = Mathf.Clamp(_actualVelocity, -_maxSpeed,  _maxSpeed);

        _actualDirSpeed += direccion.normalized * _actualVelocity; 

        _rb.velocity = _actualDirSpeed;


        //transform.position += direccion.normalized * speed * Time.deltaTime;
    }


   // private void FixedUpdate()
   // {
   //     //_rb.AddForce(direccion);
   //
   // }

}