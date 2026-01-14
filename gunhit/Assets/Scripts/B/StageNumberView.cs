using TMPro;
using UnityEngine;

public sealed class StageNumberView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;

    private void Awake()
    {
        if (stageText == null)
            stageText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void StageStarted(int stageIndex)
    {
        stageText.text = $"STAGE {stageIndex + 1}";
    }
}
