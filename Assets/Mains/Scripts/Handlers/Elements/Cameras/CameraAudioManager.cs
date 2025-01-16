using UnityEngine;

public class CameraAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [SerializeField] private AudioClip _eatingSound;

    private void Awake()
    {
        View.OnToggleSettings += HandleMusic;
    }

    private void OnDestroy()
    {
        View.OnToggleSettings -= HandleMusic;
    }

    private void Start()
    {
        HandleMusic();
    }

    public void PlayEatingSFX()
    {
        if (!Key.Config.EnableSound) return;

        _sfxSource.PlayOneShot(_eatingSound);
    }

    public void HandleMusic(SettingsType type = SettingsType.Music)
    {
        if (Key.Config.EnableMusic)
        {
            if (!_musicSource.isPlaying) _musicSource.Play();
        }
        else _musicSource.Stop();
    }
}