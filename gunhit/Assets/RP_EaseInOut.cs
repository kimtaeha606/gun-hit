using UnityEngine;

[CreateAssetMenu(menuName = "GunHit/RotationPatterns/EaseInOut")]
public sealed class RP_EaseInOut : RotationPattern
{
    public float minSpeed = 60f;
    public float maxSpeed = 180f;
    public float period = 3f;

    public override float Evaluate(float t)
    {
        float x = Mathf.Sin((t / Mathf.Max(0.01f, period)) * Mathf.PI * 2f) * 0.5f + 0.5f;
        return Mathf.Lerp(minSpeed, maxSpeed, x);
    }
}
