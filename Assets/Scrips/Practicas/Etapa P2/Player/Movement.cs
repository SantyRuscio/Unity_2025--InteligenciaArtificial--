using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    private Rigidbody playerBody;
    private float _speed;

    public Movement SetPlayerBody(Rigidbody rb)
    {
        playerBody = rb;
        return this;
    }

    public Movement SetPlayerSpeed(float speed)
    {
        _speed = speed;
        return this;
    }

    public void Move(Vector3 input)
    {
        Vector3 moveVector = new Vector3(
            input.x * _speed,             
            playerBody.velocity.y,        
            input.z * _speed              
        );

        // Aplicar velocidad al Rigidbody
        playerBody.velocity = moveVector;
    }
}

