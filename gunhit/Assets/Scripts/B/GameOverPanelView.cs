using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public sealed class GameOverPanelView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject panelRoot;

    [Header("Buttons (optional)")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (panelRoot == null)
            panelRoot = gameObject;

        Hide();

        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);

        if (quitButton != null)
            quitButton.onClick.AddListener(Quit);
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

    private void Quit()
    {
        // PC/에디터에서는 효과 없음. 필요 없으면 버튼 자체를 빼도 됨.
        Application.Quit();
    }
}
