using UnityEngine;

public class ClearState : PlayerBaseState
{
    public ClearState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Clear State Enter");
    }

    public override void HandleInput() { }

    public override void Update()
    {
        // 클리어 또는 게임 종료
        if (true)
        {
            stateMachine.ChangeState(new NextState(stateMachine));  
        }
        else
        {
            //TODO 게임 종료 로직 추가
        }
    }

    public override void Exit()
    {
        Debug.Log("Clear State Exit");
    }
}
