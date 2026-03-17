using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFish : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 4f;
    public float acceleration = 8f;
    public float detectionRange = 6f;
    public float patrolRadius = 2f;
    public float attackCooldownTime = 1.5f;

    [Header("Animation")]
    public Animator enemyAnim;
    public string walkAnim = "Walk";
    public string attackAnim = "Attack";
    private SpriteRenderer spriteRenderer;

    private Transform player;
    private Vector2 patrolTarget;
    private float attackCooldown = 0f;
    private Rigidbody2D rb;
    private Vector2 startPosition;
    private bool isAttacking = false;
    private float attackAnimTimer = 0f;
    public float attackAnimDuration = 0.5f;

    private float collisionTimer = 0f;
    private bool isCollidingWithPlayer = false;

    private Vector2 currentVelocity;
    private Vector2 targetVelocity;

    private float patrolWaitTimer = 0f;
    private float patrolWaitDuration = 0f;
    private bool isWaiting = false;

    private Vector2 lastPlayerPos;
    private Vector2 playerVelocity;

    private enum EnemyState { Patrol, Chase }
    private EnemyState currentState = EnemyState.Patrol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.linearDamping = 3f;
        startPosition = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("FishPlayer");
        if(playerObj != null)
        {
            player = playerObj.transform;
            lastPlayerPos = player.position;
        }

        SetNewPatrolTarget();
    }

    void Update()
    {
        if(player == null) return;

        attackCooldown -= Time.deltaTime;
        HandleCollision();

        playerVelocity = ((Vector2)player.position - lastPlayerPos) / Time.deltaTime;
        lastPlayerPos = player.position;

        if(isAttacking)
        {
            attackAnimTimer += Time.deltaTime;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 5f);
            if(attackAnimTimer >= attackAnimDuration)
            {
                isAttacking = false;
                attackAnimTimer = 0f;
            }
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        currentState = distToPlayer <= detectionRange ? EnemyState.Chase : EnemyState.Patrol;

        switch(currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                PlayAnim(walkAnim);
                break;
            case EnemyState.Chase:
                Chase();
                PlayAnim(walkAnim);
                break;
        }

        if(rb.linearVelocity.x < -0.1f) spriteRenderer.flipX = true;
        else if(rb.linearVelocity.x > 0.1f) spriteRenderer.flipX = false;
    }

    void HandleCollision()
    {
        if(isCollidingWithPlayer)
        {
            collisionTimer += Time.deltaTime;
            if(collisionTimer >= 0.5f && attackCooldown <= 0)
            {
                collisionTimer = 0f;
                TriggerAttack();
            }
        }
    }

    void TriggerAttack()
    {
        isAttacking = true;
        attackAnimTimer = 0f;
        PlayAnim(attackAnim);
        attackCooldown = attackCooldownTime;

        FishPlayer fishPlayer = player.GetComponent<FishPlayer>();
        if(fishPlayer != null) fishPlayer.TakeHit();
    }

    void Patrol()
    {
        if(isWaiting)
        {
            patrolWaitTimer += Time.deltaTime;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 3f);
            if(patrolWaitTimer >= patrolWaitDuration)
            {
                isWaiting = false;
                patrolWaitTimer = 0f;
                SetNewPatrolTarget();
            }
            return;
        }

        Vector2 dir = (patrolTarget - (Vector2)transform.position).normalized;
        targetVelocity = dir * moveSpeed * 0.5f;
        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            targetVelocity,
            acceleration * Time.deltaTime
        );

        if(Vector2.Distance(transform.position, patrolTarget) < 0.4f)
        {
            isWaiting = true;
            patrolWaitDuration = Random.Range(0.5f, 2f);
        }
    }

    void Chase()
    {
        Vector2 predictedPos = (Vector2)player.position + playerVelocity * 0.3f;
        Vector2 dir = (predictedPos - (Vector2)transform.position).normalized;

        targetVelocity = dir * moveSpeed;
        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            targetVelocity,
            acceleration * Time.deltaTime
        );

        float dist = Vector2.Distance(transform.position, player.position);
        if(dist < 2f)
        {
            rb.linearVelocity = Vector2.MoveTowards(
                rb.linearVelocity,
                targetVelocity * 1.5f,
                acceleration * 2f * Time.deltaTime
            );
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("FishPlayer"))
        {
            isCollidingWithPlayer = true;
        }

        if(collision.gameObject.CompareTag("Wall"))
        {
            rb.linearVelocity = Vector2.Reflect(rb.linearVelocity, collision.contacts[0].normal);
            SetNewPatrolTarget();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("FishPlayer"))
        {
            isCollidingWithPlayer = false;
            collisionTimer = 0f;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("FishPlayer") && attackCooldown <= 0 && !isAttacking)
        {
            TriggerAttack();
        }
    }

    void SetNewPatrolTarget()
    {
        patrolTarget = startPosition + new Vector2(
            Random.Range(-patrolRadius, patrolRadius),
            Random.Range(-patrolRadius, patrolRadius)
        );
    }

    void PlayAnim(string animName)
    {
        if(enemyAnim != null)
            enemyAnim.Play(animName);
    }
}