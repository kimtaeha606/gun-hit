using System;
using UnityEngine;

/// <summary>
/// ShotVFXController
/// - 발사 연출 전담(로직 없음)
/// - 이벤트는 Action(위치 정보 없음) 그대로 유지
/// - 인스펙터로 muzzleTransform을 지정해서 그 위치에서 VFX 재생
/// </summary>
public sealed class ShotVFXController : MonoBehaviour
{
    [Header("Anchor")]
    [Tooltip("발사 연출이 나갈 기준 위치(총구/발사 지점)")]
    [SerializeField] private Transform muzzleTransform;

    [Tooltip("VFX를 muzzleTransform의 자식으로 붙일지 여부. (보통 true 추천)")]
    [SerializeField] private bool followMuzzle = true;

    [Header("Flash Objects (toggle on/off)")]
    [Tooltip("발사 순간 켤 오브젝트들(스프라이트 플래시, 라이트, VFX 루트 등)")]
    [SerializeField] private GameObject[] flashObjects;

    [Tooltip("flashObjects를 켠 뒤 자동으로 끌 딜레이(초). 0이면 끄지 않음")]
    [Min(0f)]
    [SerializeField] private float flashOffDelay = 0.06f;

    [Header("Particles (play)")]
    [Tooltip("발사 때 재생할 트레이서 파티클(선택)")]
    [SerializeField] private ParticleSystem[] tracerParticles;

    [Tooltip("발사 때 재생할 추가 파티클(스파크/연기 등)(선택)")]
    [SerializeField] private ParticleSystem[] extraParticles;

    private void Awake()
    {
        // 시작 시 플래시는 꺼두는 게 보통 안전
        if (flashObjects != null)
        {
            for (int i = 0; i < flashObjects.Length; i++)
            {
                if (flashObjects[i] != null)
                    flashObjects[i].SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        GameSignals.BulletFired += OnBulletFired;
    }

    private void OnDisable()
    {
        GameSignals.BulletFired -= OnBulletFired;
    }

    private void OnBulletFired()
    {
        // 1) 위치 정렬
        AlignToMuzzle();

        // 2) 표현 재생
        PlayFlash();
        PlayParticles(tracerParticles);
        PlayParticles(extraParticles);
    }

    private void AlignToMuzzle()
    {
        if (muzzleTransform == null)
            return;

        if (followMuzzle)
        {
            // muzzle 밑으로 붙이면 muzzle가 움직여도 계속 따라감
            if (transform.parent != muzzleTransform)
                transform.SetParent(muzzleTransform, worldPositionStays: false);

            // local 기준 위치/회전은 0으로 통일
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            // 부모는 유지하고, 발사 순간에만 위치/회전 스냅
            transform.position = muzzleTransform.position;
            transform.rotation = muzzleTransform.rotation;
        }
    }

    private void PlayFlash()
    {
        if (flashObjects == null || flashObjects.Length == 0)
            return;

        // Invoke 중복 방지
        if (flashOffDelay > 0f)
            CancelInvoke(nameof(StopFlash));

        for (int i = 0; i < flashObjects.Length; i++)
        {
            var go = flashObjects[i];
            if (go == null) continue;

            go.SetActive(true);

            // Animator가 있다면 Trigger("Play")가 존재할 때만 쏴줌(선택)
            var anim = go.GetComponent<Animator>();
            if (anim != null && HasTrigger(anim, "Play"))
                anim.SetTrigger("Play");
        }

        if (flashOffDelay > 0f)
            Invoke(nameof(StopFlash), flashOffDelay);
    }

    private void StopFlash()
    {
        if (flashObjects == null) return;

        for (int i = 0; i < flashObjects.Length; i++)
        {
            var go = flashObjects[i];
            if (go != null)
                go.SetActive(false);
        }
    }

    private static void PlayParticles(ParticleSystem[] arr)
    {
        if (arr == null || arr.Length == 0)
            return;

        for (int i = 0; i < arr.Length; i++)
        {
            var ps = arr[i];
            if (ps == null) continue;

            // 매 발사마다 "딱" 끊어치기
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }
    }

    private static bool HasTrigger(Animator animator, string name)
    {
        if (animator == null) return false;

        var ps = animator.parameters;
        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i].type == AnimatorControllerParameterType.Trigger && ps[i].name == name)
                return true;
        }
        return false;
    }
}
