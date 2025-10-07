using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private float _attackEndTime;

    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        playerCombat.HandleAttack();
        _attackEndTime = Time.time + playerCombat.AttackDuration;
    }

    public override void Update()
    {
        if (Time.time >= _attackEndTime)
        {
            stateMachine.ChangeState(new PlayerIdleState(stateMachine));
        }
    }
}
