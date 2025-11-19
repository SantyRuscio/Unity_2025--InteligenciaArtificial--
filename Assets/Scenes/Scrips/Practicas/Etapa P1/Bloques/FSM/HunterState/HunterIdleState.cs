using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HunterIdleState : BaseState
{
    private Animator _animator;
    private HunterlLife _myLife;
    private Transform[] _wayPoints;

    private Transform _healPoint;
    private float _healSpeed = 5f;    
    private float _healAmount= 100f;     
    private float _arriveRange = 0.5f; 

    public HunterIdleState(HunterlLife myLife, Transform[] wayPoints)
    {
        _myLife = myLife;
        _wayPoints = wayPoints;

        if (_wayPoints != null && _wayPoints.Length > 0)
        {
            _healPoint = _wayPoints[0];
        }
    }

    public override void OnEnter()
    {
        Debug.Log("Entré a Idle");

        if (_animator == null && _myRoot != null)
            _animator = _myRoot.GetComponentInChildren<Animator>();

        if (_animator != null)
            _animator.SetBool("Walk", true);
    }

    public override void OnUpdate()
    {
        if (_myLife == null) return;

        if (_myLife._currentLife <= 50f && _healPoint != null)
        {
            Debug.Log("TENGO POCA VIDA");

            Vector3 dirToHeal = _healPoint.position - _myRoot.position;
            float distance = dirToHeal.magnitude;

            if (distance > _arriveRange)
            {
                _myRoot.position += dirToHeal.normalized * _healSpeed * Time.deltaTime;
            }
            else
            {
                Debug.Log("ENTRE A CURA");
                _myLife.Heal(_healAmount);
            }
        }

        if (_myLife._currentLife >= 60f)
        {
            fsm.ChnageState(AgentStates.Patrol);
        }
    }

    public override void OnExit()
    {
        Debug.Log("Salí de Idle");

        if (_animator != null)
            _animator.SetBool("Walk", false);
    }
}