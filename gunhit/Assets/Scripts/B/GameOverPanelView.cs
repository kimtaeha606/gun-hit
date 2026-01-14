using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public sealed class GameOverPanelView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject panelRoot;

    [Header("Buttons (optional)")]
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        if (panelRoot == null)
            panelRoot = gameObject;

        Hide();

        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);
    }

    public void Show()
    {
        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }

    public void Restart()
    {
        // 가장 단순: 씬 재로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
