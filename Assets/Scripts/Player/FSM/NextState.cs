using UnityEngine;

public class NextState : PlayerBaseState
{
    public NextState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Next State Enter");
    }

    public override void HandleInput() { }

    public override void Update()
    {
        // 다음 스테이지 전환
        stateMachine.ChangeState(new PrevState(stateMachine)); 
    }

    public override void Exit()
    {
        Debug.Log("Next State Exit");
    }
}
