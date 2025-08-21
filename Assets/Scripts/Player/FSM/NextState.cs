using UnityEngine;

public class NextState : MonoBehaviour, IState
{
    private StateMachine stateMachine;

    public NextState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Next State Enter");
    }

    public void HandleInput() { }

    public void Update()
    {
        // 다음 스테이지 전환
        stateMachine.ChangeState(new PrevState(stateMachine)); 
    }

    public void Exit()
    {
        Debug.Log("Next State Exit");
    }
}
