using System.Collections;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.IsJumping = true;
        playerMovement.Jump();
        playerAnimator.PlayJumpAnimation();
        stateMachine.StartCoroutine(ResetJumpingFlag());
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

    private IEnumerator ResetJumpingFlag()
    {
        yield return new WaitForSeconds(1f);
        stateMachine.IsJumping = false;
    }
}