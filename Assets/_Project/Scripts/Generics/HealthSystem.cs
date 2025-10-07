using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour, ISaveable, IDamageable
{
    public event Action<int, int> OnHealthChanged;

    [SerializeField] private CharacterStats _characterStats;

    private int _currentHealth;
    private int _maxHealth;
    private IDeathHandler _deathHandler;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public void LoadData(GameData data)
    {
        this._maxHealth = data.playerMaxHealth;
        this._currentHealth = this._maxHealth;
    }

    public void SaveData(ref GameData data)
    {
        data.playerMaxHealth = this._maxHealth;
    }
    private void Awake()
    {
        _deathHandler = GetComponent<IDeathHandler>();
        if (_deathHandler == null) Debug.LogError("IDeathHandler mancante!", this);

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
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }
}