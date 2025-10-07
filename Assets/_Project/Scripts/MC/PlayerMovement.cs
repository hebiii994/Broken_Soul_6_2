using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _runSpeed = 8f;
    [SerializeField] private float _jumpForce = 7f;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;

    public bool IsGrounded { get; private set; }

    private Rigidbody2D _rb;
    private PlayerInputController _inputController;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _inputController = GetComponent<PlayerInputController>();
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
    }

    public void ApplyMovement()
    {
        float currentSpeed = _inputController.IsRunning ? _runSpeed : _moveSpeed;
        _rb.linearVelocity = new Vector2(_inputController.MoveInput.x * currentSpeed, _rb.linearVelocity.y);
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
        }
    }

    private void CheckIfGrounded()
    {
        IsGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _groundLayer);
    }

    public void ResetInput()
    {

        _rb.linearVelocity = Vector2.zero;
    }
}