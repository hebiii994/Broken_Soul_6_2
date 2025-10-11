using UnityEngine;

public class DodgeDetector : MonoBehaviour
{
    private BossAI_Colloquio _bossAI;

    private void Awake()
    {
        _bossAI = GetComponentInParent<BossAI_Colloquio>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<PlayerStateMachine>(out var playerStateMachine))
        {
            float distanceToBoss = Vector2.Distance(_bossAI.transform.position, other.transform.position);

            if (distanceToBoss <= _bossAI.DodgeTriggerRange)
            {
                if (playerStateMachine.CurrentState is IPlayerAirborneState || playerStateMachine.CurrentState is PlayerAttackState)
                {
                    _bossAI.TryToDodge();
                }
            }
        }
    }
}