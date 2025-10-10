using UnityEngine;

public class PlayerSlideState : PlayerState
{
    public PlayerSlideState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Update()
    {
        if (!inputController.SlideInput || !playerMovement.IsGrounded)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine));
            return;
        }

        playerAnimator.UpdateAnimationParameters();
    }

    public override void FixedUpdate()
    {
        playerMovement.ApplySlide();
    }


}
