using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class CameraBounds2D : MonoBehaviour
{
    public Camera Cam { get; private set; }

    public float Left  { get; private set; }
    public float Right { get; private set; }
    public float Bottom{ get; private set; }
    public float Top   { get; private set; }

    [SerializeField] private float padding = 0f; // 필요하면 화면 밖 여유(월드단위)

    private int lastW, lastH;

    private void Awake()
    {
        Cam = GetComponent<Camera>();
        Recalculate();
    }

    private void LateUpdate()
    {
        // 에디터 Game뷰 변경/기기 해상도 변경 대응
        if (Screen.width != lastW || Screen.height != lastH)
            Recalculate();

        // 카메라가 움직이는 게임이면 매 프레임 갱신 권장
        // (카메라 고정 게임이면 위 if로도 충분)
        Recalculate();
    }

    public void Recalculate()
    {
        lastW = Screen.width;
        lastH = Screen.height;

        float halfH = Cam.orthographicSize;
        float halfW = halfH * Cam.aspect;

        Vector3 c = Cam.transform.position;

        Left   = c.x - halfW - padding;
        Right  = c.x + halfW + padding;
        Bottom = c.y - halfH - padding;
        Top    = c.y + halfH + padding;
    }
}
