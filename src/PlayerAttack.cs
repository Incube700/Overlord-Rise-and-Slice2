using UnityEngine;

/// <summary>
/// Система атаки игрока для базовой боевой механики.
/// Обрабатывает атаку мечом с анимацией, уроном и коллизиями.
/// Интегрируется с PlayerMovement для получения направления атаки.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    [Header("Настройки атаки")]
    [SerializeField] private float attackDamage = 25f;        // Урон атаки
    [SerializeField] private float attackRange = 1.5f;       // Дальность атаки
    [SerializeField] private float attackCooldown = 0.5f;    // Время перезарядки атаки
    [SerializeField] private float attackDuration = 0.3f;    // Длительность атаки
    
    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject attackEffect;         // Эффект атаки (опционально)
    [SerializeField] private Color attackColor = Color.red;   // Цвет атаки для отладки
    
    [Header("Отладка")]
    [SerializeField] private bool enableDebugLogs = true;     // Включить логгирование
    [SerializeField] private bool showAttackRange = true;     // Показывать дальность атаки в редакторе
    
    // Компоненты
    private PlayerMovement playerMovement;
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
        HandleAttackInput();
        UpdateAttackState();
    }
    
    /// <summary>
    /// Инициализирует необходимые компоненты
    /// </summary>
    private void InitializeComponents()
    {
        // Получаем PlayerMovement для направления атаки
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError($"PlayerAttack: На объекте {gameObject.name} отсутствует PlayerMovement!");
        }
        
        // Получаем Animator для анимаций
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning($"PlayerAttack: На объекте {gameObject.name} отсутствует Animator. Анимации атаки не будут работать.");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerAttack: Компоненты инициализированы для {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Настраивает коллайдер для атаки
    /// </summary>
    private void SetupAttackCollider()
    {
        // Создаем дочерний объект для коллайдера атаки
        GameObject attackColliderObj = new GameObject("AttackCollider");
        attackColliderObj.transform.SetParent(transform);
        attackColliderObj.transform.localPosition = Vector3.zero;
        
        // Добавляем коллайдер атаки
        attackCollider = attackColliderObj.AddComponent<CircleCollider2D>();
        attackCollider.radius = attackRange;
        attackCollider.isTrigger = true;
        
        // Скрываем коллайдер атаки (он будет активироваться только во время атаки)
        attackColliderObj.SetActive(false);
        
        if (enableDebugLogs)
        {
            Debug.Log("PlayerAttack: Коллайдер атаки настроен");
        }
    }
    
    /// <summary>
    /// Обрабатывает ввод для атаки
    /// </summary>
    private void HandleAttackInput()
    {
        // Проверяем нажатие левой кнопки мыши или пробела
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (canAttack && !isAttacking)
            {
                StartAttack();
            }
            else if (enableDebugLogs)
            {
                Debug.Log("PlayerAttack: Атака недоступна (перезарядка или уже атакуем)");
            }
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
            if (enableDebugLogs)
            {
                Debug.Log("PlayerAttack: Атака готова к использованию");
            }
        }
        
        // Завершаем атаку по истечении времени
        if (isAttacking && Time.time - lastAttackTime >= attackDuration)
        {
            EndAttack();
        }
    }
    
    /// <summary>
    /// Начинает атаку
    /// </summary>
    private void StartAttack()
    {
        isAttacking = true;
        canAttack = false;
        lastAttackTime = Time.time;
        
        // Получаем направление атаки от PlayerMovement
        if (playerMovement != null)
        {
            Vector2 movementDirection = playerMovement.GetMovementDirection();
            if (movementDirection.magnitude > 0.1f)
            {
                attackDirection = movementDirection.normalized;
            }
            else
            {
                // Если игрок не двигается, используем последнее направление
                attackDirection = playerMovement.GetLastMovementDirection();
                if (attackDirection.magnitude < 0.1f)
                {
                    attackDirection = Vector2.right; // По умолчанию вправо
                }
            }
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
            Debug.Log($"PlayerAttack: Атака начата в направлении {attackDirection}");
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
            Debug.Log("PlayerAttack: Атака завершена");
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
    /// Обрабатывает попадание атаки по врагу
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttacking) return;
        
        // Проверяем, что попали по врагу
        if (other.CompareTag("Enemy"))
        {
            DealDamage(other.gameObject);
        }
    }
    
    /// <summary>
    /// Наносит урон врагу
    /// </summary>
    private void DealDamage(GameObject enemy)
    {
        // Получаем компонент здоровья врага
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(attackDamage);
            
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerAttack: Нанесен урон {attackDamage} врагу {enemy.name}");
            }
        }
        else
        {
            // Если у врага нет системы здоровья, просто логируем попадание
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerAttack: Попадание по врагу {enemy.name} (нет системы здоровья)");
            }
        }
    }
    
    #region Публичные методы для других систем
    
    /// <summary>
    /// Проверяет, атакует ли игрок в данный момент
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
            Debug.Log($"PlayerAttack: Урон атаки изменен на {newDamage}");
        }
    }
    
    /// <summary>
    /// Устанавливает новую дальность атаки
    /// </summary>
    public void SetAttackRange(float newRange)
    {
        attackRange = newRange;
        
        if (attackCollider != null)
        {
            attackCollider.radius = newRange;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerAttack: Дальность атаки изменена на {newRange}");
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
            Debug.Log($"PlayerAttack: Перезарядка атаки изменена на {newCooldown}");
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