using JetBrains.Annotations;
using UnityEngine;

public sealed class UIGameplayBinder : MonoBehaviour
{

    [SerializeField] private StageNumberView stageNumberView;
    [SerializeField] private AmmoIconView ammoIconView;
    [SerializeField] private AmmoUsedOverlayView ammoUsedOverlayView;
    private int totalAmmo;
    private void Awake()
    {
        if (stageNumberView == null)
            stageNumberView = GetComponentInChildren<StageNumberView>(true);
        if (ammoIconView == null)
            ammoIconView = GetComponentInChildren<AmmoIconView>(true);
        if (ammoUsedOverlayView == null)
            ammoUsedOverlayView = GetComponentInChildren<AmmoUsedOverlayView>(true);
    }
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
        totalAmmo=ammo;
        if (ammoIconView != null)
            ammoIconView.Render(ammo);
    }

    private void OnAmmoChanged(int ammo)
    {
        int usedAmmo = Mathf.Max(0, totalAmmo - ammo);
        if (ammoUsedOverlayView == null)
        {
            Debug.LogWarning("UIGameplayBinder: ammoUsedOverlayView is null.", this);
            return;
        }
        Debug.Log($"UIGameplayBinder.OnAmmoChanged ammo={ammo} usedAmmo={usedAmmo} totalAmmo={totalAmmo} target={ammoUsedOverlayView.name}", this);
        ammoUsedOverlayView.MarkUsed(usedAmmo, totalAmmo);
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
