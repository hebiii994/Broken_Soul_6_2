using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

public class EndingTrigger : MonoBehaviour
{
    [Header("Riferimenti UI")]
    [Tooltip("L'oggetto di testo 'To Be Continued' che deve apparire")]
    [SerializeField] private GameObject _toBeContinuedText;

    [Header("Impostazioni di Flusso")]
    [Tooltip("Nome esatto della scena del Menu Principale")]
    [SerializeField] private string _mainMenuSceneName = "MainMenu";

    [Tooltip("Tempo (in secondi) in cui il testo resta a schermo prima del fade")]
    [SerializeField] private float _delayBeforeFade = 4.0f;

    [Tooltip("Durata (in secondi) del fade-out")]
    [SerializeField] private float _fadeDuration = 2.0f;

    private bool _isTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_isTriggered)
        {
            _isTriggered = true;
            StartCoroutine(EndingSequenceRoutine());
        }
    }

    private IEnumerator EndingSequenceRoutine()
    {
        PlayerInputController playerInput = FindFirstObjectByType<PlayerInputController>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        PlayerStateMachine psm = FindFirstObjectByType<PlayerStateMachine>();
        if (psm != null)
        {
            psm.enabled = false; 
            if (psm.PlayerMovement != null)
            {
                psm.PlayerMovement.StopMovement(); 
            }
        }

        if (_toBeContinuedText != null)
        {
            _toBeContinuedText.SetActive(true);
        }


        yield return new WaitForSeconds(_delayBeforeFade);

        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeOut(_fadeDuration);
        }
        else
        {
            Debug.LogWarning("ScreenFader.Instance non trovato!");
            yield return new WaitForSeconds(_fadeDuration);
        }

        SceneManager.LoadScene(_mainMenuSceneName);
    }
}