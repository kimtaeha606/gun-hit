using UnityEngine;

public sealed class PlayerModuleRoot : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private PlayerShooterInput shooterInput;

    [Header("Shooting")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletProjectile bulletPrefab;

    private FireRateLimiter limiter;

    private void Awake()
    {
        limiter = new FireRateLimiter(0.12f);
        if (firePoint == null)
            firePoint = transform;
    }

    private void OnEnable()
    {
        if (shooterInput != null)
        {
            shooterInput.ShootIntent += OnShootIntentReceived;
        }
    }

    private void OnDisable()
    {
        if (shooterInput != null)
        {
            shooterInput.ShootIntent -= OnShootIntentReceived;
        }
    }

    private void OnShootIntentReceived()
    {
        if (!limiter.TryGrant())
            return;

        GameSignals.RaiseShootRequested();

        if (bulletPrefab == null || firePoint == null)
            return;

        BulletProjectile bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.Fire(firePoint.up);
    }
}
