using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    public bool JumpInput { get; private set; }
    public bool AttackInput { get; private set; }
    public bool SlideInput { get; private set; }
    public bool InteractInput { get; private set; }


    private PlayerControls _playerControls;

    private void Awake()
    {
        _playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerControls.Player.Enable();
        _playerControls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        _playerControls.Player.Move.canceled += ctx => MoveInput = Vector2.zero;
        _playerControls.Player.Run.performed += ctx => IsRunning = true;
        _playerControls.Player.Run.canceled += ctx => IsRunning = false;

        _playerControls.Player.Jump.performed += ctx => JumpInput = true;
        _playerControls.Player.Attack.performed += ctx => AttackInput = true;

        _playerControls.Player.Slide.performed += ctx => SlideInput = true;
        _playerControls.Player.Slide.canceled += ctx => SlideInput = false;

        _playerControls.Player.Interact.performed += ctx => InteractInput = true;
    }

    private void OnDisable()
    {
        _playerControls.Player.Disable();
    }
    private void LateUpdate()
    {
        JumpInput = false;
        AttackInput = false;
        InteractInput = false;
    }
}