using UnityEngine;

public class ClearState : PlayerBaseState
{
    public ClearState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Clear State Enter");
    }

    public override void HandleInput() { }

    public override void Update() {}

    public override void Exit()
    {
        Debug.Log("Clear State Exit");
    }
}
