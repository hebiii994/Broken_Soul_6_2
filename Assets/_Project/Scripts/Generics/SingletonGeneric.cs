using UnityEngine;

public abstract class SingletonGeneric<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError($"L'istanza del Singleton di tipo {typeof(T)} è stata richiesta, ma non esiste nella scena o non è ancora stata inizializzata. Assicurati che un oggetto con questo script sia presente, attivo e che il suo ordine di esecuzione sia corretto.");
            }
            return _instance;
        }
    }

    protected abstract bool ShouldBeDestroyOnLoad();

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;


            if (!ShouldBeDestroyOnLoad())
            {
                DontDestroyOnLoad(gameObject);
                Debug.Log($"-- Singleton {typeof(T)} inserito in DontDestroyOnLoad --");
            }
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"Esiste già un'istanza del Singleton di tipo {typeof(T)}. Distruzione di questa istanza duplicata.", this.gameObject);
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this as T)
        {
            _instance = null;
        }
    }
}