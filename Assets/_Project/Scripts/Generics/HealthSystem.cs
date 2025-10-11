using UnityEngine;
using System;
using System.Collections;

public class HealthSystem : MonoBehaviour, ISaveable, IDamageable
{
    public event Action<int, int> OnHealthChanged;

    [Header("Damage Flash Settings")]
    [SerializeField] private SpriteRenderer _spriteRenderer; 
    [SerializeField] private Color _damageFlashColor = Color.red;
    [SerializeField] private float _damageFlashDuration = 0.1f;

    [SerializeField] private CharacterStats _characterStats;
    [SerializeField] private bool _shouldBeSaved = true;

    private int _currentHealth;
    private int _maxHealth;
    private IDeathHandler _deathHandler;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public void LoadData(GameData data)
    {
        if (!_shouldBeSaved) return;
        if (data.playerMaxHealth > 0)
        {
            this._maxHealth = data.playerMaxHealth;
            this._currentHealth = this._maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (!_shouldBeSaved) return;
        data.playerMaxHealth = this._maxHealth;
    }
    private void Awake()
    {
        _deathHandler = GetComponent<IDeathHandler>();
        if (_deathHandler == null) Debug.LogError("IDeathHandler mancante!", this);
        if (_characterStats != null)
            {
                _maxHealth = _characterStats.maxHealth;
                _currentHealth = _maxHealth;
            }

        }

    private void Start()
    {
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        if (damageAmount < 0) return;
        _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        StartCoroutine(DamageFlashRoutine());

        if (_currentHealth == 0)
        {
            _deathHandler?.HandleDeath();
        }
    }

    public void Heal(int healAmount)
    {
        if (healAmount < 0) return;
        _currentHealth = Mathf.Min(_currentHealth + healAmount, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void IncreaseMaxHealth(int amount)
    {
        if (amount <= 0) return;
        _maxHealth += amount;
        _currentHealth = _maxHealth;
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (_spriteRenderer == null) yield break;
        _spriteRenderer.color = _damageFlashColor;
        yield return new WaitForSeconds(_damageFlashDuration);
        _spriteRenderer.color = Color.white;
    }
}