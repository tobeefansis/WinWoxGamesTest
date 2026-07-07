public class FindingExitState : IGameState
{
    private readonly GameStateMachine _machine;
    private readonly GameHUD _hud;

    public FindingExitState(GameStateMachine machine, GameHUD hud)
    {
        _machine = machine;
        _hud = hud;
    }

    public void Enter()
    {
        _hud.ShowDiamondCounter(false);
        _hud.ShowFindExit(true);
    }

    public void Exit() { }
    public void Tick() { }
}
