using UnityEngine;
using UnityEngine.UI;

public sealed class AmmoUsedOverlayView : MonoBehaviour
{
    [SerializeField] private Sprite usedBulletSprite; // 검은 총알

    private AmmoIconView iconView;
    private int totalAmmo;
    private int usedAmmo;

    private void Awake()
    {
        iconView = GetComponent<AmmoIconView>();
    }

    // 판 시작 시 호출
    public void Init(int startAmmo)
    {
        totalAmmo = startAmmo;
        usedAmmo = 0;
    }

    // 발사할 때마다 호출
    public void MarkUsed(int usedCount)
    {
        usedAmmo = Mathf.Clamp(usedCount, 0, totalAmmo);

        // 위에서부터 검은색으로 덮기
        for (int i = 0; i < usedAmmo; i++)
        {
            int indexFromTop = totalAmmo - 1 - i;
            iconView.SetSpriteAt(indexFromTop, usedBulletSprite);
        }
    }
}
