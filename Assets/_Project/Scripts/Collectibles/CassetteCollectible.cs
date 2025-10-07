using UnityEngine;

public class CassetteCollectible : MonoBehaviour
{
    [SerializeField] private int _maxNostalgiaIncreaseAmount = 25;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<NostalgiaSystem>(out NostalgiaSystem nostalgiaSystem))
        {
            nostalgiaSystem.IncreaseMaxNostalgia(_maxNostalgiaIncreaseAmount);
            Destroy(gameObject);
        }
    }
}