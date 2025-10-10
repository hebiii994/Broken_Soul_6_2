using UnityEngine;

public class BossProjectile : PooledObjects
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private int _damage = 1;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction)
    {
        _rb.linearVelocity = direction.normalized * _speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<IDamageable>(out var player))
        {
            player.TakeDamage(_damage);
        }

        if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            PoolManager.Instance.ReturnPooledObject(this);
        }
    }
}