using UnityEngine;

public sealed class BulletProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 20f;
    [SerializeField] private Vector2 dir = Vector2.up;

    public System.Action<Collider2D> OnHit;

    [SerializeField] private float lifeTime = 3f;
    private float dieAt;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    public void Fire(Vector2 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        dir = direction.sqrMagnitude > 0f ? direction.normalized : dir;

        rb.simulated = true;
        rb.WakeUp();

        Vector2 fireDir = dir.sqrMagnitude > 0f ? dir.normalized : (Vector2)transform.up;
        rb.linearVelocity = fireDir * speed;

        float angle = Mathf.Atan2(fireDir.y, fireDir.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle - 90f);

        GameSignals.RaiseBulletFired();
    }

    private void OnEnable()
    {
        dieAt = Time.time + lifeTime;
    }

    private void Update()
    {
        if (Time.time >= dieAt)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnHit?.Invoke(other);
        Destroy(gameObject);

        if (other.TryGetComponent(out IHitReceiver hit))
            hit.ReceiveHit(this);
    }
}
