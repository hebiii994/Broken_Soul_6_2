using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerInputController InputController { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerAnimator PlayerAnimator { get; private set; }
    public PlayerCombat PlayerCombat { get; private set; }

    private PlayerState _currentState;

    private void Awake()
    {
        InputController = GetComponent<PlayerInputController>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerAnimator = GetComponent<PlayerAnimator>();
    }

    private void Start()
    {
        ChangeState(new PlayerIdleState(this));
    }

    private void Update()
    {
        _currentState?.Update();
    }

    private void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }

    public void ChangeState(PlayerState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
}