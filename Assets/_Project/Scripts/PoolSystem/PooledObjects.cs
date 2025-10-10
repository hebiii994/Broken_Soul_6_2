using System.Collections;
using UnityEngine;


public class PooledObjects : MonoBehaviour
{
    public PoolScriptableObject poolSO;

    protected virtual void OnEnable()
    {
        if (poolSO != null && poolSO.time > 0)
        {
            StartCoroutine(Timer());
        }
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(poolSO.time);
        PoolManager.Instance.ReturnPooledObject(this);
    }
}
