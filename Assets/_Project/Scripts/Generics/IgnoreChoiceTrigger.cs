using UnityEngine;

public class IgnoreChoiceTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            NarrativeManager.Instance.OnPlayerIgnoredChoice();
            gameObject.SetActive(false);
        }
    }
}