using UnityEngine;

public sealed class UIGameplayBinder : MonoBehaviour
{

    [SerializeField] private StageNumberView stageNumberView;
    private void OnEnable()
    {
        GameSignals.StageStarted  += OnStageStarted;
        GameSignals.AmmoSet       += OnAmmoSet;
        GameSignals.AmmoChanged   += OnAmmoChanged;
        GameSignals.GameOver      += OnGameOver;
        GameSignals.StageCleared  += OnStageCleared;
    }

    private void OnDisable()
    {
        GameSignals.StageStarted  -= OnStageStarted;
        GameSignals.AmmoSet       -= OnAmmoSet;
        GameSignals.AmmoChanged   -= OnAmmoChanged;
        GameSignals.GameOver      -= OnGameOver;
        GameSignals.StageCleared  -= OnStageCleared;
    }

    private void OnStageStarted(int stageIndex)
    {
        stageNumberView.StageStarted(stageIndex);
    }

    private void OnAmmoSet(int ammo)
    {
        // 탄 수 절대값 세팅 (초기화/리로드)
    }

    private void OnAmmoChanged(int ammo)
    {
        // 발사 후 감소 반영
    }

    private void OnGameOver()
    {
        // 게임오버 UI 표시
    }

    private void OnStageCleared(int stageIndex)
    {
        // 스테이지 클리어 연출
    }
}
