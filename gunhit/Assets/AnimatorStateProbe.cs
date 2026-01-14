using UnityEngine;

public sealed class AnimatorStateProbe : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sr;

    private int lastStateHash;
    private Sprite lastSprite;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!sr) sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!animator || !sr) return;

        var st = animator.GetCurrentAnimatorStateInfo(0);
        if (st.fullPathHash != lastStateHash)
        {
            lastStateHash = st.fullPathHash;
            Debug.Log($"[Animator] State changed -> hash={st.fullPathHash}, normalizedTime={st.normalizedTime}", this);
        }

        if (sr.sprite != lastSprite)
        {
            lastSprite = sr.sprite;
            Debug.Log($"[Sprite] Sprite changed -> {(lastSprite ? lastSprite.name : "null")}", this);
        }
    }
}
