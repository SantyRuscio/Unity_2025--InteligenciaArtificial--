using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Action<float, float> OnMove;
    public Action OnClick;

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (OnMove != null)
        {
            OnMove(horizontal, vertical);
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnClick?.Invoke();
        }
    }
}
