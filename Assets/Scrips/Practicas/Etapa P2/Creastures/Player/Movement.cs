using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    private Rigidbody playerBody;
    private float _speed;
    private float _rotationSpeed;

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

    public Movement SetRotationSpeed(float rotSpeed)
    {
        _rotationSpeed = rotSpeed;
        return this;
    }

    public void MoveTank(Vector3 input, Transform playerTransform)
    {
        if (playerBody == null) return;

        Vector3 moveDir = playerTransform.forward * input.z * _speed;
        playerBody.velocity = new Vector3(moveDir.x, playerBody.velocity.y, moveDir.z);

        if (Mathf.Abs(input.x) > 0.01f)
        {
            float rotation = input.x * _rotationSpeed * Time.deltaTime;
            playerTransform.Rotate(Vector3.up, rotation);
        }
    }
}