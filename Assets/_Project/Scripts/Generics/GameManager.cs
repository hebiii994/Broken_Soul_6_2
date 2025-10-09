using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action OnGameReady;

    public bool IsGameReady { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }

    public void SetGameReady()
    {
        IsGameReady = true;
        Debug.Log("GAME MANAGER: Il gioco è pronto. Avvio dei sistemi.");
        OnGameReady?.Invoke();
    }
}