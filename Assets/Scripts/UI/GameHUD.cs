using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private DiamondCounterUI diamondCounter;
    [SerializeField] private FindExitUI findExitUI;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private WinUI winUI;

    public void UpdateDiamondCount(int collected, int total)
        => diamondCounter.SetCount(collected, total);

    public void ShowDiamondCounter(bool visible)
        => diamondCounter.SetVisible(visible);

    public void ShowFindExit(bool visible)
        => findExitUI.SetVisible(visible);

    public void ShowGameOver(bool visible)
        => gameOverUI.SetVisible(visible);

    public void ShowWin(bool visible)
        => winUI.SetVisible(visible);
}
