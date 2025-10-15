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

        if (inputController.AttackInput)
        {
            stateMachine.ChangeState(new PlayerAttackState(stateMachine));
            return;
        }

        if (playerMovement.Rigidbody.linearVelocity.y <= 0)
        {
            if (playerMovement.IsNearLedge && playerMovement.IsTouchingWall)
            {
                Debug.Log("[JumpState] Ledge rilevato! Transizione a LedgeGrabState");
                stateMachine.ChangeState(new PlayerLedgeGrabState(stateMachine));
                return;
            }

            stateMachine.ChangeState(new PlayerFallState(stateMachine));
            return;
        }

    }

    public override void FixedUpdate()
    {
        float currentSpeed = inputController.IsRunning ? playerMovement.RunSpeed : playerMovement.MoveSpeed;
        float moveVelocity = inputController.MoveInput.x * currentSpeed;
        playerMovement.SetHorizontalVelocity(moveVelocity);
    }

}