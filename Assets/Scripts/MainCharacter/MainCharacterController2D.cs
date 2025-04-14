using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController2D : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Dictionary<SpriteRenderer, Coroutine> activeCoroutines = new Dictionary<SpriteRenderer, Coroutine>();

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float rollSpeed = 8f;
    private bool isGrounded = true;
    private bool isRolling = true;
    private bool isInvincible = false;
    [SerializeField] private float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.5f;
    private bool isDodging = false;
    [SerializeField] private AudioClip attackSound;
    private AudioSource audioSource;

    public LayerMask enemyLayer;

    private float dodgeTimer = 0f;
    private Collider2D playerCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isRolling = false;
        playerCollider = GetComponent<Collider2D>();

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isRolling)
        {
            Move();
            HandleAttack();
        }
        HandleRoll();
        if (!isDodging)
        {
            Move();
        }

        HandleDodge();
    }
    private void HandleDodge()
    {

        if (Input.GetKeyDown(KeyCode.Q) && !isDodging)
        {
            StartDodge();
        }

        if (isDodging)
        {
            dodgeTimer += Time.deltaTime;

            if (dodgeTimer >= dodgeDuration)
            {
                EndDodge();
            }
        }
    }
    private void StartDodge()
    {
        isDodging = true;
        dodgeTimer = 0f;

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        animator.SetTrigger("Roll");

        float dodgeDirection = spriteRenderer.flipX ? -1f : 1f;
        rb.velocity = new Vector2(dodgeDirection * dodgeSpeed, rb.velocity.y);
    }

    private void EndDodge()
    {
        isDodging = false;

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);

        rb.velocity = Vector2.zero;
    }


    private void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        Vector2 velocity = rb.velocity;

        rb.velocity = new Vector2(moveInput * moveSpeed, velocity.y);

        if (moveInput > 0)
        { spriteRenderer.flipX = false; }
        else if (moveInput < 0)
        { spriteRenderer.flipX = true; }

        if (moveInput != 0)
        {
            animator.SetBool("IsRunning", true);
        }
        else animator.SetBool("IsRunning", false);
    }



    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isRolling)
        {
            nextAttackTime = Time.time + attackCooldown;
            animator.SetTrigger("Attack");

            audioSource.PlayOneShot(attackSound);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 1.5f);
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();
                    Health enemyHealth = enemy.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        FindObjectOfType<ScreenShake>().StartCoroutine(FindObjectOfType<ScreenShake>().Shake());
                    }
                    if (enemyController != null)
                    {
                        enemyHealth.TakeDamage(20f);
                    }

                    SpriteRenderer enemySprite = enemy.GetComponent<SpriteRenderer>();
                    if (enemySprite != null)
                    {
                        StartCoroutine(FlashRed(enemySprite));
                    }
                }

            }
            foreach (Collider2D boss in hitEnemies)
            {
                if (boss.CompareTag("Enemy"))
                {
                    BossController bossController = boss.GetComponent<BossController>();
                    BossHealth bossHealth = boss.GetComponent<BossHealth>();
                    if (bossHealth != null)
                    {
                        FindObjectOfType<ScreenShake>().StartCoroutine(FindObjectOfType<ScreenShake>().Shake());
                    }
                    if (bossController != null)
                    {
                        bossHealth.TakeDamage(20f);
                    }

                    SpriteRenderer bossSprite = boss.GetComponent<SpriteRenderer>();
                    if (bossSprite != null)
                    {
                        StartCoroutine(FlashRed(bossSprite));
                    }
                }
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

        }
    }

    private IEnumerator FlashRed(SpriteRenderer sprite)
    {
        if (sprite == null)
        {
            yield break;
        }

        if (activeCoroutines.ContainsKey(sprite))
        {
            if (activeCoroutines[sprite] != null)
            {
                StopCoroutine(activeCoroutines[sprite]);
            }
            activeCoroutines.Remove(sprite);
        }

        Coroutine newCoroutine = StartCoroutine(FlashRedCoroutine(sprite));
        activeCoroutines[sprite] = newCoroutine;
    }

    private IEnumerator FlashRedCoroutine(SpriteRenderer sprite)
    {

        if (sprite == null)
        {
            yield break;
        }

        Color originalColor = sprite.color;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sprite.color = originalColor;

        if (activeCoroutines.ContainsKey(sprite))
        {
            activeCoroutines[sprite] = null;
        }


    }

    private void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isRolling && isGrounded && !isInvincible)
        {
            isRolling = true;
            isInvincible = true;
            animator.SetTrigger("Roll");

            float rollDirection = spriteRenderer.flipX ? -1 : 1;
            rb.velocity = new Vector2(rollSpeed * rollDirection, rb.velocity.y);

            Invoke("EndRoll", 0.5f);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }

    private void EndRoll()
    {
        isRolling = false;
        rb.velocity = new Vector2(0, rb.velocity.y);

        isInvincible = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("IsFalling", false);
        }
    }
}
