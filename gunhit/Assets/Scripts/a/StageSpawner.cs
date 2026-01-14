using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// StageSpawner (해결 A 최종본)
/// 목표:
/// 1) 디스크 스케일을 바꿔도 과일이 같이 커지지 않게 한다.
/// 2) 디스크가 회전하면 과일도 같이 회전하게 한다.
/// 3) 과일이 디스크 표면에 "붙어" 보이게 반지름을 계산한다.
///
/// 씬(또는 프리팹) 권장 Hierarchy:
/// DiskRig (회전 중심, scale=1)
/// ├─ DiskVisual (SpriteRenderer, scale만 여기서 변경)
/// └─ FruitRoot  (scale=1 고정, 과일은 여기 밑으로 스폰)
///
/// - DiskController는 DiskRig를 회전시키는 것을 권장.
/// - DiskVisual만 크기를 조절(Scale)하고, FruitRoot는 scale=1 유지.
/// </summary>
public sealed class StageSpawner : MonoBehaviour
{
    [Header("Rig / Parents")]
    [Tooltip("회전 중심(=DiskRig). DiskController가 회전시키는 대상과 동일하게 맞추세요.")]
    [SerializeField] private Transform diskRig;

    [Tooltip("과일이 생성될 부모(=FruitRoot). DiskRig의 자식이어야 디스크와 같이 회전합니다.")]
    [SerializeField] private Transform fruitRoot;

    [Header("Prefabs")]
    [SerializeField] private FruitTarget fruitPrefab;

    [Tooltip("배치 규칙(인스펙터에서 Random/Uniform 등 선택). null이면 기본 Rules 사용.")]
    [SerializeField] private FruitRingPlacer.Rules placementRules;

    [Header("Visual Refs")]
    [Tooltip("디스크 비주얼 SpriteRenderer(=DiskVisual). 스케일은 여기만 변경하세요.")]
    [SerializeField] private SpriteRenderer diskSprite;

    [Tooltip("과일 프리팹의 SpriteRenderer(반지름 계산용). 가능하면 프리팹에서 드래그.")]
    [SerializeField] private SpriteRenderer fruitSprite;

    [Header("Attachment")]
    [Tooltip("디스크 표면에서 바깥으로 여유(디스크-과일 사이 틈 보정)")]
    [SerializeField] private float radiusPadding = 0.05f;

    [Tooltip("과일 반지름 추가 여유(아웃라인/콜라이더 차이 보정)")]
    [SerializeField] private float fruitPadding = 0.02f;

    // 생성된 과일 관리
    private readonly List<FruitTarget> spawned = new();

    private void Awake()
    {
        // 안전 캐시: 인스펙터에 안 꽂아도 최대한 동작하게
        if (fruitSprite == null && fruitPrefab != null)
            fruitSprite = fruitPrefab.GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// 스테이지 과일 생성
    /// - 각도 계산: FruitRingPlacer
    /// - 위치 배치: FruitRoot 로컬 좌표로 배치(부모 회전에 따라 같이 회전)
    /// </summary>
    public void BuildStage(int fruitCount)
    {
        ClearStage();

        if (diskRig == null || fruitRoot == null || fruitPrefab == null) return;
        if (diskSprite == null || diskSprite.sprite == null) return;
        if (fruitCount <= 0) return;

        // (중요) 해결 A: FruitRoot는 DiskRig의 자식이어야 디스크와 같이 회전한다.
        // 실수 방지: 런타임에서 강제 부모를 맞춰버림(원하면 이 줄은 삭제 가능)
        if (fruitRoot.parent != diskRig)
            fruitRoot.SetParent(diskRig, true);

        // 1) 배치 각도 결정
        var placer = new FruitRingPlacer();
        var rules = placementRules ?? new FruitRingPlacer.Rules(); // 인스펙터 rules가 null이면 기본값 사용
        List<float> angles = placer.BuildAngles(fruitCount, rules);

        // 2) 디스크 표면에 "붙는" 로컬 반지름(한 번만 계산)
        float attachRadiusLocal = GetAttachRadiusLocal();

        // 3) 과일 생성 + 로컬 배치(이러면 DiskRig가 회전할 때 같이 돈다)
        for (int i = 0; i < angles.Count; i++)
        {
            float angle = angles[i];

            Vector3 localOffset = AngleToLocalOffset(angle, attachRadiusLocal);

            FruitTarget ft = Instantiate(fruitPrefab, fruitRoot);
            ft.transform.localPosition = localOffset;
            ft.transform.localRotation = Quaternion.identity;

            spawned.Add(ft);
        }
    }

    /// <summary>
    /// 현재 스테이지 과일 전부 제거
    /// </summary>
    public void ClearStage()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] != null)
                Destroy(spawned[i].gameObject);
        }
        spawned.Clear();
    }

    /// <summary>
    /// (핵심) 디스크 표면에 과일이 "붙는" 로컬 반지름 계산
    /// - DiskVisual만 scale이 변해도 이 값은 안정적으로 변한다.
    /// - FruitRoot는 scale=1을 전제로 과일 크기가 같이 커지지 않는다.
    ///
    /// attachRadiusLocal = diskLocalR * diskVisualLocalScale
    ///                  + fruitLocalR * fruitWorldScale(대개 1)
    ///                  + padding들
    /// </summary>
    private float GetAttachRadiusLocal()
    {
        // 디스크 로컬 반지름(에셋 기준)
        float diskLocalR = diskSprite.sprite.bounds.extents.x;

        // 디스크 비주얼의 스케일(로컬). DiskVisual만 스케일을 바꾸는 것을 전제로 함.
        float diskScale = diskSprite.transform.localScale.x;

        float diskR = diskLocalR * diskScale;

        float fruitR = 0f;
        if (fruitSprite != null && fruitSprite.sprite != null)
        {
            // 과일 로컬 반지름(에셋 기준)
            float fruitLocalR = fruitSprite.sprite.bounds.extents.x;

            // FruitRoot를 scale=1로 유지한다면, 과일 크기는 프리팹 스케일만 적용됨
            // 여기서는 fruitSprite의 현재 lossyScale을 반영(프리팹/씬 모두 대응)
            float fruitScale = fruitSprite.transform.lossyScale.x;

            fruitR = fruitLocalR * fruitScale;
        }

        return Mathf.Max(0.01f, diskR + fruitR + radiusPadding + fruitPadding);
    }

    /// <summary>
    /// angle(deg) + r(local) -> FruitRoot 기준 로컬 오프셋 벡터
    /// </summary>
    private static Vector3 AngleToLocalOffset(float degrees, float r)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad) * r, Mathf.Sin(rad) * r, 0f);
    }
}
