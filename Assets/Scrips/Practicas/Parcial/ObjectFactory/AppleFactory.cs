using Patterns.combined_Factory_Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleFactory : FactoryGeneric<Apples>
{
    public Apples[] prefabs;
    private ObjectPool<Apples> _pool;

    [SerializeField]
    private int _initialAmount = 6;

    private void Awake()
    {
        // Creo el pool con los métodos que necesita (en el otro le pedi 4 cosas asi que le madno esas 4)
        _pool = new ObjectPool<Apples>(
            CreatePrefab,
            InitilizeNewObject,
            DeactivateNewObject,
            _initialAmount
        );
    }

    public override Apples Create()
    {
        var x = _pool.GetObject();
        return x;
    }

    Apples CreatePrefab()
    {
        var prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
        Apples b = Instantiate(prefab);
        return b;
    }

    private void InitilizeNewObject(Apples lvl)
    {
        lvl.Initialize(this);
    }

    private void DeactivateNewObject(Apples lvl)
    {
        lvl.ResetObject();
    }

    //Obtiene un objeto del pool
    public Apples GetLevel()
    {
        return _pool.GetObject();
    }

    public override void ReleaseLevel(Apples level)
    {
        _pool.ReturnObjectToPool(level);

    }

}
