using UnityEngine;

[CreateAssetMenu(menuName = "GunHit/RotationPatterns/Constant")]
public sealed class RP_Constant : RotationPattern
{
    public float speed = 120f;
    public override float Evaluate(float t) => speed;
}
