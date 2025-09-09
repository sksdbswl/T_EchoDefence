
public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }
    public PrevState PrevState { get; }
    public SpawnState SpawnState { get; }
    public FightState FightState { get; }
    public ClearState ClearState { get; }
    public NextState NextState { get; }
    
    public PlayerStateMachine(Player player)
    {
        Player = player;

        PrevState = new PrevState(this);
        SpawnState = new SpawnState(this);
        FightState = new FightState(this);
        ClearState = new ClearState(this);
        NextState = new NextState(this);
    }
}
