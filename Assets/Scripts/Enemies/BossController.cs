using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Transform player;
    public float chaseRange = 5f;
    public float attackRange = 1.5f;
    public float moveSpeed = 3f;
    private Vector3 originalScale;
    [SerializeField] private BossBar bossBar;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool hasDealtDamage = false;
    private Dictionary<SpriteRenderer, Coroutine> EnemyActiveCoroutines = new Dictionary<SpriteRenderer, Coroutine>();
    private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioClip attackSound;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        UpdateHealthBarVisibility(distanceToPlayer);

        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            Chase();
        }
        else
        {
            Idle();
        }


    }

    void LateUpdate()
    {
        transform.localScale = originalScale;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void Chase()
    {
        if (isAttacking) return;

        isChasing = true;
        animator.SetBool("IsRunning", true);
        animator.SetBool("IsAttacking", false);

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        bossBar.SetVisibility(true);
    }
    private void UpdateHealthBarVisibility(float distanceToPlayer)
    {
        if (distanceToPlayer <= chaseRange)
        {
            bossBar.SetVisibility(true);
        }
        else
        {
            bossBar.SetVisibility(false);
        }
    }


    private void Attack()
    {
        if (isAttacking) return;

        audioSource.PlayOneShot(attackSound);

        isAttacking = true;
        isChasing = false;

        rb.velocity = Vector2.zero;
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAttacking", true);




        Invoke("DealDamage", 0.6f);
        Invoke("EndAttack", 0.6f);
    }

    public void DealDamage()
    {
        if (hasDealtDamage) return;
        hasDealtDamage = true;

        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D player in hitPlayers)
        {
            if (player.CompareTag("Player"))
            {
                MainCharacterHealth playerHealth = player.GetComponent<MainCharacterHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(50f);
                    FindObjectOfType<ScreenShake>().StartCoroutine(FindObjectOfType<ScreenShake>().Shake());
                }

            }
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        hasDealtDamage = false;
    }


    private void Idle()
    {
        isChasing = false;
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAttacking", false);

        rb.velocity = Vector2.zero;
        bossBar.SetVisibility(false);
    }
}
