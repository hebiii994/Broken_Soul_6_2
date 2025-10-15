using UnityEngine;
public class PlayerFallState : PlayerState, IPlayerAirborneState
{
    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {

        //playerAnimator.PlayFallAnimation(); 
    }

    public override void Update()
    {
        if (inputController.AttackInput)
        {
            stateMachine.ChangeState(new PlayerAttackState(stateMachine));
            return;
        }

        playerAnimator.UpdateAnimationParameters();

        if (playerMovement.Rigidbody.linearVelocity.y < 0 && playerMovement.IsNearLedge &&
            playerMovement.IsTouchingWall)
        {
            Debug.Log("[FallState] Ledge rilevato! Transizione a LedgeGrabState");
            stateMachine.ChangeState(new PlayerLedgeGrabState(stateMachine));
            return; 
        }

        if (playerMovement.IsGrounded)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        float currentSpeed = inputController.IsRunning ? playerMovement.RunSpeed : playerMovement.MoveSpeed;
        float moveVelocity = inputController.MoveInput.x * currentSpeed;
        playerMovement.SetHorizontalVelocity(moveVelocity);
    }
}