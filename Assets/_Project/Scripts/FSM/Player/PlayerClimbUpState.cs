using UnityEngine;
using System.Collections;

public class PlayerClimbUpState : PlayerState
{
    private float climbDuration = 0.3f;
    private Vector2 targetClimbPosition; 


    public PlayerClimbUpState(PlayerStateMachine stateMachine, Vector2 climbPosition) : base(stateMachine)
    {
        targetClimbPosition = climbPosition;
    }

    public override void Enter()
    {
        playerMovement.StopMovement();
        playerMovement.Rigidbody.gravityScale = 0f;
        playerMovement.SetGroundCheckEnabled(false);

        Collider2D playerCollider = playerMovement.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        stateMachine.StartCoroutine(ClimbRoutine());
    }

    private IEnumerator ClimbRoutine()
    {
        Collider2D playerCollider = playerMovement.GetComponent<Collider2D>();

        if (playerCollider == null)
        {
            Debug.LogError("Collider del giocatore non trovato! Impossibile calcolare la posizione di arrampicata.");
            stateMachine.ChangeState(new PlayerIdleState(stateMachine));
            yield break;
        }

        Vector2 startPos = playerMovement.transform.position;
        Vector2 endPos = targetClimbPosition;

        Debug.Log($"[ClimbRoutine] Inizio arrampicata. Start: {startPos}, End: {endPos}");

        float timer = 0f;
        playerCollider.enabled = false;

        while (timer < climbDuration)
        {
            Vector2 newPosition = Vector2.Lerp(startPos, endPos, timer / climbDuration);
            playerMovement.Rigidbody.MovePosition(newPosition);
            timer += Time.deltaTime;
            yield return null;
        }

        playerMovement.Rigidbody.MovePosition(endPos);
        playerCollider.enabled = true;

        Debug.Log($"[ClimbRoutine] Arrampicata completata. Posizione finale: {playerMovement.transform.position}. Velocità: {playerMovement.Rigidbody.linearVelocity}");

        stateMachine.ChangeState(new PlayerIdleState(stateMachine));
    }

    public override void Exit()
    {
        playerMovement.Rigidbody.gravityScale = 2f;
        playerMovement.SetGroundCheckEnabled(true);
    }
}
