using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Movement movimiento;
    private Vector3 playerMovementInput;

    [SerializeField] Controller inputManager;
    private View view;

    [SerializeField] private Rigidbody PlayerBody;
    [SerializeField] private Animator _animator;
    [SerializeField] private float Speed = 5f;

    private void Awake()
    {
        movimiento = new Movement()
            .SetPlayerBody(PlayerBody)
            .SetPlayerSpeed(Speed);

        view = new View().SetAnimator(_animator);

        inputManager.OnMove += Move;
    }

    void Move(float dirHorizontal, float dirVertical)
    {
        playerMovementInput = new Vector3(dirHorizontal, 0f, dirVertical);

        movimiento.Move(playerMovementInput);
    }
}

