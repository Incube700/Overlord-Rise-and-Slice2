using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    private Transform playerTransform;
    private Rigidbody2D rb2D;
    private EnemyHealth enemyHealth;
    private float lastAttackTime;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();
        
        // Ищем игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure player has 'Player' tag.");
        }
    }

    private void Update()
    {
        if (playerTransform == null || enemyHealth.IsDead())
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Проверяем, в зоне ли обнаружения
        if (distanceToPlayer <= detectionRange)
        {
            // Если в зоне атаки
            if (distanceToPlayer <= attackRange)
            {
                // TODO: Реализовать атаку
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                // Двигаемся к игроку
                MoveTowardsPlayer();
            }
        }
    }

    /// <summary>
    /// Движение к игроку
    /// </summary>
    private void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        Vector2 newPosition = rb2D.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb2D.MovePosition(newPosition);
    }

    /// <summary>
    /// Атака игрока (заглушка)
    /// </summary>
    private void AttackPlayer()
    {
        Debug.Log("Enemy attacks player!");
        // TODO: Реализовать логику атаки
        // - Нанести урон игроку
        // - Воспроизвести анимацию атаки
        // - Добавить звуковые эффекты
    }

    /// <summary>
    /// Возвращает расстояние до игрока
    /// </summary>
    /// <returns>Расстояние до игрока</returns>
    public float GetDistanceToPlayer()
    {
        if (playerTransform == null) return float.MaxValue;
        return Vector2.Distance(transform.position, playerTransform.position);
    }

    /// <summary>
    /// Проверяет, видит ли враг игрока
    /// </summary>
    /// <returns>True если игрок в зоне обнаружения</returns>
    public bool CanSeePlayer()
    {
        if (playerTransform == null) return false;
        return GetDistanceToPlayer() <= detectionRange;
    }

    /// <summary>
    /// Проверяет, может ли враг атаковать
    /// </summary>
    /// <returns>True если игрок в зоне атаки</returns>
    public bool CanAttackPlayer()
    {
        if (playerTransform == null) return false;
        return GetDistanceToPlayer() <= attackRange;
    }

    // Гизмо для отладки в редакторе
    private void OnDrawGizmosSelected()
    {
        // Зона обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Зона атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
} 