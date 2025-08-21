using UnityEngine;

public class SpawnState : MonoBehaviour, IState
{
    private StateMachine stateMachine;

    public SpawnState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        // 몬스터 소환 처리
        Debug.Log("Spawn State Enter");
    }

    public void HandleInput() { }

    public void Update()
    {
        // 소환이 끝났으면
        stateMachine.ChangeState(new FightState(stateMachine));
    }

    public void Exit()
    {
        Debug.Log("Spawn State Exit");
    }
}