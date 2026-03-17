using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bobberScript : MonoBehaviour
{
    public bool gameIsOver;
    public float bobberTime;
    public GameObject fishIcon;

    void Start()
    {
        fishIcon.SetActive(false);
    }

    void Update()
    {
        bobberTime += Time.deltaTime;

        if(bobberTime >= 3)
        {
            fishIcon.SetActive(true);
        }

        if(Input.GetKeyDown(KeyCode.P) && bobberTime <= 3)
        {
            Destroy(gameObject);
        }

        if(gameIsOver == true)
        {
            Destroy(gameObject);
        }
    }

    public void gameOver()
    {
        gameIsOver = true;
    }
}