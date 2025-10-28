using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _runSpeed = 8f;
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _slideSpeed = 10f;

    public float MoveSpeed => _moveSpeed;
    public float RunSpeed => _runSpeed;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Ledge Check")]
    [SerializeField] private Transform _wallCheckPoint;
    [SerializeField] private Transform _ledgeCheckPoint;
    [SerializeField] private float _wallCheckDistance = 0.5f;
    [SerializeField] private float _horizontalLedgeGrabOffset = 0.3f;
    public Transform LedgeGrabPositionMarker;

    public Transform WallCheckPoint => _wallCheckPoint;
    public Transform PlayerTransform => transform;
    public RaycastHit2D WallCheckHit { get; private set; }

    public bool IsTouchingWall { get; private set; }
    public bool IsNearLedge { get; private set; }
    public bool IsGrounded { get; private set; }
    public int FacingDirection { get; private set; }
    public bool GroundCheckEnabled { get; private set; } = true;
    public Rigidbody2D Rigidbody => _rb;

    private Rigidbody2D _rb;
    private PlayerInputController _inputController;
    private PlayerAnimator _playerAnimator;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _inputController = GetComponent<PlayerInputController>();
        _playerAnimator = GetComponent<PlayerAnimator>();
        FacingDirection = 1;
    }

    private void Update()
    {
        if (_inputController.MoveInput.x > 0.1f)
        {
            FacingDirection = 1;
        }
        else if (_inputController.MoveInput.x < -0.1f)
        {
            FacingDirection = -1;
        }
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
        CheckLedgeAndWall();
    }

    public void SetHorizontalVelocity(float velocity)
    {
        Vector2 newVelocity = new Vector2(velocity, _rb.linearVelocity.y);
        if (_rb.linearVelocity != newVelocity) 
        {
            _rb.linearVelocity = newVelocity;
            //Debug.Log($"[PlayerMovement] Velocità impostata a {_rb.linearVelocity}");
        }
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            Vector2 oldVelocity = _rb.linearVelocity;
            _rb.linearVelocity = new Vector2(oldVelocity.x, 0);
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
             Debug.Log($"[PlayerMovement] Saltato. Nuova velocità: {_rb.linearVelocity}");
        }
    }
    public void ApplySlide()
    {
        float slideDirection = _playerAnimator.GetLastMoveDirection().x > 0 ? 1 : -1;
        _rb.linearVelocity = new Vector2(slideDirection * _slideSpeed, _rb.linearVelocity.y);
    }
    public void SetGroundCheckEnabled(bool enabled)
    {
        GroundCheckEnabled = enabled;
    }
    private void CheckIfGrounded()
    {
        if (!GroundCheckEnabled) 
        {
            IsGrounded = false;
            return;
        }

        IsGrounded = Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _groundLayer);
    }
    public void StopMovement()
    {
        _rb.linearVelocity = Vector2.zero;
    }
    public void ResetInput()
    {

        _rb.linearVelocity = Vector2.zero;
    }

    private void CheckLedgeAndWall()
    {
        RaycastHit2D previousWallHit = WallCheckHit;
        WallCheckHit = Physics2D.Raycast(_wallCheckPoint.position, Vector2.right * FacingDirection, _wallCheckDistance, _groundLayer);

        if (previousWallHit && WallCheckHit && Vector2.Distance(previousWallHit.point, WallCheckHit.point) > 0.1f)
        {
            Debug.LogWarning($"[CheckLedgeAndWall] WallCheckHit cambiato! Prima: {previousWallHit.point}, Ora: {WallCheckHit.point}");
        }
        IsTouchingWall = WallCheckHit; 
        IsNearLedge = IsTouchingWall && !Physics2D.Raycast(_ledgeCheckPoint.position, Vector2.right * FacingDirection, _wallCheckDistance, _groundLayer);
        if (IsNearLedge && IsTouchingWall)
        {
            Debug.Log($"[CheckLedgeAndWall] LEDGE DISPONIBILE! IsNearLedge: {IsNearLedge}, IsTouchingWall: {IsTouchingWall}, VelocityY: {_rb.linearVelocity.y}");
        }
    }

    private void OnDrawGizmos()
    {
        if (_wallCheckPoint != null)
        {
            Vector2 wallCheckStart = (Vector2)_wallCheckPoint.position + new Vector2(FacingDirection * _horizontalLedgeGrabOffset, 0);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheckStart, wallCheckStart + (Vector2.right * FacingDirection * _wallCheckDistance));
        }
        if (_ledgeCheckPoint != null)
        {
            Vector2 ledgeCheckStart = (Vector2)_ledgeCheckPoint.position + new Vector2(FacingDirection * _horizontalLedgeGrabOffset, 0);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(ledgeCheckStart, ledgeCheckStart + (Vector2.right * FacingDirection * _wallCheckDistance));
        }
        if (_groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
        }
    }

}