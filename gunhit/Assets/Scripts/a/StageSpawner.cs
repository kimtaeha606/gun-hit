using System.Collections.Generic;
using UnityEngine;

public sealed class StageSpawner : MonoBehaviour
{
    [Header("Disk")]
    [SerializeField] private Transform diskRig;              // 회전 중심(진짜 원점)
    [SerializeField] private SpriteRenderer diskSprite;      // 비주얼(반지름 측정용)

    [Header("Fruits")]
    [SerializeField] private Transform worldFruitRoot;       // (권장) StageSpawner 아래 빈 오브젝트, scale=1
    [SerializeField] private FruitTarget fruitPrefab;
    [SerializeField] private SpriteRenderer fruitSprite;     // fruitPrefab 내부 SpriteRenderer 참조(에셋 반지름용)

    [Header("Placement")]
    [SerializeField] private FruitRingPlacer.Rules placementRules;

    [Header("Padding")]
    [SerializeField] private float contactPadding = 0.00f;   // 먼저 0으로 두고 시작

    private struct FruitSlot
    {
        public FruitTarget ft;
        public float angleDeg;   // 디스크 로컬 기준 각도
    }

    private readonly List<FruitSlot> slots = new();

    private void Awake()
    {
        if (worldFruitRoot == null)
            worldFruitRoot = transform;

        // 월드 루트는 스케일 1 강제(부모 영향 최소화)
        worldFruitRoot.localScale = Vector3.one;

        if (fruitSprite == null && fruitPrefab != null)
            fruitSprite = fruitPrefab.GetComponentInChildren<SpriteRenderer>(true);
    }

    public void BuildStage(int fruitCount)
    {
        ClearStage();

        if (diskRig == null || diskSprite == null || diskSprite.sprite == null) return;
        if (fruitPrefab == null || fruitCount <= 0) return;

        // 각도 생성
        var placer = new FruitRingPlacer();
        var rules = placementRules ?? new FruitRingPlacer.Rules();
        List<float> angles = placer.BuildAngles(fruitCount, rules);

        // 과일 생성 (부모는 worldFruitRoot = 디스크 자식 아님)
        for (int i = 0; i < angles.Count; i++)
        {
            FruitTarget ft = Instantiate(fruitPrefab, worldFruitRoot);
            ft.transform.rotation = Quaternion.identity; // 필요시
            slots.Add(new FruitSlot { ft = ft, angleDeg = angles[i] });
        }

        // 생성 직후 한 번 위치 갱신
        UpdateFruitPositions();
    }

    private void LateUpdate()
    {
        // 디스크가 회전 중이면 매 프레임 따라가야 “부모 없이 회전 동기화”가 됨
        if (slots.Count > 0)
            UpdateFruitPositions();
    }

    public void ClearStage()
    {
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].ft != null)
                Destroy(slots[i].ft.gameObject);
        }
        slots.Clear();
    }

    private void UpdateFruitPositions()
    {
        Vector3 center = GetDiskCenterWorld();                // 회전 중심(원점) 확정
        float attachR = GetAttachRadiusWorld();               // diskR + fruitR (+padding)

        Quaternion diskRot = diskRig.rotation;                // 디스크의 현재 회전(월드)

        for (int i = 0; i < slots.Count; i++)
        {
            var s = slots[i];
            if (s.ft == null) continue;

            // 디스크 로컬 기준 방향(각도)
            float rad = s.angleDeg * Mathf.Deg2Rad;
            Vector3 dirLocal = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);

            // 디스크 회전 적용한 월드 방향
            Vector3 dirWorld = diskRot * dirLocal;

            s.ft.transform.position = center + dirWorld * attachR;

            // “사과도 같이 돈다” 연출이 필요하면 회전도 맞추기(원하면 끄면 됨)
            s.ft.transform.rotation = diskRot;

            slots[i] = s;
        }
    }

    private Vector3 GetDiskCenterWorld()
    {
        // 원칙: 회전 중심은 diskRig.position 이게 1순위 정답
        // (diskSprite.bounds.center를 쓰면 피벗/오프셋에 따라 중심이 흔들릴 수 있음)
        return diskRig.position;
    }

    private float GetAttachRadiusWorld()
    {
        float diskR = GetDiskRadiusWorldStable(); // 디스크의 “실제 월드 반지름”

        float fruitR = 0f;
        if (fruitSprite != null && fruitSprite.sprite != null)
        {
            // 사과 반지름(에셋 기준) * 프리팹 로컬 스케일(월드 루트 아래라 scale 전파 없음)
            float assetR = fruitSprite.sprite.bounds.extents.x;
            float prefabScale = fruitPrefab.transform.localScale.x;
            fruitR = assetR * prefabScale;
        }

        return Mathf.Max(0.01f, diskR + fruitR + contactPadding);
    }
    private float GetDiskRadiusWorldStable()
    {
        // 에셋 기준 로컬 반지름
        float localR = diskSprite.sprite.bounds.extents.x;

        // 스케일만 반영(회전 영향 없음)
        Vector3 worldVec = diskSprite.transform.TransformVector(new Vector3(localR, 0f, 0f));
        return worldVec.magnitude;
    }
}
