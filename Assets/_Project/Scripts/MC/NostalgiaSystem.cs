using UnityEngine;
using System;

public class NostalgiaSystem : MonoBehaviour, ISaveable
{
    public event Action<int, int> OnNostalgiaChanged;

    [SerializeField] private CharacterStats _characterStats;

    private int _currentNostalgia;
    private int _maxNostalgia;

    public int CurrentNostalgia => _currentNostalgia;
    public int MaxNostalgia => _maxNostalgia;

    public void LoadData(GameData data)
    {
        this._maxNostalgia = data.playerMaxNostalgia;
        this._currentNostalgia = this._maxNostalgia;
    }

    public void SaveData(ref GameData data)
    {
        data.playerMaxNostalgia = this._maxNostalgia;
    }

    private void Start()
    {
        OnNostalgiaChanged?.Invoke(_currentNostalgia, _maxNostalgia);
    }

    public void AddNostalgia(int amount)
    {
        if (amount < 0) return;
        _currentNostalgia = Mathf.Min(_currentNostalgia + amount, _maxNostalgia);
        OnNostalgiaChanged?.Invoke(_currentNostalgia, _maxNostalgia);
    }

    public void LoseNostalgia(int amount)
    {
        if (amount < 0) return;
        _currentNostalgia = Mathf.Max(_currentNostalgia - amount, 0);
        OnNostalgiaChanged?.Invoke(_currentNostalgia, _maxNostalgia);
    }

    public void IncreaseMaxNostalgia(int amount)
    {
        if (amount <= 0) return;
        _maxNostalgia += amount;
        OnNostalgiaChanged?.Invoke(_currentNostalgia, _maxNostalgia);
    }
}