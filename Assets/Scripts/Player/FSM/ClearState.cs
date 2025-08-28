using UnityEngine;

public class ClearState : PlayerBaseState
{
    public ClearState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Clear State Enter");
        stateMachine.Player.animator.SetTrigger(PlayerAnimationController.Idle);
        
        //TODO::
        // 카메라 초기위치로 셋팅
        // 플레이어 startpos로 이동
        // PrevState로 전환
    }

    public override void HandleInput() { }

    public override void Update()
    {
        //Debug.Log("Clear State Update");
    }

    public override void Exit()
    {
        Debug.Log("Clear State Exit");
    }
}
