using UnityEngine;

public enum PrefabType
{
    BossProjectile
}
[CreateAssetMenu(fileName = "NewProjectileStats", menuName = "Broken Soul/Data/PoolSO")]
public class PoolScriptableObject : ScriptableObject
{

    public PrefabType type;
    public PooledObjects Prefab;
    public bool collectionCheck = false;
    public int size = 10;
    public int maxSize = 20;
    public float time = 5f; // Tempo di vita di default

    public PooledObjects Create() => Instantiate(Prefab);
    public void OnTakeFromPool(PooledObjects gO) => gO.gameObject.SetActive(true);
    public void OnReturnToPool(PooledObjects gO) => gO.gameObject.SetActive(false);
    public void OnDestroyPooledObject(PooledObjects gO) => Destroy(gO.gameObject);
}