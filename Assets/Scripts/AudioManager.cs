using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip fishingMusic;
    public AudioClip underwaterMusic;

    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioSource reelSource;
    public AudioClip castSFX;
    public AudioClip splashSFX;
    public AudioClip reelSFX;
    public AudioClip caughtSFX;
    public AudioClip failSFX;
    public AudioClip clickSFX;
    public AudioClip cancelSFX;

    [Header("Volume")]
    public float musicVolume = 2f;
    public float sfxVolume = 2f;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        musicSource.loop = true;
        reelSource.loop = true;
        reelSource.clip = reelSFX;
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        PlayMenuMusic();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sfxVolume = 0.3f;
        sfxSource.volume = sfxVolume;
        reelSource.volume = sfxVolume;

        if(scene.name == "MainMenu")
        {
            PlayMenuMusic();
        }
        else if(scene.name == "GameScene")
        {
            PlayGameMusic();
        }
        else if(scene.name == "FishScene")
        {
            PlayUnderwaterMusic();
        }
    }

    public void PlayMenuMusic()
    {
        musicSource.clip = menuMusic;
        musicSource.Play();
    }

    public void PlayGameMusic()
    {
        musicSource.clip = gameMusic;
        musicSource.Play();
    }

    public void PlayFishingMusic()
    {
        musicSource.clip = fishingMusic;
        musicSource.Play();
    }

    public void PlayUnderwaterMusic()
    {
        musicSource.clip = underwaterMusic;
        musicSource.Play();
    }

    public void PlayCast()
    {
        sfxSource.PlayOneShot(castSFX, sfxVolume);
    }

    public void PlaySplash()
    {
        sfxSource.PlayOneShot(splashSFX, sfxVolume);
    }

    public void StartReel()
    {
        if(!reelSource.isPlaying)
        {
            reelSource.volume = sfxVolume;
            reelSource.Play();
        }
    }

    public void StopReel()
    {
        reelSource.Stop();
    }

    public void PlayCaught()
    {
        StopReel();
        sfxSource.PlayOneShot(caughtSFX, sfxVolume);
    }

    public void PlayFail()
    {
        StopReel();
        sfxSource.PlayOneShot(failSFX, sfxVolume);
    }

    public void PlayClick()
    {
        sfxSource.PlayOneShot(clickSFX, sfxVolume * 0.8f);
    }

    public void PlayCancel()
    {
        sfxSource.PlayOneShot(cancelSFX, sfxVolume * 1.3f);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        sfxVolume = clampedVolume;
        sfxSource.volume = clampedVolume;
        reelSource.volume = clampedVolume;
    }
}