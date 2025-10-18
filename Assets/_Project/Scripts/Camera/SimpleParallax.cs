using UnityEngine;

public class SimpleParallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("Determina quanto velocemente questo strato si muove rispetto alla telecamera. " +
             "Valori < 1: si muove più lentamente (sfondo). " +
             "Valori > 1: si muove più velocemente (primo piano, foreground). " +
             "Valore = 1: si muove esattamente con la telecamera (come il gameplay layer).")]
    [Range(-2f, 2f)] 
    public float parallaxEffectMultiplier = 0.5f;

    [Tooltip("Se spuntato, il movimento di parallasse si applica anche all'asse Y. " +
             "Deseleziona per parallasse solo orizzontale (tipico dei side-scroller).")]
    public bool applyToYAxis = false;

    private Camera mainCamera;
    private Vector3 lastCameraPosition;
    private float startZ;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("SimpleParallax: Main Camera non trovata! Assicurati che la tua telecamera abbia il tag 'MainCamera'.", this);
            enabled = false; 
            return;
        }

        lastCameraPosition = mainCamera.transform.position;
        startZ = transform.position.z; 
    }

    void LateUpdate()
    {

        if (mainCamera == null || !mainCamera.gameObject.activeInHierarchy)
        {
            return;
        }

        Vector3 cameraDeltaMovement = mainCamera.transform.position - lastCameraPosition;


        Vector3 parallaxMovement = new Vector3(
            cameraDeltaMovement.x * parallaxEffectMultiplier,
            applyToYAxis ? (cameraDeltaMovement.y * parallaxEffectMultiplier) : 0f, 
            0f 
        );


        transform.position += parallaxMovement;

        lastCameraPosition = mainCamera.transform.position;
    }
}