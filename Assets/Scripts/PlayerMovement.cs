using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float swimSpeed = 2.5f;
    private float currentSpeed;
    private Vector2 movement;
    public bool isMoving = false;
    private float moveTimer = 0f;
    public bool isRunning = false;
    public bool isSwimming = false;
    private Rigidbody2D rb;

    [Header("Animation")]
    public Animator playerAnim;
    private SpriteRenderer spriteRenderer;

    [Header("Effects")]
    public ParticleSystem dustTrail;

    [Header("Zone States")]
    public bool inFishingZone = false;

    [Header("Dialogue")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    private bool dialogueShowing = false;
    private float dialogueTimer = 0f;
    public float dialogueDuration = 12f;
    private string fishingDialogue = "i cant fish here this game was made in 2 days I shld try the left side of the dock";

    [Header("Script References")]
    public playerScript fishingScript;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
        if(dialogueBox != null) dialogueBox.SetActive(false);
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        HandleMovement();
        HandleDialogue();

        // Only show dialogue when space pressed outside fishing zone and not swimming and not currently fishing
        if(Input.GetKeyDown(KeyCode.Space) && !inFishingZone && !isSwimming)
        {
            if(fishingScript != null && !fishingScript.isFishing && !fishingScript.winnerAnim)
            {
                ShowDialogue();
            }
        }
    }

    void HandleMovement()
    {
        // Dont allow movement while fishing minigame is active
        if(fishingScript != null && fishingScript.fishBiting)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float moveX = 0f;
        float moveY = 0f;

        if(Input.GetKey(KeyCode.W)) moveY = 1f;
        if(Input.GetKey(KeyCode.S)) moveY = -1f;
        if(Input.GetKey(KeyCode.A)) moveX = -1f;
        if(Input.GetKey(KeyCode.D)) moveX = 1f;

        movement = new Vector2(moveX, moveY).normalized;
        isMoving = movement.magnitude > 0;

        // Dont move while casting or fishing
        // Dont move while casting or fishing
        if(fishingScript != null && (fishingScript.poleBack || fishingScript.isFishing))
        {
        movement = Vector2.zero;
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
        spriteRenderer.flipX = false;
        return;
        }

        if(isSwimming)
        {
            currentSpeed = swimSpeed;
            moveTimer = 0f;
            isRunning = false;
        }
        else if(isMoving)
        {
            moveTimer += Time.deltaTime;
            if(moveTimer >= 4f)
            {
                isRunning = true;
                currentSpeed = runSpeed;
            }
            else
            {
                isRunning = false;
                currentSpeed = walkSpeed;
            }
        }
        else
        {
            moveTimer = 0f;
            isRunning = false;
            currentSpeed = walkSpeed;
        }

        if(rb != null)
        {
            rb.MovePosition(rb.position + movement * currentSpeed * Time.deltaTime);
        }

        // Flip sprite
        if(moveX < 0) spriteRenderer.flipX = true;
        else if(moveX > 0) spriteRenderer.flipX = false;

        // Only handle animations if not fishing
        if(fishingScript == null || (!fishingScript.poleBack && !fishingScript.isFishing && !fishingScript.winnerAnim))
        {
            if(isSwimming)
            {
                playerAnim.Play("Base Layer.Player_swimming");
                SetDustTrail(false);
            }
            else if(!isMoving)
            {
                playerAnim.Play("Base Layer.Player_Still");
                SetDustTrail(false);
            }
            else if(isRunning)
            {
                playerAnim.Play("Base Layer.Player_run");
                SetDustTrail(true);
            }
            else
            {
                playerAnim.Play("Base Layer.Player_walk");
                SetDustTrail(false);
            }
        }
    }

    void SetDustTrail(bool active)
    {
        if(dustTrail != null && dustTrail.gameObject.activeSelf != active)
        {
            dustTrail.gameObject.SetActive(active);
        }
    }

    void HandleDialogue()
    {
        if(dialogueShowing)
        {
            dialogueTimer += Time.deltaTime;
            if(dialogueTimer >= dialogueDuration)
            {
                if(dialogueBox != null) dialogueBox.SetActive(false);
                dialogueShowing = false;
                dialogueTimer = 0f;
            }
        }
    }

    void ShowDialogue()
    {
        if(!dialogueShowing)
        {
            if(dialogueText != null) dialogueText.text = fishingDialogue;
            if(dialogueBox != null) dialogueBox.SetActive(true);
            dialogueShowing = true;
            dialogueTimer = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("FishingZone")) inFishingZone = true;
        if(other.CompareTag("WaterZone")) isSwimming = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("FishingZone")) inFishingZone = false;
        if(other.CompareTag("WaterZone")) isSwimming = false;
    }
}