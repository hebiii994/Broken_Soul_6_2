using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour, IDeathHandler
{
    public void HandleDeath()
    {
        // - Disabilitare il controllo del personaggio
        // - Avviare l'animazione di morte
        // - Mostrare la schermata di Game Over
        Debug.Log("Il giocatore è morto! Avvio sequenza di Game Over...");
        GetComponent<PlayerInputController>().enabled = false; 
    }
}