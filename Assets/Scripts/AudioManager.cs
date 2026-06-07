using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource; 
    public AudioSource sfxSource; 

    [Header("Background Music")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Sound Effects")]
    public AudioClip clickSFX;
    public AudioClip dashSFX;
    public AudioClip shootSFX;
    public AudioClip hitSFX;
    public AudioClip countdownTickSFX;
    public AudioClip countdownFightSFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMenuMusic()
    {
        if (bgmSource.clip == menuMusic) return; 
        bgmSource.clip = menuMusic;
        bgmSource.Play();
    }

    public void PlayGameMusic()
    {
        if (bgmSource.clip == gameMusic) return;
        bgmSource.clip = gameMusic;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip); 
        }
    }

    public void PlayButtonSound()
    {
        PlaySFX(clickSFX);
    }

    public void SetMusicVolume (float volume)
    {
        if (bgmSource != null) bgmSource.volume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume (float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        if(bgmSource != null) bgmSource.volume = savedMusic;
        if(sfxSource != null) sfxSource.volume = savedSFX;
    }
}