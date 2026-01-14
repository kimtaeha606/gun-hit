using System.Collections;
using UnityEngine;

public interface IHitReceiver
{
    void ReceiveHit(BulletProjectile bullet);
}

public sealed class FruitTarget : MonoBehaviour, IHitReceiver
{
    [Header("Break Animation")]
    [Tooltip("Animator가 있으면 Trigger로 파괴 애니메이션을 재생합니다.")]
    [SerializeField] private Animator animator;

    [Tooltip("Animator Trigger 이름 (애니메이션 전환 조건)")]
    [SerializeField] private string breakTriggerName = "Break";

    [Tooltip("파괴 애니메이션 길이(초). 끝나면 오브젝트를 제거합니다.")]
    [SerializeField] private float breakDuration = 0.35f;

    [Header("Optional")]
    [Tooltip("맞은 뒤 콜라이더를 꺼서 추가 충돌/중복 히트를 막습니다.")]
    [SerializeField] private Collider2D targetCollider;

    [Tooltip("맞은 뒤 리지드바디가 있으면 멈추거나 비활성화할 때 사용(선택)")]
    [SerializeField] private Rigidbody2D rb;

    

    private bool isBroken;

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (targetCollider == null) targetCollider = GetComponent<Collider2D>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    public void ReceiveHit(BulletProjectile bullet)
    {
        if (isBroken) return;
        isBroken = true;

        // 중복 히트 방지
        if (targetCollider != null) targetCollider.enabled = false;

        GameSignals.RaiseFruitHit(this);

        // 파괴 애니메이션 재생
        if (animator != null && !string.IsNullOrWhiteSpace(breakTriggerName))
        {
            animator.ResetTrigger(breakTriggerName);
            animator.SetTrigger(breakTriggerName);
        }

        // 애니 끝나면 제거
        StartCoroutine(CoDestroyAfter(breakDuration));

        
    }

    private IEnumerator CoDestroyAfter(float seconds)
    {
        if (seconds < 0f) seconds = 0f;
        yield return new WaitForSeconds(seconds);

        GameSignals.RaiseFruitDestroyed(this);

        Destroy(gameObject);
    }
}
