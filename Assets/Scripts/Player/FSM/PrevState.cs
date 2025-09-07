using System.Collections;
using UnityEngine;

public class PrevState : PlayerBaseState
{
    public PrevState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Player.StartCoroutine(DelayToSpawn());
    }

    private IEnumerator DelayToSpawn()
    {
        // TODO :: 타이머 연출 추가
        //CameraManager.Instance.StageSettingTimer();
        
        yield return new WaitForSeconds(5f);
        stateMachine.ChangeState(stateMachine.FightState);
        
        var scroll = StageManager.Instance.GetComponent<MapScrollController>();
        scroll.ScrollUntilBossZone();
    }

    public override void HandleInput() { }

    public override void Update()
    {
        //Debug.Log("Prev State Update");
    }

    public override void Exit()
    {
        Debug.Log("Prev State Exit");
    }
}