public class CollectingDiamondsState : IGameState
{
    private readonly GameStateMachine _machine;
    private readonly GameHUD _hud;

    public CollectingDiamondsState(GameStateMachine machine, GameHUD hud)
    {
        _machine = machine;
        _hud = hud;
    }

    public void Enter()
    {
        _hud.ShowDiamondCounter(true);
        _hud.ShowFindExit(false);
        _hud.UpdateDiamondCount(_machine.CollectedDiamonds, _machine.TotalDiamonds);
    }

    public void Exit() { }
    public void Tick() { }
}
