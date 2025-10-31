using UnityEngine;
using UnityEngine.InputSystem; 

public class GameExitHandler : SingletonGeneric<GameExitHandler>
{

    protected override bool ShouldBeDestroyOnLoad() => false;

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            DoQuitGame();
        }
    }

    public void DoQuitGame()
    {
        Debug.Log("Richiesta di chiusura del gioco...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Se siamo in una build, chiudi l'applicazione
        Application.Quit();
#endif
    }
}
