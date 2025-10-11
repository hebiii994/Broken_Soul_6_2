using System.Collections;
using UnityEngine;

public class PlayerJumpState : PlayerState, IPlayerAirborneState
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

        if (playerMovement.Rigidbody.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(new PlayerFallState(stateMachine));
            return;
        }

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