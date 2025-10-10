using System;
using UnityEngine;

public class GameManager : SingletonGeneric<GameManager>
{
    public static event Action OnGameReady;
    public bool IsGameReady { get; private set; } = false;

    protected override bool ShouldBeDestroyOnLoad() => false;

    public void SetGameReady()
    {
        IsGameReady = true;
        Debug.Log("GAME MANAGER: Il gioco è pronto. Avvio dei sistemi.");
        OnGameReady?.Invoke();
    }
}