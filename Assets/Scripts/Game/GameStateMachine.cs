using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    [SerializeField] private GameHUD hud;

    public int TotalDiamonds { get; private set; }
    public int CollectedDiamonds { get; private set; }

    private IGameState _current;
    private CollectingDiamondsState _collectingState;
    private FindingExitState _findingExitState;
    private GameOverState _gameOverState;
    private WinState _winState;

    private void Awake()
    {
        ServiceLocator.Instance.Register(this);
    }

    private void Update() => _current?.Tick();

    public void Initialize(int totalDiamonds)
    {
        TotalDiamonds = totalDiamonds;
        CollectedDiamonds = 0;

        _collectingState  = new CollectingDiamondsState(this, hud);
        _findingExitState = new FindingExitState(this, hud);
        _gameOverState    = new GameOverState(hud);
        _winState         = new WinState(hud);

        TransitionTo(_collectingState);
    }

    public void CollectDiamond()
    {
        if (!(_current is CollectingDiamondsState)) return;

        CollectedDiamonds++;
        hud.UpdateDiamondCount(CollectedDiamonds, TotalDiamonds);

        if (CollectedDiamonds >= TotalDiamonds)
            TransitionTo(_findingExitState);
    }

    public void PlayerDied()
    {
        if (_current is GameOverState) return;
        TransitionTo(_gameOverState);
    }

    public void ReachExit()
    {
        if (!(_current is FindingExitState)) return;
        TransitionTo(_winState);
    }

    public void TransitionTo(IGameState next)
    {
        _current?.Exit();
        _current = next;
        _current?.Enter();
    }
}
