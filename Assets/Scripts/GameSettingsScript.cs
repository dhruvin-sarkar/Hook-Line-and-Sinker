using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettingsScript : MonoBehaviour
{
    public GameObject settingsPanel;
    public Scrollbar musicScrollbar;
    public Scrollbar sfxScrollbar;

    void Start()
    {
        settingsPanel.SetActive(false);
    }

    public void ToggleSettings()
    {
        if(settingsPanel.activeSelf)
        {
            if(AudioManager.instance != null) AudioManager.instance.PlayCancel();
            settingsPanel.SetActive(false);
        }
        else
        {
            if(AudioManager.instance != null) AudioManager.instance.PlayClick();
            settingsPanel.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if(AudioManager.instance != null) AudioManager.instance.PlayClick();
        settingsPanel.SetActive(false);
    }

    public void QuitToMenu()
    {
        if(AudioManager.instance != null) AudioManager.instance.PlayCancel();
        SceneManager.LoadScene("MainMenu");
    }

    public void OnMusicVolumeChanged(float value)
    {
        if(AudioManager.instance != null)
        {
            float clampedValue = Mathf.Clamp01(value);
            AudioManager.instance.SetMusicVolume(1f - clampedValue);
        }
    }

    public void OnSFXVolumeChanged(float value)
    {
        if(AudioManager.instance != null)
        {
            float clampedValue = Mathf.Clamp01(value);
            AudioManager.instance.SetSFXVolume(1f - clampedValue);
        }
    }
}