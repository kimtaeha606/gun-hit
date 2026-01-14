using UnityEngine;
using UnityEngine.UI;

public sealed class AmmoUsedOverlayView : MonoBehaviour
{
    [SerializeField] private Sprite usedBulletSprite; // 검은 총알

    [SerializeField] private AmmoIconView iconView;

    private void Awake()
    {
        if (iconView == null)
            iconView = GetComponent<AmmoIconView>();
        if (iconView == null)
            iconView = GetComponentInParent<AmmoIconView>();
        if (iconView == null)
            iconView = GetComponentInChildren<AmmoIconView>(true);
        if (iconView == null)
            Debug.LogWarning("AmmoUsedOverlayView: AmmoIconView reference missing.", this);
    }

    // 발사할 때마다 호출
    public void MarkUsed(int usedAmmo, int totalAmmo)
    {
        if (iconView == null)
        {
            Debug.LogWarning("AmmoUsedOverlayView: iconView is null.", this);
            return;
        }
        if (usedBulletSprite == null)
        {
            Debug.LogWarning("AmmoUsedOverlayView: usedBulletSprite is null.", this);
            return;
        }
        int clampedTotal = Mathf.Max(0, totalAmmo);
        if (clampedTotal == 0)
        {
            Debug.LogWarning("AmmoUsedOverlayView: totalAmmo is 0.", this);
            return;
        }

        int clampedUsed = Mathf.Clamp(usedAmmo, 0, clampedTotal);
        Debug.Log($"AmmoUsedOverlayView.MarkUsed used={usedAmmo} clampedUsed={clampedUsed} total={totalAmmo} clampedTotal={clampedTotal} iconView={iconView.name}", this);
        // 위에서부터 검은색으로 덮기
        for (int i = 0; i < clampedUsed; i++)
        {
            int indexFromTop = clampedTotal - 1 - i;
            iconView.SetSpriteAt(indexFromTop, usedBulletSprite);
        }

    }
}
