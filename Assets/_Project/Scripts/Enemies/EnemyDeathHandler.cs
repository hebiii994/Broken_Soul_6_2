using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour, IDeathHandler
{
    public void HandleDeath()
    {
        Debug.Log(gameObject.name + " è stato sconfitto.");
        Destroy(gameObject);
    }
}
