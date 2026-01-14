using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class FruitRingPlacer
{
    // 배치 알고리즘 종류
    public enum Mode
    {
        Uniform, // 360 / count 균등 배치
        Random   // 랜덤 배치 + 최소 간격 보장
    }

    [Serializable]
    public struct AngleArc
    {
        public float fromDeg; // 시작 각도
        public float toDeg;   // 끝 각도

        public bool Contains(float angleDeg)
        {
            angleDeg = Wrap360(angleDeg);
            float a = Wrap360(fromDeg);
            float b = Wrap360(toDeg);

            // 일반 구간
            if (a <= b)
                return angleDeg >= a && angleDeg <= b;

            // 래핑 구간 (300 → 40 같은 경우)
            return angleDeg >= a || angleDeg <= b;
        }
    }

    

    [Serializable]
    public sealed class Rules
    {
        public Mode mode = Mode.Uniform;

        // 균등 배치 시 기준 시작 각
        public bool randomStartAngle = true;
        public float startAngleDeg = 0f;

        // 랜덤 배치 시 최소 간격
        public float minSeparationDeg = 18f;

        // 랜덤 샘플링 시 최대 시도 횟수
        public int maxTriesPerFruit = 60;

        // 특정 구간 비우기 옵션
        public bool useForbiddenArcs = false;
        public List<AngleArc> forbiddenArcs = new();
    }

    /// <summary>
    /// 외부(StageSpawner)가 호출하는 유일한 진입점
    /// fruitCount → 배치 각도 리스트 반환
    /// </summary>
    public List<float> BuildAngles(int fruitCount, Rules rules, int? seed = null)
    {
        if (fruitCount <= 0)
            return new List<float>();

        var rng = seed.HasValue
            ? new System.Random(seed.Value)
            : new System.Random();

        // 시작 각도 결정
        float start = rules.randomStartAngle
            ? NextFloat(rng, 0f, 360f)
            : rules.startAngleDeg;

        // 배치 모드에 따라 알고리즘 분기
        return rules.mode switch
        {
            Mode.Uniform => BuildUniform(fruitCount, start, rules, rng),
            Mode.Random => BuildRandom(fruitCount, rules, rng),
            _ => BuildUniform(fruitCount, start, rules, rng),
        };
    }

    private List<float> BuildUniform(int count, float startDeg, Rules rules, System.Random rng)
    {
        var angles = new List<float>(count);
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = Wrap360(startDeg + step * i);

            // 금지 구간이면 배치하지 않음
            if (IsForbidden(angle, rules))
                continue;

            angles.Add(angle);
        }

        // 금지 구간 때문에 부족하면 랜덤으로 보충
        if (angles.Count < count)
            FillBySampling(angles, count, rules, rng);

        angles.Sort();
        return angles;
    }

    private List<float> BuildRandom(int count, Rules rules, System.Random rng)
    {
        var angles = new List<float>(count);

        FillBySampling(angles, count, rules, rng);

        angles.Sort();
        return angles;
    }

    private void FillBySampling(
        List<float> angles,
        int targetCount,
        Rules rules,
        System.Random rng)
    {
        while (angles.Count < targetCount)
        {
            float angle = SampleValidAngle(angles, rules, rng);
            if (angle < 0f)
                break;

            angles.Add(angle);
        }


    }

    private float SampleValidAngle(
        List<float> existing,
        Rules rules,
        System.Random rng)
    {
        for (int i = 0; i < rules.maxTriesPerFruit; i++)
        {
            float candidate = NextFloat(rng, 0f, 360f);

            if (IsValidCandidate(candidate, existing, rules))
                return Wrap360(candidate);
        }

        return -1f;
    }

    private bool IsValidCandidate(
        float candidate,
        List<float> existing,
        Rules rules)
    {
        candidate = Wrap360(candidate);

        if (IsForbidden(candidate, rules))
            return false;

        for (int i = 0; i < existing.Count; i++)
        {
            float delta = CircularDeltaDeg(candidate, existing[i]);
            if (delta < rules.minSeparationDeg)
                return false;
        }

        return true;
    }

    private bool IsForbidden(float angleDeg, Rules rules)
    {
        if (!rules.useForbiddenArcs)
            return false;

        foreach (var arc in rules.forbiddenArcs)
        {
            if (arc.Contains(angleDeg))
                return true;
        }

        return false;
    }

    private static float CircularDeltaDeg(float a, float b)
    {
        a = Wrap360(a);
        b = Wrap360(b);

        float diff = Mathf.Abs(a - b);
        return Mathf.Min(diff, 360f - diff);
    }

    private static float Wrap360(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    private static float NextFloat(System.Random rng, float min, float max)
    {
        return (float)(min + (max - min) * rng.NextDouble());
    }
}