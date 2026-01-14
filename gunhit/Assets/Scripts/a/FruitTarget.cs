using UnityEngine;

public sealed class FruitTarget : MonoBehaviour
{
    [SerializeField] private Collider2D col;
    [SerializeField] private GameObject hitVfx;

    private bool hit;

    private void Awake()
    {
        if (col == null) col = GetComponent<Collider2D>();
    }

    public void OnHit()
    {
        if (hit) return;
        hit = true;

        if (col != null) col.enabled = false;

        if (hitVfx != null)
            Instantiate(hitVfx, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
