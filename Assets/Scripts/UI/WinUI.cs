using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        restartButton.onClick.AddListener(Restart);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
