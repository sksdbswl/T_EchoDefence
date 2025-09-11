using UnityEngine;

public class ClearState : PlayerBaseState
{
    public ClearState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        if (StageManager.Instance.Stage == 8)
        {
            stateMachine.Player.animator.SetTrigger(PlayerAnimationController.Idle);
            GameManager.Instance.GameClear();        
            
            GameManager.Instance.CameraController.SetCameraState(CameraState.Clear);
            stateMachine.Player.animator.SetTrigger(PlayerAnimationController.Clear);
            
            return;   
        }
        
        stateMachine.Player.animator.SetTrigger(PlayerAnimationController.Idle);
        GameManager.Instance.Units.SetUnitAnimation(PlayerAnimationController.Idle);
        
        StageManager.Instance.Stage++;
        StageManager.Instance.CreateStageMonster(StageManager.Instance.Stage);
        GameManager.Instance.CameraController.SetCameraState(CameraState.Prev);
        
        // 몹/보스/맵 생성 이후 진행과정
        StageManager.Instance.StageSettings((startPos) =>
        {
            stateMachine.Player.animator.SetTrigger(PlayerAnimationController.Run);
            GameManager.Instance.Units.SetUnitAnimation(PlayerAnimationController.Run);
            
            
            var scroll = StageManager.Instance.GetComponent<MapScrollController>();
            scroll.ScrollUntilStartAtZero(startPos, () =>
            {
                GameManager.Instance.IsStageClear = false;
                StageManager.Instance.StageActive = false;
              
                
                stateMachine.Player.animator.SetTrigger(PlayerAnimationController.Idle);
                GameManager.Instance.Units.SetUnitAnimation(PlayerAnimationController.Idle);
                
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
        //Debug.Log("Clear State Exit");
    }
}
