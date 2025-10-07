using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour, IDeathHandler
{
    public void HandleDeath()
    {
        Debug.Log(gameObject.name + " � stato sconfitto.");
        Destroy(gameObject);
    }
}
