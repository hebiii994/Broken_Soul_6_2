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