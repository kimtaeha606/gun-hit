using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerShooterInput : MonoBehaviour
{
    /// <summary>
    /// 발사 "의도"만 발행 (발사 가능 여부/실행은 상위에서 처리)
    /// </summary>
    public event Action ShootIntent;

    [Header("Input Actions (New Input System)")]
    [SerializeField] private InputActionReference shootAction; // Button (e.g., LMB / Touch Press)

    private void OnEnable()
    {
        if (shootAction == null) return;

        var action = shootAction.action;
        action.Enable();
        action.performed += OnShootPerformed;
    }

    private void OnDisable()
    {
        if (shootAction == null) return;

        var action = shootAction.action;
        action.performed -= OnShootPerformed;
        action.Disable();
    }

    private void OnShootPerformed(InputAction.CallbackContext ctx)
    {
        // "의도"만 올림
        ShootIntent?.Invoke();
        Debug.Log("발사 신호 전달 완료");
    }
}
