using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishingBarScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public bool atTop;
    public float targetTime = 4.0f;
    public float savedTargetTime;

    public GameObject p1;
    public GameObject p2;
    public GameObject p3;
    public GameObject p4;
    public GameObject p5;
    public GameObject p6;
    public GameObject p7;
    public GameObject p8;

    public bool onFish;
    public playerScript playerS;
    public GameObject bobber;

    public GameObject fishObject;
    public float fishMaxSpeed = 0.5f;
    private bool fishGoingUp = true;
    public float fishBoundaryTop = 0.371f;
    public float fishBoundaryBottom = -0.353f;
    public float catchDistance = 0.2f;

    private float currentFishSpeed = 0f;
    private float fishPauseTimer = 0f;
    private float fishPauseDuration = 0f;
    private bool fishPaused = false;
    private float behaviorTimer = 0f;
    private float behaviorDuration = 0f;
    private enum FishState { SlowMove, FastMove, Pause, Twitch }
    private FishState currentState = FishState.SlowMove;

    void Start()
    {
        targetTime = 4.0f;
        PickNewBehavior();
    }

    void Update()
    {
        MoveFish();
        CheckFishOverlap();

        if(onFish)
        {
            targetTime += Time.deltaTime * 1.5f;
            if(AudioManager.instance != null) AudioManager.instance.StartReel();
        }
        else
        {
            targetTime -= Time.deltaTime;
            if(AudioManager.instance != null) AudioManager.instance.StopReel();
        }

        // Clamp targetTime so it never goes below 0 before triggering loss
        targetTime = Mathf.Max(targetTime, -0.1f);

        if(targetTime <= 0.0f)
        {
            ResetBar();
            if(AudioManager.instance != null) AudioManager.instance.PlayFail();
            playerS.fishGameLossed();
            Destroy(GameObject.Find("Bobber(Clone)"));
            targetTime = 4.0f;
        }
        if(targetTime >= 8.0f)
        {
            ResetBar();
            if(AudioManager.instance != null) AudioManager.instance.PlayCaught();
            playerS.fishGameWon();
            Destroy(GameObject.Find("Bobber(Clone)"));
            targetTime = 4.0f;
        }

        UpdatePoints();

        // Only add force when fishing minigame is active
        if(Input.GetKey(KeyCode.Space) && playerS.fishBiting)
        {
            rb.AddForce(Vector2.up * 1.5f, ForceMode2D.Impulse);
        }
    }

    void ResetBar()
    {
        transform.localPosition = new Vector3(0.66784f, -0.21878f, 0);
        onFish = false;
    }

    void UpdatePoints()
    {
        p1.SetActive(targetTime >= 1.0f);
        p2.SetActive(targetTime >= 2.0f);
        p3.SetActive(targetTime >= 3.0f);
        p4.SetActive(targetTime >= 4.0f);
        p5.SetActive(targetTime >= 5.0f);
        p6.SetActive(targetTime >= 6.0f);
        p7.SetActive(targetTime >= 7.0f);
        p8.SetActive(targetTime >= 8.0f);
    }

    void PickNewBehavior()
    {
        int rand = Random.Range(0, 4);
        currentState = (FishState)rand;

        switch(currentState)
        {
            case FishState.SlowMove:
                currentFishSpeed = Random.Range(0.05f, 0.15f);
                behaviorDuration = Random.Range(0.5f, 1.5f);
                break;
            case FishState.FastMove:
                currentFishSpeed = Random.Range(0.3f, fishMaxSpeed);
                behaviorDuration = Random.Range(0.2f, 0.6f);
                break;
            case FishState.Pause:
                currentFishSpeed = 0f;
                behaviorDuration = Random.Range(0.3f, 1.2f);
                break;
            case FishState.Twitch:
                currentFishSpeed = Random.Range(0.2f, 0.4f);
                behaviorDuration = Random.Range(0.1f, 0.3f);
                fishGoingUp = Random.value > 0.5f;
                break;
        }
        behaviorTimer = 0f;
    }

    void MoveFish()
    {
        if(fishObject == null) return;

        behaviorTimer += Time.deltaTime;
        if(behaviorTimer >= behaviorDuration) PickNewBehavior();

        if(currentFishSpeed > 0f)
        {
            if(fishGoingUp)
                fishObject.transform.localPosition += Vector3.up * currentFishSpeed * Time.deltaTime;
            else
                fishObject.transform.localPosition -= Vector3.up * currentFishSpeed * Time.deltaTime;
        }

        Vector3 clampedPos = fishObject.transform.localPosition;
        clampedPos.y = Mathf.Clamp(clampedPos.y, fishBoundaryBottom, fishBoundaryTop);
        fishObject.transform.localPosition = clampedPos;

        if(fishObject.transform.localPosition.y >= fishBoundaryTop)
        {
            fishGoingUp = false;
            PickNewBehavior();
        }
        if(fishObject.transform.localPosition.y <= fishBoundaryBottom)
        {
            fishGoingUp = true;
            PickNewBehavior();
        }
    }

    void CheckFishOverlap()
    {
        if(fishObject == null) return;

        float barY = transform.localPosition.y;
        float fishY = fishObject.transform.localPosition.y;
        float distance = Mathf.Abs(barY - fishY);
        onFish = distance < catchDistance;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("fish")) onFish = true;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("fish")) onFish = false;
    }
}