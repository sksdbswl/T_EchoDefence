using UnityEngine;

public class FightState : PlayerBaseState
{
    public FightState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Fight Enter");
    }

    public override void HandleInput() { }

    public override void Update()
    {
        // 게임이 클리어 되었을경우 or 죽었을 경우
        
        if (true)
        {
            stateMachine.ChangeState(new ClearState(stateMachine));  
        }
        else
        {
            // TODO 게임 종료 로직 추가
        }
    }

    public override void Exit()
    {
        Debug.Log("Fight Exit");
    }
}
