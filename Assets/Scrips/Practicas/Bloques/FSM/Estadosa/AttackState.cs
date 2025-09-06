using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    public Transform _myRoot;
    public float _atackRange = 0.5f;

    public override void OnEnter()
    {
        Debug.Log("entre a AttackState ");
    }

    public override void OnUpdate()
    {
        if (_myRoot == null) return;

       float distance = Vector3.Distance(_myRoot.position, Target.Position);

        if (distance < _atackRange ) 
        {
            Debug.Log("LOGICA DE ATACK");
        }

        if (distance > _atackRange)
        {
            Debug.Log("Target fuera de rango, volver a Chase");
            fsm.ChnageState(EnemyStates.Chase);
        }

        Debug.Log("if lo tengo en rango y no hay vida me escapo UTILIZO EVADE Y un rango que al alejarme cambie de estado // un estado de buscar curacion y ese vuelve a patrol");

        Debug.Log("ELse if se me fue del rango ataque vuevlo a chase"); //fsm.ChnageState(EnemyStates.Chase);

    }

    public override void OnExit()
    {
        Debug.Log("sali de AttackState");
    }

    public void SetRoot(Transform newroot)
    {
        _myRoot = newroot;
    }
}
