using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View
{
    private Animator _animator;

    public View SetAnimator(Animator animator)
    {
        _animator = animator;
        return this;
    }

    public void Forward()
    {
        _animator.SetBool("isWalking", true); 
    }
}
