using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private PlayerMovement _playerMovement;
    private PlayerInputController _inputController;

    private Vector2 _lastMoveDirection;
    public Vector2 GetLastMoveDirection() => _lastMoveDirection;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _inputController = GetComponent<PlayerInputController>();
        _playerMovement = GetComponent<PlayerMovement>();
        _lastMoveDirection = Vector2.down;
    }

    public void UpdateAnimationParameters()
    {
        Vector2 currentMoveInput = _inputController.MoveInput;
        bool isMoving = currentMoveInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            _lastMoveDirection = currentMoveInput.normalized;
        }

        _animator.SetFloat("MoveX", currentMoveInput.x);
        _animator.SetFloat("MoveY", currentMoveInput.y);
        _animator.SetBool("IsMoving", isMoving);
        _animator.SetBool("IsRunning", _inputController.IsRunning);
        _animator.SetBool("IsSliding", _inputController.SlideInput);
        _animator.SetFloat("IdleDirX", _lastMoveDirection.x);
        _animator.SetFloat("IdleDirY", _lastMoveDirection.y);
        _animator.SetFloat("SlideDirX", _lastMoveDirection.x);
        _animator.SetFloat("SlideDirY", _lastMoveDirection.y);
        _animator.SetBool("IsGrounded", _playerMovement.IsGrounded);
    }
    public void PlayAttackAnimation(Vector2 attackDirection)
    {
        _animator.SetFloat("AttackDirX", attackDirection.x);
        _animator.SetFloat("AttackDirY", attackDirection.y);
        _animator.SetTrigger("Attack");
    }

    public void PlayJumpAnimation()
    {
        _animator.SetTrigger("Jump");
    }
}
  