using UnityEngine;
using static StageDatabase;

public class GameCoreFlow : MonoBehaviour
{
    public enum CoreState { Playing, Cleared, GameOver }

    [Header("Refs")]
    [SerializeField] private StageSpawner spawner;
    [SerializeField] private DiskController disk;
    [SerializeField] private StageDatabase stageDb;

    [Header("Fallback (stageDb 없을 때)")]
    [SerializeField] private StageConfig[] fallbackStages;

    [Header("State (read-only)")]
    [SerializeField] private int currentStage = 0;
    [SerializeField] private int ammo = 0;
    [SerializeField] private int fruitsRemaining = 0;
    [SerializeField] private CoreState state = CoreState.GameOver;

    private void OnEnable()
    {
        GameSignals.ShootRequested += OnShootRequested;
        GameSignals.FruitHit += OnFruitHit;
        GameSignals.DiskHitOrMiss += OnDiskHitOrMiss;
    }

    private void OnDisable()
    {
        GameSignals.ShootRequested -= OnShootRequested;
        GameSignals.FruitHit -= OnFruitHit;
        GameSignals.DiskHitOrMiss -= OnDiskHitOrMiss;
    }

    private void Start()
    {
        StartStage(0);
    }

    public void StartStage(int stageIndex)
    {
        currentStage = stageIndex;

        StageConfig cfg = GetStageConfig(stageIndex);

        ammo = Mathf.Max(0, cfg.ammo);
        fruitsRemaining = Mathf.Max(0, cfg.fruitCount);

        state = CoreState.Playing;

        // 하위 호출
        if (disk != null)
        {
            disk.Apply(cfg.rotateSpeed);
            disk.SetActive(true);
        }

        if (spawner != null)
            spawner.BuildStage(cfg.fruitCount);

        // 이벤트 발행
        GameSignals.RaiseStageStarted(currentStage);
        GameSignals.RaiseAmmoSet(ammo);

        // 즉시 클리어/즉시 게임오버 방지 처리
        if (fruitsRemaining == 0)
            StageClear();
        else if (ammo == 0)
            TriggerGameOver();
    }

    private StageConfig GetStageConfig(int stageIndex)
    {
        if (stageDb != null)
            return stageDb.Get(stageIndex);

        if (fallbackStages != null && fallbackStages.Length > 0)
        {
            int idx = Mathf.Clamp(stageIndex, 0, fallbackStages.Length - 1);
            return fallbackStages[idx];
        }

        return StageConfig.Default();
    }

    private void OnShootRequested()
    {
        if (state != CoreState.Playing) return;
        if (ammo <= 0) return;

        ammo--;
        GameSignals.RaiseAmmoChanged(ammo);

        if (ammo == 0 && fruitsRemaining > 0)
        {
            // 주의: 마지막 탄이 날아가서 과일 맞추는 경우는
            // B가 FruitHit 먼저 보내야 “세이브”됨.
            TriggerGameOver();
        }
    }

    private void OnFruitHit(FruitTarget fruit)
    {
        if (state != CoreState.Playing) return;
        if (fruit == null) return;

        // 과일 자체 반응 처리
        fruit.OnHit();

        fruitsRemaining = Mathf.Max(0, fruitsRemaining - 1);

        if (fruitsRemaining == 0)
            StageClear();
    }

    private void OnDiskHitOrMiss()
    {
        // 미스 처리: 아무 일 없음(탄은 이미 감소됨)
        // 확장할 거면 여기서 “미스 연출/사운드 이벤트”만 발행
        if (state != CoreState.Playing) return;
    }

    private void StageClear()
    {
        if (state != CoreState.Playing) return;

        state = CoreState.Cleared;
        GameSignals.RaiseStageCleared(currentStage);

        // 즉시 다음 스테이지
        StartStage(currentStage + 1);
    }

    private void TriggerGameOver()
    {
        if (state == CoreState.GameOver) return;

        state = CoreState.GameOver;

        if (disk != null) disk.SetActive(false);
        // 선택: 스테이지 정리
        // if (spawner != null) spawner.ClearStage();

        GameSignals.RaiseGameOver();
    }
}
