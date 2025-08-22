public interface IState
{
    void Exit();
    void Enter();
    void HandleInput();
    void Update();
}
