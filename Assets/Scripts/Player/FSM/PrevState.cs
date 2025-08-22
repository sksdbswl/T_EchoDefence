using System.Collections;
using UnityEngine;

public class PrevState : PlayerBaseState
{
    public PrevState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Prev State Enter");
        stateMachine.Player.StartCoroutine(DelayToSpawn());
    }

    private IEnumerator DelayToSpawn()
    {
        // TODO :: 타이머 연출 추가
        Debug.Log("Corutin Start :: Prev State Delay To Spawn");
        yield return new WaitForSeconds(5f);
        stateMachine.ChangeState(stateMachine.FightState);
    }

    public override void HandleInput() { }

    public override void Update()
    {
        Debug.Log("Prev State Update");
    }

    public override void Exit()
    {
        Debug.Log("Prev State Exit");
    }
}