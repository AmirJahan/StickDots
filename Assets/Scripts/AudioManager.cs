using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    //[SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private EventReference _menuMusic;
    [SerializeField] private EventReference _ingameMusic;
    [SerializeField] private EventReference _beepSound;
    [SerializeField] private EventReference _allBoxesSFX;

    private EventInstance _audioInstance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
            PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        PlayAudio(_menuMusic);
    }

    public void PlayInGameMusic()
    {
        StopAudio();
        PlayAudio(_ingameMusic);
    }

    public void PlayBeepSound()
    {
        PlayAudio(_beepSound);
    }

    public void PlayAllBoxesCaptured()
    {
        PlayAudio(_allBoxesSFX);
    }

    private void PlayAudio(EventReference reference)
    {
        _audioInstance = RuntimeManager.CreateInstance(reference);
        _audioInstance.start();
        _audioInstance.release();
    }

    public void StopAudio()
    {
        _audioInstance.stop(STOP_MODE.IMMEDIATE);
    }
}