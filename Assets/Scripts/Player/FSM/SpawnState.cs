using UnityEngine;

public class SpawnState : PlayerBaseState
{
    public SpawnState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        // 몬스터 소환 처리
        Debug.Log("Spawn State Enter");
    }

    public override void HandleInput() { }

    public override void Update()
    {
        // 소환이 끝났으면
        stateMachine.ChangeState(new FightState(stateMachine));
    }

    public override void Exit()
    {
        Debug.Log("Spawn State Exit");
    }
}