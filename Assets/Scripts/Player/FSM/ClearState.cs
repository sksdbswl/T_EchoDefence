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
        
        // 몹/보스/맵 생성 이후 진행과정
        StageManager.Instance.StageSettings((startPos) =>
        {
            stateMachine.Player.animator.SetTrigger(PlayerAnimationController.Run);

            var scroll = StageManager.Instance.GetComponent<MapScrollController>();
            scroll.ScrollUntilStartAtZero(startPos, () =>
            {
                GameManager.Instance.IsBattleClear = false;
                stateMachine.Player.animator.SetTrigger(PlayerAnimationController.Idle);
                stateMachine.ChangeState(new PrevState(stateMachine));
            });
        });
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
