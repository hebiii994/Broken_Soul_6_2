using UnityEngine;

public class SecretEndingTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            NarrativeManager.Instance.TriggerSecretEnding();
            gameObject.SetActive(false);
        }
    }
}