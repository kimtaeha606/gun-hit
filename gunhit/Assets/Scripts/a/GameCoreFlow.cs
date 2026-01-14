using UnityEngine;
using static StageDatabase;

public class GameCoreFlow : MonoBehaviour
{
    public enum CoreState { Playing, Cleared, GameOver }

    [Header("Refs")]
    [SerializeField] private StageSpawner spawner;
    [SerializeField] private DiskController disk;
    [SerializeField] private StageDatabase stageDb;

    [Header("Fallback (stageDb ���� ��)")]
    [SerializeField] private StageConfig[] fallbackStages;

    [Header("State (read-only)")]
    [SerializeField] private int currentStage = 0;
    [SerializeField] private int ammo = 0;
    [SerializeField] private int fruitsRemaining = 0;
    [SerializeField] private CoreState state = CoreState.GameOver;

    private void OnEnable()
    {
        GameSignals.ShootRequested += OnShootRequested;
        GameSignals.FruitDestroyed += OnFruitDestroyed;
        GameSignals.DiskHitOrMiss += OnDiskHitOrMiss;
    }

    private void OnDisable()
    {
        GameSignals.BulletFired -= OnShootRequested;
        GameSignals.FruitDestroyed -= OnFruitDestroyed;
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

        // ���� ȣ��
        if (disk != null)
        {
            disk.Apply(cfg.rotateSpeed);
            disk.SetActive(true);
        }

        if (spawner != null)
            spawner.BuildStage(cfg.fruitCount);

        // �̺�Ʈ ����
        GameSignals.RaiseStageStarted(currentStage);
        GameSignals.RaiseAmmoSet(ammo);

        // ��� Ŭ����/��� ���ӿ��� ���� ó��
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
        Debug.Log("총알 감소");
        GameSignals.RaiseAmmoChanged(ammo);

        if (ammo == 0 && fruitsRemaining > 0)
        {
            // ����: ������ ź�� ���ư��� ���� ���ߴ� ����
            // B�� FruitHit ���� ������ �����̺ꡱ��.
            TriggerGameOver();
        }
    }

    private void OnFruitDestroyed(FruitTarget fruit)
    {
        
        if (state != CoreState.Playing) return;
        if (fruit == null) return;
        
        fruitsRemaining = Mathf.Max(0, fruitsRemaining - 1);

        if (fruitsRemaining == 0)
            StageClear();
    }

    private void OnDiskHitOrMiss()
    {
        // �̽� ó��: �ƹ� �� ����(ź�� �̹� ���ҵ�)
        // Ȯ���� �Ÿ� ���⼭ ���̽� ����/���� �̺�Ʈ���� ����
        if (state != CoreState.Playing) return;
    }

    private void StageClear()
    {
        if (state != CoreState.Playing) return;

        state = CoreState.Cleared;
        GameSignals.RaiseStageCleared(currentStage);

        // ��� ���� ��������
        StartStage(currentStage + 1);
    }

    private void TriggerGameOver()
    {
        if (state == CoreState.GameOver) return;

        state = CoreState.GameOver;

        if (disk != null) disk.SetActive(false);
        // ����: �������� ����
        // if (spawner != null) spawner.ClearStage();

        GameSignals.RaiseGameOver();
    }
}
