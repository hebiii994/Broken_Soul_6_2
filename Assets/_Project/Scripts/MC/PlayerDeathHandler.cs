using UnityEngine;
using System;

public class PlayerDeathHandler : MonoBehaviour, IDeathHandler
{
    public static event Action OnPlayerDied;
    public void HandleDeath()
    {
        Debug.Log("Il giocatore è morto! Avvio sequenza di Game Over...");
        OnPlayerDied?.Invoke();
        //gameObject.SetActive(false);
    }
}