using System.Collections;
using UnityEngine;

public class KnockbackHandler : MonoBehaviour
{
    [SerializeField] private CharacterStats _stats;
    [SerializeField] private float _knockbackDuration = 0.2f;

    private Rigidbody2D _rb;
    private Coroutine _knockbackCoroutine;
    private PlayerStateMachine _stateMachine;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _stateMachine = GetComponent<PlayerStateMachine>();
    }

    public void ApplyKnockback(Transform damageSource)
    {
        if (_stateMachine.IsKnockedBack) return;

        if (_knockbackCoroutine != null) StopCoroutine(_knockbackCoroutine);
        _knockbackCoroutine = StartCoroutine(KnockbackRoutine(damageSource));
    }

    private IEnumerator KnockbackRoutine(Transform damageSource)
    {
        _stateMachine.IsKnockedBack = true;

        Vector2 direction = (transform.position - damageSource.position).normalized;
        direction.y += 0.2f;

        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(direction.normalized * _stats.knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(_knockbackDuration);

        _rb.linearVelocity = Vector2.zero;

        _stateMachine.IsKnockedBack = false;
        _knockbackCoroutine = null;
    }
}