using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats _characterStats;

    [Header("Hitboxes")]
    [SerializeField] private GameObject _rightHitbox;
    [SerializeField] private GameObject _leftHitbox;
    [SerializeField] private GameObject _upHitbox;
    [SerializeField] private GameObject _downHitbox;

    [Header("Settings")]
    [SerializeField] private float _attackDuration = 0.2f;
    public float AttackDuration => _attackDuration;

    // Riferimenti ai servizi
    private PlayerAnimator _playerAnimator;
    private PlayerInputController _inputController;
    private PlayerMovement _playerMovement;
    private Rigidbody2D _rb;
    private bool _isAttacking = false;

    private void Awake()
    {
        _playerAnimator = GetComponent<PlayerAnimator>();
        _inputController = GetComponent<PlayerInputController>();
        _playerMovement = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();

        SetupHitboxes();
    }

    private void SetupHitboxes()
    {
        _rightHitbox.GetComponent<DamageDealer>().SetDamage(_characterStats.attackDamage, transform);
        _leftHitbox.GetComponent<DamageDealer>().SetDamage(_characterStats.attackDamage, transform);
        _upHitbox.GetComponent<DamageDealer>().SetDamage(_characterStats.attackDamage, transform);
        _downHitbox.GetComponent<DamageDealer>().SetDamage(_characterStats.attackDamage, transform);
        _downHitbox.GetComponent<PogoEffector>().SetPlayerRigidbody(_rb);

        _rightHitbox.SetActive(false);
        _leftHitbox.SetActive(false);
        _upHitbox.SetActive(false);
        _downHitbox.SetActive(false);
    }

    public void HandleAttack()
    {
        if (_isAttacking) return;

        Vector2 moveInput = _inputController.MoveInput;
        GameObject activeHitbox = null;
        Vector2 animationDirection = Vector2.zero;
        

        if (moveInput.y > 0.1f)
        {
            activeHitbox = _upHitbox;
            animationDirection = Vector2.up;
        }
        else if (moveInput.y < -0.1f && !_playerMovement.IsGrounded)
        {
            activeHitbox = _downHitbox;
            animationDirection = Vector2.down;
        }
        else
        {
            if (moveInput.x > 0.1f)
            {
                activeHitbox = _rightHitbox;
                animationDirection = Vector2.right;
            }
            else if (moveInput.x < -0.1f)
            {
                activeHitbox = _leftHitbox;
                animationDirection = Vector2.left;
            }
            else
            {
                Vector2 lastMoveDirection = _playerAnimator.GetLastMoveDirection();
                if (lastMoveDirection.x >= 0)
                {
                    activeHitbox = _rightHitbox;
                    animationDirection = Vector2.right;
                }
                else
                {
                    activeHitbox = _leftHitbox;
                    animationDirection = Vector2.left;
                }
            }
            
        }

        if (activeHitbox != null)
        {
            _playerAnimator.PlayAttackAnimation(animationDirection);
            StartCoroutine(ActivateHitboxRoutine(activeHitbox));
        }
    }

    private IEnumerator ActivateHitboxRoutine(GameObject hitbox)
    {
        _isAttacking = true;
        hitbox.SetActive(true);
        yield return new WaitForSeconds(_attackDuration);
        hitbox.SetActive(false);
        _isAttacking = false;
    }
}
