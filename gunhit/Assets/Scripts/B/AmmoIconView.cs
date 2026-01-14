using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class AmmoIconView : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform container;   // 아이콘 부모(없으면 본인)

    [Header("Visual")]
    [SerializeField] private Sprite bulletSprite;
    [SerializeField] private Vector2 iconSize = new Vector2(32, 32);
    [SerializeField] private float spacing = 6f;

    private readonly List<Image> pool = new();

    private void Awake()
    {
        if (container == null)
            container = GetComponent<RectTransform>();
    }

    // 판 시작 때 외부에서 1번 호출
    public void Render(int ammo)
    {
        if (ammo < 0) ammo = 0;

        EnsurePoolSize(ammo);

        for (int i = 0; i < pool.Count; i++)
        {
            bool active = i < ammo;
            pool[i].gameObject.SetActive(active);

            if (active)
            {
                pool[i].sprite = bulletSprite;
                pool[i].rectTransform.sizeDelta = iconSize;
            }
        }

        Layout(ammo);
    }

    private void EnsurePoolSize(int needed)
    {
        while (pool.Count < needed)
            pool.Add(CreateIcon());
    }

    private Image CreateIcon()
    {
        var go = new GameObject("AmmoIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(container, false);

        var rt = (RectTransform)go.transform;

        // 🔥 아래에서 위로 쌓기: Bottom-Left
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 0f);
        rt.pivot     = new Vector2(0f, 0f);

        rt.sizeDelta = iconSize;
        rt.anchoredPosition = Vector2.zero;

        var img = go.GetComponent<Image>();
        img.sprite = bulletSprite;
        img.raycastTarget = false;

        return img;
    }

    private void Layout(int count)
    {
        float step = iconSize.y + spacing;

        for (int i = 0; i < count; i++)
        {
            // 🔥 아래 → 위
            pool[i].rectTransform.anchoredPosition =
                new Vector2(0f, i * step);
        }
    }

}
