using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    private int _damageAmount;
    private Transform _damageSource;

    public void SetDamage(int damage, Transform source)
    {
        _damageAmount = damage;
        _damageSource = source;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(_damageAmount);
        }
        if (_damageSource.TryGetComponent<KnockbackHandler>(out KnockbackHandler playerKnockback))
        {
            if (GetComponent<PogoEffector>() != null)
            {
                return;
            }
            playerKnockback.ApplyKnockback(other.transform);
        }
    }
}