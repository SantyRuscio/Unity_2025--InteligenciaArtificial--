using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Movement movimiento;
    private Vector3 playerMovementInput;

    [SerializeField] Controller inputManager;

    [SerializeField] private Rigidbody PlayerBody;
    [SerializeField] private float Speed = 5f;

    private void Awake()
    {
        movimiento = new Movement()
            .SetPlayerBody(PlayerBody)
            .SetPlayerSpeed(Speed); ///ACA TENEMOS VALORES DEL REMOTE

        inputManager.OnMove += Move;
    }

    void Move(float dirHorizontal, float dirVertical)
    {
        playerMovementInput = new Vector3(dirHorizontal, 0f, dirVertical);
        movimiento.Move(playerMovementInput);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
