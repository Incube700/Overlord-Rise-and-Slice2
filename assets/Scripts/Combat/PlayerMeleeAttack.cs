using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 0.4f;
    [SerializeField] private Vector2 attackOffset = new Vector2(1f, 0f);
    [SerializeField] private LayerMask enemyLayer;

    [Header("Visual Feedback")]
    [SerializeField] private Color attackColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    private float lastAttackTime = -999f;
    private Transform playerTransform;
    private SpriteRenderer playerSprite;
    private Color originalColor;

    private void Awake()
    {
        playerTransform = transform;
        playerSprite = GetComponent<SpriteRenderer>();
        if (playerSprite != null)
        {
            originalColor = playerSprite.color;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log("Player attacking with Mouse0!");
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    private void PerformAttack()
    {
        // Определяем направление атаки (по направлению взгляда игрока или движения)
        Vector2 attackDirection = playerTransform.right;
        Vector2 attackCenter = (Vector2)playerTransform.position + (Vector2)(playerTransform.right * attackOffset.x) + (Vector2)(playerTransform.up * attackOffset.y);

        Debug.Log($"Attack center: {attackCenter}, Range: {attackRange}");

        // Ищем всех врагов в зоне атаки
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCenter, attackRange, enemyLayer);
        Debug.Log($"Found {hitEnemies.Length} colliders in attack range");

        bool hitEnemy = false;
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Debug.Log($"Hit enemy: {enemy.name}");
                
                // Пытаемся нанести урон через EnemyHealth
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage((int)attackDamage);
                    hitEnemy = true;
                    Debug.Log($"Dealt {attackDamage} damage to {enemy.name}");
                }
                else
                {
                    Debug.LogWarning($"Enemy {enemy.name} has no EnemyHealth component!");
                }
            }
        }

        if (!hitEnemy)
        {
            Debug.Log("No enemies hit in attack range");
        }

        // Визуальная обратная связь
        StartCoroutine(FlashAttack());
    }

    private System.Collections.IEnumerator FlashAttack()
    {
        if (playerSprite != null)
        {
            playerSprite.color = attackColor;
            yield return new WaitForSeconds(flashDuration);
            playerSprite.color = originalColor;
        }
    }

    // Гизмо для отображения зоны удара в редакторе
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null)
            playerTransform = transform;
        Vector2 attackCenter = (Vector2)playerTransform.position + (Vector2)(playerTransform.right * attackOffset.x) + (Vector2)(playerTransform.up * attackOffset.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCenter, attackRange);
    }

    // Гизмо всегда видимый для отладки
    private void OnDrawGizmos()
    {
        if (playerTransform == null)
            playerTransform = transform;
        Vector2 attackCenter = (Vector2)playerTransform.position + (Vector2)(playerTransform.right * attackOffset.x) + (Vector2)(playerTransform.up * attackOffset.y);
        Gizmos.color = new Color(1, 0, 0, 0.3f); // Полупрозрачный красный
        Gizmos.DrawWireSphere(attackCenter, attackRange);
    }
} 