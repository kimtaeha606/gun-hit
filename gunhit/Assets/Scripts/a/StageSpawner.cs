using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
public sealed class StageSpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform diskRoot;
    [SerializeField] private FruitTarget fruitPrefab;

    [Header("Placement")]
    [Tooltip("반지름(원판 중심 기준)")]
    [SerializeField] private float radius = 1.8f;
    [Tooltip("각도 랜덤 오프셋(0이면 균등 배치)")]
    [SerializeField] private float randomAngleJitter = 0f;

    private readonly List<FruitTarget> spawned = new();

    public void BuildStage(int fruitCount)
    {
        ClearStage();

        if (diskRoot == null || fruitPrefab == null) return;
        if (fruitCount <= 0) return;
        //이건 나중에 수정할 거임 
        float step = 360f / fruitCount;
        float baseOffset = Random.Range(0f, 360f);
        //
        for (int i = 0; i < fruitCount; i++)
        {
            float angle = baseOffset + step * i;
            if (randomAngleJitter > 0f)
                angle += Random.Range(-randomAngleJitter, randomAngleJitter);

            Vector3 localPos = AngleToLocalPos(angle, radius);

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
        
}
