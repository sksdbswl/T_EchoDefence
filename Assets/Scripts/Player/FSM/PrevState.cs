using UnityEngine;

public class PrevState : MonoBehaviour, IState
{
    private StateMachine stateMachine;

    public PrevState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        // 초기 셋업
        Debug.Log("Prev State Enter");
    }

    public void HandleInput() { }

    public void Update()
    {
        // 초기 설정 및 타이머가 종료되면
        stateMachine.ChangeState(new SpawnState(stateMachine));
    }

    public void Exit()
    {
        Debug.Log("Prev State Exit");
    }
}