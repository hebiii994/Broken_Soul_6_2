using UnityEngine;

public class VHSCollectible : MonoBehaviour
{
    [SerializeField] private int _maxHealthIncreaseAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<HealthSystem>(out HealthSystem healthSystem))
        {
            healthSystem.IncreaseMaxHealth(_maxHealthIncreaseAmount);
            Destroy(gameObject);
        }
    }
}