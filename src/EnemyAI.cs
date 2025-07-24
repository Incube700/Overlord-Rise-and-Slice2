using UnityEngine;

/// <summary>
/// Простой ИИ для врагов с преследованием игрока и атакой.
/// Поддерживает различные состояния: патрулирование, преследование, атака.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Настройки ИИ")]
    [SerializeField] private float detectionRange = 5f;       // Дальность обнаружения игрока
    [SerializeField] private float attackRange = 1.5f;       // Дальность атаки
    [SerializeField] private float moveSpeed = 2f;           // Скорость движения
    [SerializeField] private float attackCooldown = 1f;      // Перезарядка атаки
    
    [Header("Патрулирование")]
    [SerializeField] private bool enablePatrol = true;       // Включить патрулирование
    [SerializeField] private float patrolRadius = 3f;        // Радиус патрулирования
    [SerializeField] private Vector2 patrolCenter;           // Центр патрулирования
    
    [Header("Отладка")]
    [SerializeField] private bool enableDebugLogs = true;     // Включить логгирование
    [SerializeField] private bool showRanges = true;         // Показывать дальности в редакторе
    
    // Компоненты
    private Transform player;
    private Rigidbody2D rb2D;
    private HealthSystem healthSystem;
    private EnemyAttack enemyAttack;
    
    // Состояние ИИ
    private enum AIState { Patrol, Chase, Attack, Return }
    private AIState currentState = AIState.Patrol;
    
    // Переменные движения
    private Vector2 targetPosition;
    private Vector2 startPosition;
    private float lastAttackTime = 0f;
    private bool canAttack = true;
    
    // Патрулирование
    private float patrolTimer = 0f;
    private float patrolChangeTime = 3f;
    
    void Start()
    {
        InitializeComponents();
        InitializeAI();
    }
    
    void Update()
    {
        UpdateAIState();
        HandleCurrentState();
    }
    
    void FixedUpdate()
    {
        HandleMovement();
    }
    
    /// <summary>
    /// Инициализирует необходимые компоненты
    /// </summary>
    private void InitializeComponents()
    {
        // Находим игрока
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("EnemyAI: Игрок не найден! Убедитесь, что у игрока есть тег 'Player'");
        }
        
        // Получаем компоненты
        rb2D = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthSystem>();
        enemyAttack = GetComponent<EnemyAttack>();
        
        if (rb2D == null)
        {
            Debug.LogError($"EnemyAI: На объекте {gameObject.name} отсутствует Rigidbody2D!");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyAI: Компоненты инициализированы для {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Инициализирует ИИ
    /// </summary>
    private void InitializeAI()
    {
        startPosition = transform.position;
        patrolCenter = startPosition;
        targetPosition = startPosition;
        
        // Настраиваем Rigidbody2D для ИИ
        if (rb2D != null)
        {
            rb2D.gravityScale = 0f;
            rb2D.freezeRotation = true;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyAI: ИИ инициализирован для {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Обновляет состояние ИИ на основе расстояния до игрока
    /// </summary>
    private void UpdateAIState()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Проверяем перезарядку атаки
        if (!canAttack && Time.time - lastAttackTime >= attackCooldown)
        {
            canAttack = true;
        }
        
        // Определяем новое состояние
        AIState newState = currentState;
        
        if (distanceToPlayer <= attackRange && canAttack)
        {
            newState = AIState.Attack;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            newState = AIState.Chase;
        }
        else if (currentState == AIState.Chase && distanceToPlayer > detectionRange * 1.5f)
        {
            newState = AIState.Return;
        }
        else if (currentState == AIState.Return && Vector2.Distance(transform.position, startPosition) < 0.5f)
        {
            newState = AIState.Patrol;
        }
        else if (currentState == AIState.Patrol && !enablePatrol)
        {
            newState = AIState.Chase; // Если патрулирование отключено, сразу преследуем
        }
        
        // Обновляем состояние
        if (newState != currentState)
        {
            ChangeState(newState);
        }
    }
    
    /// <summary>
    /// Обрабатывает текущее состояние ИИ
    /// </summary>
    private void HandleCurrentState()
    {
        switch (currentState)
        {
            case AIState.Patrol:
                HandlePatrol();
                break;
            case AIState.Chase:
                HandleChase();
                break;
            case AIState.Attack:
                HandleAttack();
                break;
            case AIState.Return:
                HandleReturn();
                break;
        }
    }
    
    /// <summary>
    /// Обрабатывает патрулирование
    /// </summary>
    private void HandlePatrol()
    {
        patrolTimer += Time.deltaTime;
        
        if (patrolTimer >= patrolChangeTime)
        {
            // Выбираем новую случайную точку для патрулирования
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            targetPosition = patrolCenter + randomDirection * patrolRadius;
            patrolTimer = 0f;
            
            if (enableDebugLogs)
            {
                Debug.Log($"EnemyAI: {gameObject.name} патрулирует к точке {targetPosition}");
            }
        }
    }
    
    /// <summary>
    /// Обрабатывает преследование игрока
    /// </summary>
    private void HandleChase()
    {
        if (player != null)
        {
            targetPosition = player.position;
        }
    }
    
    /// <summary>
    /// Обрабатывает атаку
    /// </summary>
    private void HandleAttack()
    {
        if (enemyAttack != null && canAttack)
        {
            enemyAttack.PerformAttack();
            lastAttackTime = Time.time;
            canAttack = false;
            
            if (enableDebugLogs)
            {
                Debug.Log($"EnemyAI: {gameObject.name} атакует игрока");
            }
        }
    }
    
    /// <summary>
    /// Обрабатывает возврат к начальной позиции
    /// </summary>
    private void HandleReturn()
    {
        targetPosition = startPosition;
    }
    
    /// <summary>
    /// Обрабатывает движение врага
    /// </summary>
    private void HandleMovement()
    {
        if (rb2D == null) return;
        
        // Проверяем, не мертв ли враг
        if (healthSystem != null && healthSystem.IsDead())
        {
            rb2D.velocity = Vector2.zero;
            return;
        }
        
        // Вычисляем направление движения
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        
        // Применяем движение
        Vector2 velocity = direction * moveSpeed;
        rb2D.velocity = velocity;
        
        // Поворачиваем спрайт в направлении движения
        if (velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    
    /// <summary>
    /// Изменяет состояние ИИ
    /// </summary>
    private void ChangeState(AIState newState)
    {
        if (currentState == newState) return;
        
        AIState oldState = currentState;
        currentState = newState;
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyAI: {gameObject.name} изменил состояние с {oldState} на {newState}");
        }
        
        // Специальная обработка при смене состояния
        switch (newState)
        {
            case AIState.Patrol:
                // Сбрасываем таймер патрулирования
                patrolTimer = 0f;
                break;
            case AIState.Return:
                // Останавливаем движение при возврате
                if (rb2D != null)
                {
                    rb2D.velocity = Vector2.zero;
                }
                break;
        }
    }
    
    #region Публичные методы для других систем
    
    /// <summary>
    /// Получает текущее состояние ИИ
    /// </summary>
    public AIState GetCurrentState()
    {
        return currentState;
    }
    
    /// <summary>
    /// Проверяет, преследует ли враг игрока
    /// </summary>
    public bool IsChasing()
    {
        return currentState == AIState.Chase;
    }
    
    /// <summary>
    /// Проверяет, атакует ли враг
    /// </summary>
    public bool IsAttacking()
    {
        return currentState == AIState.Attack;
    }
    
    /// <summary>
    /// Устанавливает новую скорость движения
    /// </summary>
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyAI: Скорость движения изменена на {newSpeed}");
        }
    }
    
    /// <summary>
    /// Устанавливает новую дальность обнаружения
    /// </summary>
    public void SetDetectionRange(float newRange)
    {
        detectionRange = newRange;
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyAI: Дальность обнаружения изменена на {newRange}");
        }
    }
    
    /// <summary>
    /// Устанавливает новую дальность атаки
    /// </summary>
    public void SetAttackRange(float newRange)
    {
        attackRange = newRange;
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyAI: Дальность атаки изменена на {newRange}");
        }
    }
    
    /// <summary>
    /// Принудительно заставляет врага преследовать игрока
    /// </summary>
    public void ForceChase()
    {
        ChangeState(AIState.Chase);
    }
    
    /// <summary>
    /// Принудительно заставляет врага вернуться к начальной позиции
    /// </summary>
    public void ForceReturn()
    {
        ChangeState(AIState.Return);
    }
    
    #endregion
    
    #region Отладка в редакторе
    
    void OnDrawGizmosSelected()
    {
        if (!showRanges) return;
        
        // Рисуем дальность обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Рисуем дальность атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Рисуем зону патрулирования
        if (enablePatrol)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(patrolCenter, patrolRadius);
        }
        
        // Рисуем линию к цели
        if (currentState == AIState.Chase && player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
        else if (currentState == AIState.Patrol)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
    
    #endregion
} 