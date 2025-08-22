using UnityEngine;

public class PrevState : PlayerBaseState
{
    public PrevState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        // 초기 셋업
        Debug.Log("Prev State Enter");
    }

    public override void HandleInput() { }

    public override void Update()
    {
        Debug.Log("Prev State Update");
        
        // 초기 설정 및 타이머가 종료되면
        stateMachine.ChangeState(new SpawnState(stateMachine));
    }

    public override void Exit()
    {
        Debug.Log("Prev State Exit");
    }
}