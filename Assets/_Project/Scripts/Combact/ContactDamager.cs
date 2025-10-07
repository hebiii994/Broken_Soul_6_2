using UnityEngine;

public class ContactDamager : MonoBehaviour
{
    [SerializeField] private CharacterStats _characterStats;
    [SerializeField] private float _damageCooldown = 1f;
    private float _lastDamageTime;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < _lastDamageTime + _damageCooldown)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            _lastDamageTime = Time.time;

            if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable playerDamageable))
            {
                playerDamageable.TakeDamage(_characterStats.attackDamage);
            }

            if (collision.gameObject.TryGetComponent<KnockbackHandler>(out KnockbackHandler playerKnockback))
            {
                playerKnockback.ApplyKnockback(transform);
            }
        }
    }
}