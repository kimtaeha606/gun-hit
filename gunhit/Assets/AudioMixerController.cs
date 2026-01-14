using UnityEngine;
using UnityEngine.Audio;

public sealed class AudioMixerController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer;

    // AudioMixer에서 Expose한 파라미터 이름과 정확히 동일해야 함
    [SerializeField] private string sfxParam = "SFXVolume";
    [SerializeField] private string bgmParam = "BGMVolume";

    // value: 0~1
    public void SetSFXVolume(float value)
    {
        SetVolume(sfxParam, value);
    }

    // value: 0~1
    public void SetBGMVolume(float value)
    {
        SetVolume(bgmParam, value);
    }

    private void SetVolume(string paramName, float value01)
    {
        if (mixer == null) return;

        // 0이면 Log10(0)로 -Infinity 나와서 문제 생김
        value01 = Mathf.Clamp(value01, 0.0001f, 1f);

        float db = Mathf.Log10(value01) * 20f;
        mixer.SetFloat(paramName, db);
    }
}
