using UnityEngine;

/// <summary>
/// Система атаки для врагов.
/// Обрабатывает атаку врага с уроном, коллизиями и визуальными эффектами.
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    [Header("Настройки атаки")]
    [SerializeField] private float attackDamage = 15f;        // Урон атаки врага
    [SerializeField] private float attackRange = 1.2f;       // Дальность атаки
    [SerializeField] private float attackDuration = 0.4f;    // Длительность атаки
    [SerializeField] private float attackCooldown = 1.5f;    // Перезарядка атаки
    
    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject attackEffect;         // Эффект атаки
    [SerializeField] private Color attackColor = Color.red;   // Цвет атаки для отладки
    
    [Header("Отладка")]
    [SerializeField] private bool enableDebugLogs = true;     // Включить логгирование
    [SerializeField] private bool showAttackRange = true;     // Показывать дальность атаки в редакторе
    
    // Компоненты
    private Transform player;
    private Animator animator;
    
    // Состояние атаки
    private bool isAttacking = false;
    private bool canAttack = true;
    private float lastAttackTime = 0f;
    private Vector2 attackDirection = Vector2.right;
    
    // Коллайдер атаки
    private Collider2D attackCollider;
    
    void Start()
    {
        InitializeComponents();
        SetupAttackCollider();
    }
    
    void Update()
    {
        UpdateAttackState();
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
            Debug.LogWarning("EnemyAttack: Игрок не найден! Убедитесь, что у игрока есть тег 'Player'");
        }
        
        // Получаем Animator для анимаций
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning($"EnemyAttack: На объекте {gameObject.name} отсутствует Animator. Анимации атаки не будут работать.");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyAttack: Компоненты инициализированы для {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Настраивает коллайдер для атаки
    /// </summary>
    private void SetupAttackCollider()
    {
        // Создаем дочерний объект для коллайдера атаки
        GameObject attackColliderObj = new GameObject("EnemyAttackCollider");
        attackColliderObj.transform.SetParent(transform);
        attackColliderObj.transform.localPosition = Vector3.zero;
        
        // Добавляем коллайдер атаки
        CircleCollider2D circleCollider = attackColliderObj.AddComponent<CircleCollider2D>();
        circleCollider.radius = attackRange;
        attackCollider = circleCollider;
        attackCollider.isTrigger = true;
        
        // Скрываем коллайдер атаки (он будет активироваться только во время атаки)
        attackColliderObj.SetActive(false);
        
        if (enableDebugLogs)
        {
            Debug.Log("EnemyAttack: Коллайдер атаки настроен");
        }
    }
    
    /// <summary>
    /// Обновляет состояние атаки
    /// </summary>
    private void UpdateAttackState()
    {
        // Проверяем перезарядку атаки
        if (!canAttack && Time.time - lastAttackTime >= attackCooldown)
        {
            canAttack = true;
        }
        
        // Завершаем атаку по истечении времени
        if (isAttacking && Time.time - lastAttackTime >= attackDuration)
        {
            EndAttack();
        }
    }
    
    /// <summary>
    /// Выполняет атаку (вызывается из EnemyAI)
    /// </summary>
    public void PerformAttack()
    {
        if (!canAttack || isAttacking) return;
        
        StartAttack();
    }
    
    /// <summary>
    /// Начинает атаку
    /// </summary>
    private void StartAttack()
    {
        isAttacking = true;
        canAttack = false;
        lastAttackTime = Time.time;
        
        // Определяем направление атаки к игроку
        if (player != null)
        {
            attackDirection = (player.position - transform.position).normalized;
        }
        
        // Позиционируем коллайдер атаки
        PositionAttackCollider();
        
        // Активируем коллайдер атаки
        if (attackCollider != null)
        {
            attackCollider.gameObject.SetActive(true);
        }
        
        // Запускаем анимацию атаки
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            animator.SetFloat("AttackDirectionX", attackDirection.x);
            animator.SetFloat("AttackDirectionY", attackDirection.y);
        }
        
        // Создаем визуальный эффект атаки
        CreateAttackEffect();
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyAttack: {gameObject.name} атакует в направлении {attackDirection}");
        }
    }
    
    /// <summary>
    /// Завершает атаку
    /// </summary>
    private void EndAttack()
    {
        isAttacking = false;
        
        // Деактивируем коллайдер атаки
        if (attackCollider != null)
        {
            attackCollider.gameObject.SetActive(false);
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyAttack: Атака {gameObject.name} завершена");
        }
    }
    
    /// <summary>
    /// Позиционирует коллайдер атаки в направлении атаки
    /// </summary>
    private void PositionAttackCollider()
    {
        if (attackCollider == null) return;
        
        // Позиционируем коллайдер атаки в направлении атаки
        Vector3 attackPosition = transform.position + (Vector3)(attackDirection * attackRange * 0.5f);
        attackCollider.transform.position = attackPosition;
    }
    
    /// <summary>
    /// Создает визуальный эффект атаки
    /// </summary>
    private void CreateAttackEffect()
    {
        if (attackEffect != null)
        {
            Vector3 effectPosition = transform.position + (Vector3)(attackDirection * attackRange * 0.5f);
            GameObject effect = Instantiate(attackEffect, effectPosition, Quaternion.identity);
            
            // Поворачиваем эффект в направлении атаки
            float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
            effect.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            // Уничтожаем эффект через некоторое время
            Destroy(effect, 0.5f);
        }
    }
    
    /// <summary>
    /// Обрабатывает попадание атаки по игроку
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttacking) return;
        
        // Проверяем, что попали по игроку
        if (other.CompareTag("Player"))
        {
            DealDamageToPlayer(other.gameObject);
        }
    }
    
    /// <summary>
    /// Наносит урон игроку
    /// </summary>
    private void DealDamageToPlayer(GameObject playerObj)
    {
        // Получаем компонент здоровья игрока
        HealthSystem playerHealth = playerObj.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            
            if (enableDebugLogs)
            {
                Debug.Log($"EnemyAttack: {gameObject.name} нанес {attackDamage} урона игроку");
            }
        }
        else
        {
            // Если у игрока нет системы здоровья, просто логируем попадание
            if (enableDebugLogs)
            {
                Debug.Log($"EnemyAttack: {gameObject.name} попал по игроку (нет системы здоровья)");
            }
        }
    }
    
    #region Публичные методы для других систем
    
    /// <summary>
    /// Проверяет, атакует ли враг в данный момент
    /// </summary>
    public bool IsAttacking()
    {
        return isAttacking;
    }
    
    /// <summary>
    /// Проверяет, готова ли атака к использованию
    /// </summary>
    public bool CanAttack()
    {
        return canAttack && !isAttacking;
    }
    
    /// <summary>
    /// Получает текущее направление атаки
    /// </summary>
    public Vector2 GetAttackDirection()
    {
        return attackDirection;
    }
    
    /// <summary>
    /// Устанавливает новый урон атаки
    /// </summary>
    public void SetAttackDamage(float newDamage)
    {
        attackDamage = newDamage;
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyAttack: Урон атаки изменен на {newDamage}");
        }
    }
    
    /// <summary>
    /// Устанавливает новую дальность атаки
    /// </summary>
    public void SetAttackRange(float newRange)
    {
        attackRange = newRange;
        
        if (attackCollider != null && attackCollider is CircleCollider2D circleCollider)
        {
            circleCollider.radius = newRange;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyAttack: Дальность атаки изменена на {newRange}");
        }
    }
    
    /// <summary>
    /// Устанавливает новое время перезарядки атаки
    /// </summary>
    public void SetAttackCooldown(float newCooldown)
    {
        attackCooldown = newCooldown;
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyAttack: Перезарядка атаки изменена на {newCooldown}");
        }
    }
    
    #endregion
    
    #region Отладка в редакторе
    
    void OnDrawGizmosSelected()
    {
        if (!showAttackRange) return;
        
        // Рисуем дальность атаки в редакторе
        Gizmos.color = attackColor;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Рисуем направление атаки
        if (attackDirection.magnitude > 0.1f)
        {
            Gizmos.color = Color.red;
            Vector3 attackEnd = transform.position + (Vector3)(attackDirection * attackRange);
            Gizmos.DrawLine(transform.position, attackEnd);
        }
    }
    
    #endregion
} 