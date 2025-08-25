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
        if (stateMachine.Player.IsBattleClear)
            stateMachine.ChangeState(stateMachine.ClearState);
        else if (stateMachine.Player.IsPlayerDead)
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


//
// public class FightState : PlayerBaseState
// {
//     public FightState(PlayerStateMachine stateMachine) : base(stateMachine) { }
//     
//     public override void Enter()
//     {
//         Debug.Log("Fight Enter :: 전투 시작");
//         Update();
//         UpdateBattle();
//         stateMachine.Player.animator.SetTrigger(PlayerAnimationController.Run);
//     }
//
//     public override void HandleInput() { }
//
//     public override void Update()
//     {
//         // 전투 중 조건 체크
//         if (stateMachine.Player.IsBattleClear)
//         {
//             stateMachine.ChangeState(stateMachine.ClearState);  
//         }
//         else if (stateMachine.Player.IsPlayerDead)
//         {
//             // TODO: 패배 로직 (GameOverState 등으로 전환)
//         }
//     }
//
//     public override void Exit()
//     {
//         Debug.Log("Fight Exit");
//         stateMachine.Player.StopCoroutine(AutoShoot());
//     }
//     
//     public void UpdateBattle()
//     {
//         //Debug.Log("Fight :: 전투 진행 중");
//         stateMachine.Player.StartCoroutine(AutoShoot());
//     }
//     
//     private IEnumerator AutoShoot()
//     {
//         while (true)
//         {
//             Vector3 pos = stateMachine.Player.Weapon.MuzzlePoint.position;
//             Quaternion rot = stateMachine.Player.Weapon.MuzzlePoint.rotation;
//             GameManager.Instance.BulletController.Shoot(pos, rot, stateMachine.Player);
//
//             // TODO :: 플레이어 속도 아이템에 따라 조절
//             yield return new WaitForSeconds(1.5f); 
//         }
//     }
// }
