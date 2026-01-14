using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
public sealed class StageSpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform diskRoot;
    [SerializeField] private FruitTarget fruitPrefab;
    [SerializeField] private FruitRingPlacer.Rules placementRules;


    [Header("Placement")]
    [Tooltip("반지름(원판 중심 기준)")]
    [SerializeField] private float radius = 1.8f;
    

    [SerializeField] private SpriteRenderer diskSprite;
    [SerializeField] private SpriteRenderer fruitSprite;
    [SerializeField] private float radiusPadding = 0.1f;
    [SerializeField] private float fruitRadiusPadding = 0.02f;

    private readonly List<FruitTarget> spawned = new();

    public void BuildStage(int fruitCount)
    {
        ClearStage();

        if (diskRoot == null || fruitPrefab == null) return;
        if (fruitCount <= 0) return;

        var placer = new FruitRingPlacer();
        var rules = placementRules ?? new FruitRingPlacer.Rules();
        var angles = placer.BuildAngles(fruitCount, rules);




        

        for (int i = 0; i < angles.Count; i++)
        {
            float angle = angles[i];

            float useRadius = GetAutoRadius();
            Vector3 localPos = AngleToLocalPos(angle, useRadius);

            FruitTarget ft = Instantiate(fruitPrefab, diskRoot);
            ft.transform.localPosition = localPos;
            ft.transform.localRotation = Quaternion.identity;

            spawned.Add(ft);
        }

    
    }

    public void ClearStage()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] != null)
                Destroy(spawned[i].gameObject);
        }
        spawned.Clear();
    }

    private static Vector3 AngleToLocalPos(float degrees, float r)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad) * r, Mathf.Sin(rad) * r, 0f);
    }

    private float GetAutoRadius()
    {
        if (diskSprite == null) return radius;

        // diskSprite의 로컬 반지름(스프라이트 픽셀 크기 → 유니티 유닛 반영)을 로컬 스케일까지 포함해 계산
        // bounds 대신 localBounds(로컬) + lossyScale을 명확히 분리해서 쓴다
        float localHalfWidth = diskSprite.sprite.bounds.extents.x; // 로컬(스프라이트 에셋 기준)
        float scaleX = diskSprite.transform.localScale.x;          // 같은 로컬 체인에서 쓰는 게 핵심

        float localRadius = localHalfWidth * scaleX;
        return Mathf.Max(0.01f, localRadius + radiusPadding);
    }

}
