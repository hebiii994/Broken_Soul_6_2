using UnityEngine;
using System;
using System.Collections;

public class PlayerDeathHandler : MonoBehaviour, IDeathHandler
{
    public static event Action OnPlayerDied;

    [SerializeField] private float _deathAnimationDuration = 2.5f;

    private PlayerStateMachine _psm;

    private void Awake()
    {
        _psm = GetComponent<PlayerStateMachine>();
    }
    public void HandleDeath()
    {
        Debug.Log("Il giocatore è morto! Avvio sequenza di Game Over...");
        if (_psm != null)
        {
            _psm.enabled = false;
        }
        if (ScreenFader.Instance != null)
        {
            ScreenFader.Instance.FadeOut(1.2f);
        }
        OnPlayerDied?.Invoke();
        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        yield return new WaitForSeconds(_deathAnimationDuration);

    }
}