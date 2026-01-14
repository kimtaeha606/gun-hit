using UnityEngine;

public sealed class FireRateLimiter
{
    private readonly float cooldownSeconds;
    private float nextAllowedTime;

    // "한 프레임 여러 번" 방지
    private int lastGrantedFrame = -1;

    public FireRateLimiter(float cooldownSeconds)
    {
        this.cooldownSeconds = Mathf.Max(0f, cooldownSeconds);
        nextAllowedTime = 0f;
    }

    /// <summary>
    /// 이번 프레임에 "발사 1회"를 허용할지 판단한다.
    /// 입력이 여러 번 들어와도, 조건을 만족하면 딱 1번만 true를 반환하도록 설계.
    /// </summary>
    public bool TryGrant()
    {
        // 프레임당 1회 제한
        int frame = Time.frameCount;
        if (lastGrantedFrame == frame)
            return false;

        // 쿨타임 제한
        float now = Time.time;
        if (now < nextAllowedTime)
            return false;

        // 통과(허용)
        lastGrantedFrame = frame;
        nextAllowedTime = now + cooldownSeconds;
        return true;
    }

    /// <summary>외부에서 강제로 쿨타임을 리셋/동기화하고 싶을 때.</summary>
    public void Reset()
    {
        nextAllowedTime = 0f;
        lastGrantedFrame = -1;
    }

    /// <summary>남은 쿨타임(디버그/HUD용)</summary>
    public float GetRemainingCooldown()
    {
        return Mathf.Max(0f, nextAllowedTime - Time.time);
    }
}
