using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    public Animator playerAnim;
    public bool isFishing;
    public bool poleBack;
    public bool throwBobber;
    public Transform fishingPoint;
    public GameObject bobber;
    public float targetTime = 0.0f;
    public float savedTargetTime;
    public float extraBobberDistance;
    public GameObject fishGame;
    public float timeTillCatch = 0.0f;
    public bool winnerAnim;
    public bool fishBiting = false;

    public PlayerMovement playerMovement;

    void Start()
    {
        isFishing = false;
        if(fishGame != null) fishGame.SetActive(false);
        throwBobber = false;
        targetTime = 0.0f;
        savedTargetTime = 0.0f;
        extraBobberDistance = 0.0f;
        fishBiting = false;

        if(AudioManager.instance != null)
            AudioManager.instance.PlayGameMusic();
    }

    void Update()
    {
        // Block all fishing if swimming
        if(playerMovement != null && playerMovement.isSwimming) return;

        // Start casting
        if(Input.GetKeyDown(KeyCode.Space) && !isFishing && !winnerAnim && playerMovement != null && playerMovement.inFishingZone)
        {
            poleBack = true;
        }

        // Fish bite timer
        if(isFishing)
        {
            timeTillCatch += Time.deltaTime;
            if(timeTillCatch >= 3 && !fishBiting)
            {
                fishBiting = true;
                if(fishGame != null) fishGame.SetActive(true);
                playerAnim.Play("Base Layer.Player_reeling");
                if(AudioManager.instance != null) AudioManager.instance.StartReel();
            }
        }

        // Release cast
        if(Input.GetKeyUp(KeyCode.Space) && !isFishing && !winnerAnim && playerMovement != null && playerMovement.inFishingZone)
        {
            poleBack = false;
            isFishing = true;
            throwBobber = true;
            if(AudioManager.instance != null) AudioManager.instance.PlayCast();

            if(targetTime >= 3)
                extraBobberDistance += 3;
            else
                extraBobberDistance += targetTime;
        }

        Vector3 temp = new Vector3(extraBobberDistance, 0, 0);
        fishingPoint.transform.position += temp;

        // Casting animation
        if(poleBack)
        {
            playerAnim.Play("Base Layer.Player_swingback");
            savedTargetTime = targetTime;
            targetTime += Time.deltaTime;
        }

        // Bobber throw
        if(isFishing)
        {
            if(throwBobber)
            {
                Instantiate(bobber, fishingPoint.position, fishingPoint.rotation, transform);
                if(AudioManager.instance != null) AudioManager.instance.PlaySplash();
                fishingPoint.transform.position -= temp;
                throwBobber = false;
                targetTime = 0.0f;
                savedTargetTime = 0.0f;
                extraBobberDistance = 0.0f;
            }
            if(!fishBiting)
            {
                playerAnim.Play("Base Layer.Player_fishing");
            }
        }

        // Cancel fishing with P
        if(Input.GetKeyDown(KeyCode.P) && timeTillCatch <= 3)
        {
            ResetFishing();
            playerAnim.Play("Base Layer.Player_Still");
            if(AudioManager.instance != null) AudioManager.instance.PlayCancel();
        }
    }

    void ResetFishing()
    {
        poleBack = false;
        throwBobber = false;
        isFishing = false;
        timeTillCatch = 0;
        fishBiting = false;
    }

    public void fishGameWon()
    {
        playerAnim.Play("Base Layer.Player_caught");
        if(AudioManager.instance != null) AudioManager.instance.PlayCaught();
        if(fishGame != null) fishGame.SetActive(false);
        ResetFishing();
    }

    public void fishGameLossed()
    {
        playerAnim.Play("Base Layer.Player_Still");
        if(AudioManager.instance != null) AudioManager.instance.PlayFail();
        if(fishGame != null) fishGame.SetActive(false);
        ResetFishing();
        TransitionManager.instance.TransitionToScene("FishScene");
    }
}