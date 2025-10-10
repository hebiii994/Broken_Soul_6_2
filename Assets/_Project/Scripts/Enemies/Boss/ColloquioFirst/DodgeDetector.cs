using UnityEngine;

public class DodgeDetector : MonoBehaviour
{
    public BossAI_Colloquio BossAI_Colloquio;
    public bool CanDodge { get; private set; } = false;

    private void Awake()
    {
        if (BossAI_Colloquio == null)
        {
            BossAI_Colloquio = GetComponentInParent<BossAI_Colloquio>();
        }
        Debug.Log("DODGE DETECTOR PRONTO");
    }
    private void OnTriggerEnter2D(Collider2D other)

    {

        if (other.CompareTag("Player") && other.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine playerStateMachine))
        {
            float distanza = Vector2.Distance(transform.position, other.transform.position);
            if (playerStateMachine.IsJumping && distanza < 8f)
            {
                CanDodge = true;
                Debug.Log("CanDodge!");
                StartCoroutine(BossAI_Colloquio.DodgeRoutine());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine playerStateMachine))
        {
            CanDodge = false;
        }
    }
}
