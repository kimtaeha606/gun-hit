using UnityEngine;

[CreateAssetMenu(menuName = "GunHit/RotationPatterns/ReverseEveryNSeconds")]
public sealed class RP_ReverseEveryNSeconds : RotationPattern
{
    public float baseSpeed = 120f;
    public float interval = 2f;

    public override float Evaluate(float t)
    {
        int k = Mathf.FloorToInt(t / Mathf.Max(0.01f, interval));
        return (k % 2 == 0) ? baseSpeed : -baseSpeed;
    }
}
