using System.Collections;
using UnityEngine;

public class PrevState : PlayerBaseState
{
    public PrevState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        //GameManager.Instance.CameraController.SetCameraState(CameraState.Prev);

        if (StageManager.Instance.Stage != 1)
        {
            stateMachine.Player.StartCoroutine(GameManager.Instance.CameraController.IntroSequence());
        }
        
        //stateMachine.Player.StartCoroutine(DelayToSpawn());
    }

    // private IEnumerator DelayToSpawn()
    // {
    //     // TODO :: 타이머 연출 추가
    //     GameManager.Instance.CameraController.SetCameraState(CameraState.Prev);
    //     
    //     yield return new WaitForSeconds(5f);
    //     stateMachine.ChangeState(stateMachine.FightState);
    //     
    //     var scroll = StageManager.Instance.GetComponent<MapScrollController>();
    //     scroll.ScrollUntilBossZone();
    // }

    public override void HandleInput() { }

    public override void Update()
    {
        if (StageManager.Instance.StageActive)
        {
            Debug.Log("다시 싸워 !");
            stateMachine.ChangeState(stateMachine.FightState);
        
            var scroll = StageManager.Instance.GetComponent<MapScrollController>();
            scroll.ScrollUntilBossZone();
        }
    }

    public override void Exit()
    {
        Debug.Log("Prev State Exit");
    }
}