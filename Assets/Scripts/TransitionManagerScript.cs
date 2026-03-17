using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    [Header("Transition")]
    public Image transitionImage;
    public Sprite[] transitionFrames;
    public float frameRate = 15f;
    private bool isTransitioning = false;

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
        if(transitionImage != null)
            transitionImage.gameObject.SetActive(false);
    }

    public void TransitionToScene(string sceneName)
    {
        if(!isTransitioning)
        {
            StartCoroutine(PlayTransition(sceneName));
        }
    }

    IEnumerator PlayTransition(string sceneName)
    {
        isTransitioning = true;
        transitionImage.gameObject.SetActive(true);

        // Play forward
        for(int i = 0; i < transitionFrames.Length; i++)
        {
            transitionImage.sprite = transitionFrames[i];
            yield return new WaitForSeconds(1f / frameRate);
        }

        // Load scene
        SceneManager.LoadScene(sceneName);
        yield return null;

        // Play backward to reveal new scene
        for(int i = transitionFrames.Length - 1; i >= 0; i--)
        {
            transitionImage.sprite = transitionFrames[i];
            yield return new WaitForSeconds(1f / frameRate);
        }

        transitionImage.gameObject.SetActive(false);
        isTransitioning = false;
    }
}