using UnityEngine;

public class ClearState : MonoBehaviour, IState
{
    private StateMachine stateMachine;

    public ClearState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Clear State Enter");
    }

    public void HandleInput() { }

    public void Update()
    {
        // 클리어 또는 게임 종료
        if (true)
        {
            stateMachine.ChangeState(new NextState(stateMachine));  
        }
        else
        {
            //TODO 게임 종료 로직 추가
        }
    }

    public void Exit()
    {
        Debug.Log("Clear State Exit");
    }
}
