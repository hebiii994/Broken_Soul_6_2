using UnityEngine;

public class PlayerIdleState: PlayerState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log($"[IdleState Enter] Entrato in Idle. Velocità attuale: {playerMovement.Rigidbody.linearVelocity}. Chiamo StopMovement.");
        playerMovement.StopMovement();
        Debug.Log($"[IdleState Enter] Velocità DOPO StopMovement: {playerMovement.Rigidbody.linearVelocity}");
    }
    
    public override void Update()
    {
        if (!playerMovement.IsGrounded)
        {
            stateMachine.ChangeState(new PlayerFallState(stateMachine));
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

        if (inputController.MoveInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(new PlayerMoveState(stateMachine));
            return;
        }


        playerAnimator.UpdateAnimationParameters();
    }


}