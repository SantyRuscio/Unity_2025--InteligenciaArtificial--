using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigos : MonoBehaviour
{
   private BloquesFsm _bloquesFsm;

    private void Awake()
    {
        _bloquesFsm = new BloquesFsm();
        _bloquesFsm._possibleStates.Add(EnemyStates.Idle, new IdleState().SetUp(_bloquesFsm));
        _bloquesFsm._possibleStates.Add(EnemyStates.Chase, new ChaseState().SetUp(_bloquesFsm));
        _bloquesFsm._possibleStates.Add(EnemyStates.Patrol, new PatrolState().SetUp(_bloquesFsm));

        _bloquesFsm.ChnageState(EnemyStates.Idle);

    }

    void Update()
    {
        _bloquesFsm.OnUpdate();
    }
}