using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        playerMovement.Jump();
        playerAnimator.PlayJumpAnimation();
    }

    public override void Update()
    {
        playerAnimator.UpdateAnimationParameters();

        if (playerMovement.IsGrounded)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        playerMovement.ApplyMovement();
    }
}