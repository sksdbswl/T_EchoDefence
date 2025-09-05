using System.Collections;
using UnityEngine;

public class FightState : PlayerBaseState
{
    public FightState(PlayerStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        Debug.Log("Fight Enter :: 전투 시작");
        stateMachine.Player.animator.SetTrigger(PlayerAnimationController.Run);

        // 플레이어에 바인딩된 유닛 발사 루프 시작
        GameManager.Instance.Units.StartFireLoop();
    }

    public override void Update()
    {
        if (GameManager.Instance.IsBattleClear)
            stateMachine.ChangeState(stateMachine.ClearState);
        else if (GameManager.Instance.IsPlayerDead)
        {
            // TODO: GameOver
        }
    }

    public override void Exit()
    {
        Debug.Log("Fight Exit");
        GameManager.Instance.Units.StopFireLoop();
    }
}