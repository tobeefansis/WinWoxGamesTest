public class GameOverState : IGameState
{
    private readonly GameHUD _hud;

    public GameOverState(GameHUD hud)
    {
        _hud = hud;
    }

    public void Enter() => _hud.ShowGameOver(true);
    public void Exit() => _hud.ShowGameOver(false);
    public void Tick() { }
}
