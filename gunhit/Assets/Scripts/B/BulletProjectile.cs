using UnityEngine;

public sealed class BulletProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 20f;

    [SerializeField] private Vector2 dir = Vector2.up;

    public System.Action<Collider2D> OnHit; // ?��?�??�림

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    public void Fire()
    {
        rb.linearVelocity = dir.normalized * speed;
        rb.rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        GameSignals.RaiseBulletFired();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnHit?.Invoke(other);   // ?�맞?�다?�만 ?�림
        Destroy(gameObject);    // ?�괴 책임?� 총알

        if (other.TryGetComponent(out IHitReceiver hit))
            hit.ReceiveHit(this);
        Debug.Log("코드문제다 씨발롬아");
        
    }

    [SerializeField] private float lifeTime = 3f;
    private float dieAt;

    private void OnEnable()
    {
        dieAt = Time.time + lifeTime;
    }

    private void Update()
    {
        if (Time.time >= dieAt)
            Destroy(gameObject);
    }
}

