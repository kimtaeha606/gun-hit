using UnityEngine;

public sealed class PlayerModuleRoot : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private PlayerShooterInput shooterInput;
    [SerializeField] private BulletProjectile bulletProjectile;
    private FireRateLimiter limiter;

    private bool approve;

    private void Awake()
    {
        limiter = new FireRateLimiter(0.12f);
    }

    private void OnEnable()
    {
        if (shooterInput != null)
            shooterInput.ShootIntent += OnShootIntentReceived;
    }

    private void OnDisable()
    {
        if (shooterInput != null)
            shooterInput.ShootIntent -= OnShootIntentReceived;
    }

    private void OnShootIntentReceived()
    {
        approve=limiter.TryGrant();
        if(approve) bulletProjectile.Fire();
    }
}
