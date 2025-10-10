using System.Collections.Generic;
using UnityEngine.Pool;

public class PoolManager : SingletonGeneric<PoolManager>
{
    protected override bool ShouldBeDestroyOnLoad() => false;

    private Dictionary<PrefabType, IObjectPool<PooledObjects>> _allPools = new Dictionary<PrefabType, IObjectPool<PooledObjects>>();

    private IObjectPool<PooledObjects> GetPool(PoolScriptableObject so)
    {
        if (_allPools.TryGetValue(so.type, out var pool))
        {
            return pool;
        }

        pool = new ObjectPool<PooledObjects>(
            so.Create,
            so.OnTakeFromPool,
            so.OnReturnToPool,
            so.OnDestroyPooledObject,
            so.collectionCheck,
            so.size,
            so.maxSize);

        _allPools.Add(so.type, pool);
        return pool;
    }

    public PooledObjects GetPooledObject(PoolScriptableObject so)
    {
        PooledObjects obj = GetPool(so).Get();

        obj.poolSO = so;

        return obj;
    }

    public void ReturnPooledObject(PooledObjects pooledObject)
    {
        GetPool(pooledObject.poolSO).Release(pooledObject);
    }
}
