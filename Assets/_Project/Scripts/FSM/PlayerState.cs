using UnityEngine;

public abstract class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected PlayerInputController inputController;
    protected PlayerMovement playerMovement;
    protected PlayerAnimator playerAnimator;
    protected PlayerCombat playerCombat;

    public PlayerState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.inputController = stateMachine.InputController;
        this.playerMovement = stateMachine.PlayerMovement;
        this.playerAnimator = stateMachine.PlayerAnimator;
        this.playerCombat = stateMachine.PlayerCombat;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}