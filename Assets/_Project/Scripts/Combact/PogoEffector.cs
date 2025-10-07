using UnityEngine;

public class PogoEffector : MonoBehaviour
{
    [SerializeField] private float _pogoForce = 10f;
    private Rigidbody2D _playerRb;

    public void SetPlayerRigidbody(Rigidbody2D playerRb)
    {
        _playerRb = playerRb;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out _) && _playerRb != null)
        {

            _playerRb.linearVelocity = new Vector2(_playerRb.linearVelocity.x, 0); 
            _playerRb.AddForce(Vector2.up * _pogoForce, ForceMode2D.Impulse);
        }
    }
}