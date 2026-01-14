using UnityEngine;

public sealed class AudioSFXPlayer : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip fruitDestroyedClip;

    private void Awake()
    {
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GameSignals.ShootRequested += OnShootRequested;
        GameSignals.GameOver += OnGameOver;
        GameSignals.FruitDestroyed += OnFruitDestroyed;
    }

    private void OnDisable()
    {
        GameSignals.ShootRequested -= OnShootRequested;
        GameSignals.GameOver -= OnGameOver;
        GameSignals.FruitDestroyed -= OnFruitDestroyed;
    }

    private void OnShootRequested()
    {
        Play(shootClip);
    }

    private void OnGameOver()
    {
        Play(gameOverClip);
    }

    private void OnFruitDestroyed(FruitTarget fruit)
    {
        Play(fruitDestroyedClip);
    }

    private void Play(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
