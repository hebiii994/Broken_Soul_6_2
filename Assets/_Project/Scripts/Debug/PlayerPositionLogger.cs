using UnityEngine;

public class PlayerPositionLogger : MonoBehaviour
{
    private Vector3 _lastPosition;
    private Vector3 _positionAtEndOfFrame;
    private Rigidbody2D _rb;

    void Start()
    {
        _lastPosition = transform.position;
        _positionAtEndOfFrame = transform.position;
        _rb = GetComponent<Rigidbody2D>();

        if (_rb == null)
        {
            Debug.LogError("PlayerPositionLogger: Rigidbody2D non trovato sul Player.");
            enabled = false;
        }
        Debug.Log($"[PlayerPositionLogger] Player inizializzato a: {transform.position}");
    }

    void FixedUpdate()
    {
        if (transform.position != _lastPosition)
        {
            if (Vector3.Distance(transform.position, _positionAtEndOfFrame) > 1.0f)
            {
                Debug.LogError($"<color=red>TELETRASPORTO RILEVATO!</color> Posizione a fine frame precedente: {_positionAtEndOfFrame}. Posizione attuale in FixedUpdate: {transform.position}");
                Debug.LogError($"<color=yellow>STACK TRACE:</color>\n{System.Environment.StackTrace}");

            }

            _lastPosition = transform.position;
        }
    }
    void LateUpdate()
    {
        // Memorizza la posizione del giocatore alla fine di tutti gli Update.
        _positionAtEndOfFrame = transform.position;
    }
}