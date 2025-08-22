using UnityEngine;

public class FightState : PlayerBaseState
{
    public FightState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    
    public override void Enter()
    {
        Debug.Log("Fight Enter :: 전투 시작");
        Update();
    }

    public override void HandleInput() { }

    public override void Update()
    {
        // 전투 중 조건 체크
        if (stateMachine.Player.IsBattleClear)
        {
            stateMachine.ChangeState(stateMachine.ClearState);  
        }
        else if (stateMachine.Player.IsPlayerDead)
        {
            // TODO: 패배 로직 (GameOverState 등으로 전환)
        }
        
        UpdateBattle();
    }

    public override void Exit()
    {
        Debug.Log("Fight Exit");
    }

    public void UpdateBattle()
    {
        Debug.Log("Fight :: 전투 진행 중");
    }
}
