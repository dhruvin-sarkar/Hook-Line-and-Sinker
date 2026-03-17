using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float swimSpeed = 8f;
    public float maxSpeed = 15f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private SpriteRenderer spriteRenderer;
    private bool wasFacingRight = true;
    private bool isTurning = false;
    private float turnTimer = 0f;
    public float turnDuration = 0.3f;

    [Header("Animation")]
    public Animator fishAnim;

    [Header("Health")]
    public int maxHealth = 4;
    public int currentHealth;
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    private bool isHurt = false;
    private float hurtTimer = 0f;
    public float hurtDuration = 1f;
    private bool isInvincible = false;

    [Header("Timer")]
    public float survivalTime = 60f;
    private float currentTime = 0f;
    public TextMeshProUGUI timerText;
    private bool gameActive = true;

    [Header("Enemies")]
    public GameObject[] enemies;
    public float enemyActivationDelay = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        PhysicsMaterial2D bouncyMat = new PhysicsMaterial2D();
        bouncyMat.bounciness = 0.8f;
        bouncyMat.friction = 0f;
        rb.sharedMaterial = bouncyMat;
        GetComponent<Collider2D>().sharedMaterial = bouncyMat;

        currentHealth = maxHealth;
        UpdateHearts();

        // Disable all enemies at start
        foreach(GameObject enemy in enemies)
        {
            if(enemy != null) enemy.SetActive(false);
        }

        // Enable after transition delay
        StartCoroutine(ActivateEnemies());
    }

    IEnumerator ActivateEnemies()
    {
        yield return new WaitForSeconds(enemyActivationDelay);
        foreach(GameObject enemy in enemies)
        {
            if(enemy != null) enemy.SetActive(true);
        }
    }

    void Update()
    {
        if(!gameActive) return;

        HandleMovement();
        HandleTimer();
        HandleHurt();
    }

    void HandleMovement()
    {
        float moveX = 0f;
        float moveY = 0f;

        if(Input.GetKey(KeyCode.W)) moveY = 1f;
        if(Input.GetKey(KeyCode.S)) moveY = -1f;
        if(Input.GetKey(KeyCode.A)) moveX = -1f;
        if(Input.GetKey(KeyCode.D)) moveX = 1f;

        movement = new Vector2(moveX, moveY).normalized;

        rb.AddForce(movement * swimSpeed);
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);

        if(moveX > 0 && !wasFacingRight && !isTurning)
        {
            isTurning = true;
            turnTimer = 0f;
            fishAnim.Play("Fish_turning");
        }
        else if(moveX < 0 && wasFacingRight && !isTurning)
        {
            isTurning = true;
            turnTimer = 0f;
            fishAnim.Play("Fish_turning");
        }

        if(moveX < 0)
        {
            spriteRenderer.flipX = true;
            wasFacingRight = false;
        }
        else if(moveX > 0)
        {
            spriteRenderer.flipX = false;
            wasFacingRight = true;
        }

        if(isTurning)
        {
            turnTimer += Time.deltaTime;
            if(turnTimer >= turnDuration)
            {
                isTurning = false;
                turnTimer = 0f;
            }
            return;
        }

        if(!isHurt && !isTurning)
        {
            if(movement.magnitude == 0)
            {
                fishAnim.Play("Fish_swim");
            }
            else if(moveY > 0 && moveX != 0)
            {
                fishAnim.Play("Fish_swimup");
            }
            else if(moveY < 0 && moveX != 0)
            {
                fishAnim.Play("Fish_swimdown");
            }
            else
            {
                fishAnim.Play("Fish_swim");
            }
        }
    }

    void HandleTimer()
    {
        currentTime += Time.deltaTime;
        float remaining = survivalTime - currentTime;

        if(remaining <= 0)
        {
            remaining = 0;
            gameActive = false;
            WinGame();
        }

        int minutes = Mathf.FloorToInt(remaining / 60);
        int seconds = Mathf.FloorToInt(remaining % 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    void HandleHurt()
    {
        if(isHurt)
        {
            hurtTimer += Time.deltaTime;
            spriteRenderer.enabled = Mathf.FloorToInt(hurtTimer * 10) % 2 == 0;
            if(hurtTimer >= hurtDuration)
            {
                isHurt = false;
                isInvincible = false;
                hurtTimer = 0f;
                spriteRenderer.enabled = true;
            }
        }
    }

    public void TakeHit()
    {
        if(isInvincible) return;

        currentHealth--;
        isHurt = true;
        isInvincible = true;
        hurtTimer = 0f;
        fishAnim.Play("Fish_ouch");
        UpdateHearts();

        if(currentHealth <= 0)
        {
            gameActive = false;
            LoseGame();
        }
    }

    void UpdateHearts()
    {
        for(int i = 0; i < heartImages.Length; i++)
        {
            if(i < currentHealth)
                heartImages[i].sprite = fullHeart;
            else
                heartImages[i].sprite = emptyHeart;
        }
    }

    void WinGame()
    {
        if(AudioManager.instance != null) AudioManager.instance.PlayCaught();
        if(TransitionManager.instance != null)
            TransitionManager.instance.TransitionToScene("GameScene");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    void LoseGame()
    {
        if(AudioManager.instance != null) AudioManager.instance.PlayFail();
        if(TransitionManager.instance != null)
            TransitionManager.instance.TransitionToScene("GameScene");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}