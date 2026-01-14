using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class AudioSourceDriver : MonoBehaviour
{
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();

        // 기본 SFX 세팅
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f; // 2D 사운드
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        source.PlayOneShot(clip, volume);
    }

    public void Stop()
    {
        source.Stop();
    }
}
