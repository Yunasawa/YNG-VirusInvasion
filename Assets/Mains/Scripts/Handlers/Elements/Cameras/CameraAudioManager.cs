using UnityEngine;

public class CameraAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _sfxSource;

    [SerializeField] private AudioClip _eatingSound;

    public void PlayEatingSFX()
    {
        _sfxSource.PlayOneShot(_eatingSound);
    }
}