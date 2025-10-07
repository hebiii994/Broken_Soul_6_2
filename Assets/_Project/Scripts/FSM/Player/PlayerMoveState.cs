using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Update()
    {

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
        playerMovement.ApplyMovement();
    }
}