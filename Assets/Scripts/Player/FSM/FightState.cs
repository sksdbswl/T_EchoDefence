using System.Collections;
using UnityEngine;

public class FightState : PlayerBaseState
{
    public FightState(PlayerStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        GameManager.Instance.CameraController.SetCameraState(CameraState.Fight);
        
        stateMachine.Player.PlayerOverlapSphere.enabled = true;
        stateMachine.Player.animator.SetTrigger(PlayerAnimationController.Run);
        GameManager.Instance.Units.SetUnitAnimation(PlayerAnimationController.Run);
        
        // 플레이어에 바인딩된 유닛 발사 루프 시작
        stateMachine.Player.StartCoroutine(StartFireAfterDelay(1f));
    }

    private IEnumerator StartFireAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.Instance.Units.StartFireLoop();
    }

    public override void Update()
    {
        if (GameManager.Instance.IsStageClear)
        {
            stateMachine.ChangeState(stateMachine.ClearState);
            GameManager.Instance.Units.StopFireLoop();
        }
        else if (stateMachine.Player.playerStat.UnitCnt < 0)
        {
            stateMachine.Player.animator.SetTrigger(PlayerAnimationController.Die);
            GameManager.Instance.GameOver();
        } 
    }

    public override void Exit()
    {
        GameManager.Instance.Units.StopFireLoop();
        stateMachine.Player.PlayerOverlapSphere.enabled = false;
    }
}