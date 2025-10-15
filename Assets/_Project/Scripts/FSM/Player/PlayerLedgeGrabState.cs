using UnityEngine;

public class PlayerLedgeGrabState : PlayerState
{
    private Vector2 grabPosition;
    private Vector2 ledgeClimbPosition;

    public PlayerLedgeGrabState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        playerMovement.StopMovement();
        playerMovement.Rigidbody.gravityScale = 0f;
        playerMovement.SetGroundCheckEnabled(false);

        Collider2D playerCollider = playerMovement.GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogError("Collider del giocatore non trovato in LedgeGrabState.");
            stateMachine.ChangeState(new PlayerIdleState(stateMachine));
            return;
        }

        Vector2 ledgePoint = playerMovement.WallCheckHit.point;

        float grabOffsetX = 0.4f;      // Distanza dal muro per il grab
        float grabOffsetY = 1.0f;      // Quanto sotto il ledge
        float climbOffsetX = 0.4f;     // Quanto dentro la piattaforma dopo il climb
        float climbOffsetY = 0.1f;     // Quanto sopra il ledge dopo il climb

        grabPosition = new Vector2(
            ledgePoint.x - (grabOffsetX * playerMovement.FacingDirection),
            ledgePoint.y - grabOffsetY
        );

        ledgeClimbPosition = new Vector2(
            ledgePoint.x + (climbOffsetX * playerMovement.FacingDirection),
            ledgePoint.y + climbOffsetY
        );

        playerCollider.enabled = false;

        playerMovement.Rigidbody.MovePosition(grabPosition);

        // playerAnimator.PlayLedgeHangAnimation();

        Debug.Log($"[LedgeGrab] LedgePoint: {ledgePoint}, GrabPos: {grabPosition}, ClimbPos: {ledgeClimbPosition}");
    }

    public override void Update()
    {
        if (inputController.JumpInput || inputController.MoveInput.y > 0.1f)
        {

            stateMachine.ChangeState(new PlayerClimbUpState(stateMachine, ledgeClimbPosition));
            return;
        }
        else if (inputController.MoveInput.y < -0.1f || inputController.SlideInput)
        {
            stateMachine.ChangeState(new PlayerFallState(stateMachine));
        }
    }

    public override void Exit()
    {
        playerMovement.Rigidbody.gravityScale = 2f;
        playerMovement.SetGroundCheckEnabled(true);

        Collider2D playerCollider = playerMovement.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }
    }
}
