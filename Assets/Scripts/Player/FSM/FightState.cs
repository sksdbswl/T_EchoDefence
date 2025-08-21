using UnityEngine;

public class FightState : MonoBehaviour, IState
{
    private StateMachine stateMachine;

    public FightState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Fight Enter");
    }

    public void HandleInput() { }

    public void Update()
    {
        // 게임이 클리어 되었을경우 or 죽었을 경우
        
        if (true)
        {
            stateMachine.ChangeState(new ClearState(stateMachine));  
        }
        else
        {
            // TODO 게임 종료 로직 추가
        }
    }

    public void Exit()
    {
        Debug.Log("Fight Exit");
    }
}
