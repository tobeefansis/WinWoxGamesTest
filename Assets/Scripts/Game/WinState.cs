public class WinState : IGameState
{
    private readonly GameHUD _hud;

    public WinState(GameHUD hud)
    {
        _hud = hud;
    }

    public void Enter() => _hud.ShowWin(true);
    public void Exit()  => _hud.ShowWin(false);
    public void Tick()  { }
}
