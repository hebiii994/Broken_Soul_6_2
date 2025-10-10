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
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_damage);
        }

        PoolManager.Instance.ReturnPooledObject(this);
    }
}