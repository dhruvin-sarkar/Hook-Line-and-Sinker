using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayNightSystem : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayDuration = 300f;
    private float currentTime = 0f;
    public int dayCount = 1;

    [Header("Weather Icon")]
    public Image weatherIcon;
    public Sprite[] weatherSprites;

    [Header("Arrows")]
    public GameObject morningArrow;
    public GameObject noonArrow;
    public GameObject nightArrow;

    [Header("Screen Tint")]
    public Image screenTint;
    public Color morningColor = new Color(1f, 1f, 1f, 0f);
    public Color noonColor = new Color(1f, 0.6f, 0.2f, 0.15f);
    public Color nightColor = new Color(0.1f, 0.1f, 0.4f, 0.4f);

    [Header("Day Counter")]
    public TextMeshProUGUI dayText;

    void Start()
    {
        currentTime = 0f;
        dayCount = 1;
        UpdateDayText();
        UpdateWeather();
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime >= dayDuration)
        {
            currentTime = 0f;
            dayCount++;
            UpdateDayText();
        }

        UpdateWeather();
    }

    void UpdateWeather()
    {
        float t = currentTime / dayDuration;

        if(t < 0.167f)
        {
            if(weatherSprites.Length > 0) weatherIcon.sprite = weatherSprites[0];
            SetArrows(true, false, false);
            screenTint.color = morningColor;
        }
        else if(t < 0.333f)
        {
            if(weatherSprites.Length > 1) weatherIcon.sprite = weatherSprites[1];
            SetArrows(true, false, false);
            screenTint.color = morningColor;
        }
        else if(t < 0.5f)
        {
            if(weatherSprites.Length > 2) weatherIcon.sprite = weatherSprites[2];
            SetArrows(false, true, false);
            float lerpT = (t - 0.333f) / 0.167f;
            screenTint.color = Color.Lerp(morningColor, noonColor, lerpT);
        }
        else if(t < 0.667f)
        {
            if(weatherSprites.Length > 3) weatherIcon.sprite = weatherSprites[3];
            SetArrows(false, true, false);
            screenTint.color = noonColor;
        }
        else if(t < 0.833f)
        {
            if(weatherSprites.Length > 4) weatherIcon.sprite = weatherSprites[4];
            SetArrows(false, false, true);
            float lerpT = (t - 0.667f) / 0.167f;
            screenTint.color = Color.Lerp(noonColor, nightColor, lerpT);
        }
        else
        {
            if(weatherSprites.Length > 5) weatherIcon.sprite = weatherSprites[5];
            SetArrows(false, false, true);
            screenTint.color = nightColor;
        }
    }

    void SetArrows(bool morning, bool noon, bool night)
    {
        morningArrow.SetActive(morning);
        noonArrow.SetActive(noon);
        nightArrow.SetActive(night);
    }

    void UpdateDayText()
    {
        dayText.text = "DAY " + dayCount;
    }
}