using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Update()
    {
        if (!playerMovement.IsGrounded)
        {
            stateMachine.ChangeState(new PlayerFallState(stateMachine));
            return;
        }

        if (inputController.SlideInput)
        {
            stateMachine.ChangeState(new PlayerSlideState(stateMachine));
            return;
        }
        if (inputController.AttackInput)
        {
            stateMachine.ChangeState(new PlayerAttackState(stateMachine));
            return;
        }

        if (inputController.JumpInput && playerMovement.IsGrounded)
        {
            stateMachine.ChangeState(new PlayerJumpState(stateMachine));
            return;
        }

        if (inputController.MoveInput.sqrMagnitude <= 0.01f)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine));
            return;
        }

        playerAnimator.UpdateAnimationParameters();
    }

    public override void FixedUpdate()
    {
        float currentSpeed = inputController.IsRunning ? playerMovement.RunSpeed : playerMovement.MoveSpeed;
        float moveVelocity = inputController.MoveInput.x * currentSpeed;
        playerMovement.SetHorizontalVelocity(moveVelocity);
    }
}