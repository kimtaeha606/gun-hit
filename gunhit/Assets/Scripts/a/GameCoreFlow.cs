using UnityEngine;
using static StageDatabase;
using System.Collections;

public class GameCoreFlow : MonoBehaviour
{
    public enum CoreState { Playing, Cleared, GameOver }

    [Header("Refs")]
    [SerializeField] private StageSpawner spawner;
    [SerializeField] private DiskController disk;
    [SerializeField] private StageDatabase stageDb;
    [SerializeField] private DiskRotationDriver rotationDriver;

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
        CancelGameOverRoutine();

        currentStage = stageIndex;

        StageConfig cfg = GetStageConfig(stageIndex);

        ammo = Mathf.Max(0, cfg.ammo);
        fruitsRemaining = Mathf.Max(0, cfg.fruitCount);

        state = CoreState.Playing;

        // ���� ȣ��
        if (disk != null)
        {
            
            disk.SetActive(true);
        }

        if (rotationDriver != null)
        {
            if (cfg.rotationPattern != null)
                rotationDriver.SetPattern(cfg.rotationPattern, currentStage);
            else
                rotationDriver.SetPattern(null, currentStage);
        }

        if (cfg.rotationPattern == null && disk != null)
        {
            disk.SetSpeed(cfg.rotateSpeed);
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
            
            
            CheckGameOverDeferred();  
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

    private Coroutine gameOverRoutine;

    private void CheckGameOverDeferred()
    {
        if (ammo != 0) return;
        if (fruitsRemaining <= 0) return;

        if (gameOverRoutine != null) StopCoroutine(gameOverRoutine);
        gameOverRoutine = StartCoroutine(CoGameOverAfter(1.0f)); // 0.4~1.0 추천, 3초는 너무 김
    }

    private IEnumerator CoGameOverAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (ammo == 0 && fruitsRemaining > 0)
            TriggerGameOver();

        gameOverRoutine = null;
        
    }

    private void CancelGameOverRoutine()
    {
        if (gameOverRoutine != null)
        {
            StopCoroutine(gameOverRoutine);
            gameOverRoutine = null;
        }
    }

}

