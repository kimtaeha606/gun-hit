using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
public sealed class StageSpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform diskRoot;
    [SerializeField] private FruitTarget fruitPrefab;
    [SerializeField] private FruitRingPlacer fruitRingPlacer;

    [Header("Placement")]
    [Tooltip("반지름(원판 중심 기준)")]
    [SerializeField] private float radius = 1.8f;
    

    [SerializeField] private SpriteRenderer diskSprite;
    [SerializeField] private float radiusPadding = 0.1f;

    private readonly List<FruitTarget> spawned = new();

    public void BuildStage(int fruitCount)
    {
        ClearStage();

        if (diskRoot == null || fruitPrefab == null) return;
        if (fruitCount <= 0) return;

        var placer = new FruitRingPlacer();
        var rules = new FruitRingPlacer.Rules
        {
            mode = FruitRingPlacer.Mode.Uniform,
            randomStartAngle = true,
            minSeparationDeg = 18f
        };

        List<float> angles = placer.BuildAngles(fruitCount, rules);

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
        // bounds는 월드 기준 크기
        float worldRadius = diskSprite.bounds.extents.x; // 원판이 정원이라고 가정
        return Mathf.Max(0.01f, worldRadius + radiusPadding);
    }

}
