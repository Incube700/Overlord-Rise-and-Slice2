using UnityEngine;

/// <summary>
/// Основной контроллер врага, управляющий всеми его компонентами.
/// Координирует работу EnemyHealth, EnemyAI, EnemyHealthBar и анимаций.
/// Обеспечивает правильную инициализацию и взаимодействие между системами.
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("Настройки врага")]
    [SerializeField] private string enemyName = "Enemy"; // Имя врага
    [SerializeField] private int enemyLevel = 1; // Уровень врага
    
    [Header("Компоненты (автоматически найдены)")]
    [SerializeField] private EnemyHealth enemyHealth; // Система здоровья
    [SerializeField] private EnemyAI enemyAI; // Искусственный интеллект
    [SerializeField] private EnemyHealthBar healthBar; // Полоска здоровья
    [SerializeField] private Animator animator; // Аниматор
    [SerializeField] private SpriteRenderer spriteRenderer; // Рендерер спрайтов
    [SerializeField] private Rigidbody2D rb2D; // Физическое тело
    [SerializeField] private Collider2D enemyCollider; // Коллайдер
    
    [Header("Настройки инициализации")]
    [SerializeField] private bool autoSetupComponents = true; // Автоматическая настройка компонентов
    [SerializeField] private bool createHealthBarIfMissing = true; // Создавать полоску здоровья если её нет
    
    [Header("Отладка")]
    [SerializeField] private bool enableDebugLogs = true; // Включить логгирование
    [SerializeField] private bool showComponentStatus = true; // Показывать статус компонентов
    
    // Состояние врага
    private bool isInitialized = false;
    private bool isDying = false;
    
    // События для других систем
    public System.Action<EnemyController> OnEnemyInitialized; // Враг инициализирован
    public System.Action<EnemyController> OnEnemyDying; // Враг умирает
    public System.Action<EnemyController> OnEnemyDestroyed; // Враг уничтожен
    
    void Awake()
    {
        FindComponents();
        
        if (autoSetupComponents)
        {
            SetupComponents();
        }
    }
    
    void Start()
    {
        InitializeEnemy();
        SubscribeToEvents();
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
        OnEnemyDestroyed?.Invoke(this);
    }
    
    /// <summary>
    /// Находит все необходимые компоненты
    /// </summary>
    private void FindComponents()
    {
        // Находим основные компоненты
        if (enemyHealth == null)
            enemyHealth = GetComponent<EnemyHealth>();
        
        if (enemyAI == null)
            enemyAI = GetComponent<EnemyAI>();
        
        if (animator == null)
            animator = GetComponent<Animator>();
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (rb2D == null)
            rb2D = GetComponent<Rigidbody2D>();
        
        if (enemyCollider == null)
            enemyCollider = GetComponent<Collider2D>();
        
        // Ищем полоску здоровья в дочерних объектах
        if (healthBar == null)
            healthBar = GetComponentInChildren<EnemyHealthBar>();
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyController: Компоненты найдены для {enemyName}");
        }
    }
    
    /// <summary>
    /// Настраивает компоненты врага
    /// </summary>
    private void SetupComponents()
    {
        // Настраиваем Rigidbody2D
        SetupRigidbody();
        
        // Настраиваем коллайдер
        SetupCollider();
        
        // Создаём полоску здоровья если нужно
        if (createHealthBarIfMissing && healthBar == null)
        {
            CreateHealthBar();
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyController: Компоненты настроены для {enemyName}");
        }
    }
    
    /// <summary>
    /// Настраивает Rigidbody2D для врага
    /// </summary>
    private void SetupRigidbody()
    {
        if (rb2D == null) return;
        
        // Настройки для 2D top-down игры
        rb2D.gravityScale = 0f; // Отключаем гравитацию
        rb2D.freezeRotation = true; // Замораживаем вращение
        rb2D.bodyType = RigidbodyType2D.Dynamic; // Динамическое тело
        rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Непрерывное обнаружение коллизий
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyController: Rigidbody2D настроен для {enemyName}");
        }
    }
    
    /// <summary>
    /// Настраивает коллайдер врага
    /// </summary>
    private void SetupCollider()
    {
        if (enemyCollider == null) return;
        
        // Убеждаемся что коллайдер не является триггером (для физических взаимодействий)
        enemyCollider.isTrigger = false;
        
        // Устанавливаем тег Enemy если его нет
        if (!gameObject.CompareTag("Enemy"))
        {
            gameObject.tag = "Enemy";
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyController: Коллайдер настроен для {enemyName}");
        }
    }
    
    /// <summary>
    /// Создаёт полоску здоровья для врага
    /// </summary>
    private void CreateHealthBar()
    {
        // Создаём GameObject для полоски здоровья
        GameObject healthBarObject = new GameObject("HealthBar");
        healthBarObject.transform.SetParent(transform);
        healthBarObject.transform.localPosition = Vector3.zero;
        
        // Добавляем компонент EnemyHealthBar
        healthBar = healthBarObject.AddComponent<EnemyHealthBar>();
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyController: Полоска здоровья создана для {enemyName}");
        }
    }
    
    /// <summary>
    /// Инициализирует врага
    /// </summary>
    private void InitializeEnemy()
    {
        if (isInitialized) return;
        
        // Проверяем наличие критически важных компонентов
        ValidateComponents();
        
        // Устанавливаем имя объекта
        if (string.IsNullOrEmpty(enemyName))
        {
            enemyName = gameObject.name;
        }
        else
        {
            gameObject.name = enemyName;
        }
        
        isInitialized = true;
        
        // Уведомляем о завершении инициализации
        OnEnemyInitialized?.Invoke(this);
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyController: {enemyName} инициализирован");
            
            if (showComponentStatus)
            {
                LogComponentStatus();
            }
        }
    }
    
    /// <summary>
    /// Проверяет наличие критически важных компонентов
    /// </summary>
    private void ValidateComponents()
    {
        bool hasErrors = false;
        
        if (enemyHealth == null)
        {
            Debug.LogError($"EnemyController: {enemyName} не имеет компонента EnemyHealth!");
            hasErrors = true;
        }
        
        if (enemyAI == null)
        {
            Debug.LogWarning($"EnemyController: {enemyName} не имеет компонента EnemyAI");
        }
        
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"EnemyController: {enemyName} не имеет компонента SpriteRenderer");
        }
        
        if (rb2D == null)
        {
            Debug.LogError($"EnemyController: {enemyName} не имеет компонента Rigidbody2D!");
            hasErrors = true;
        }
        
        if (enemyCollider == null)
        {
            Debug.LogError($"EnemyController: {enemyName} не имеет компонента Collider2D!");
            hasErrors = true;
        }
        
        if (hasErrors)
        {
            Debug.LogError($"EnemyController: {enemyName} имеет критические ошибки компонентов!");
        }
    }
    
    /// <summary>
    /// Выводит статус всех компонентов в лог
    /// </summary>
    private void LogComponentStatus()
    {
        Debug.Log($"=== Статус компонентов {enemyName} ===");
        Debug.Log($"EnemyHealth: {(enemyHealth != null ? "✓" : "✗")}");
        Debug.Log($"EnemyAI: {(enemyAI != null ? "✓" : "✗")}");
        Debug.Log($"EnemyHealthBar: {(healthBar != null ? "✓" : "✗")}");
        Debug.Log($"Animator: {(animator != null ? "✓" : "✗")}");
        Debug.Log($"SpriteRenderer: {(spriteRenderer != null ? "✓" : "✗")}");
        Debug.Log($"Rigidbody2D: {(rb2D != null ? "✓" : "✗")}");
        Debug.Log($"Collider2D: {(enemyCollider != null ? "✓" : "✗")}");
        Debug.Log("=====================================");
    }
    
    /// <summary>
    /// Подписывается на события компонентов
    /// </summary>
    private void SubscribeToEvents()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath += OnEnemyHealthDeath;
            enemyHealth.OnDamageTaken += OnEnemyDamageTaken;
        }
    }
    
    /// <summary>
    /// Отписывается от событий компонентов
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath -= OnEnemyHealthDeath;
            enemyHealth.OnDamageTaken -= OnEnemyDamageTaken;
        }
    }
    
    /// <summary>
    /// Обработчик смерти врага
    /// </summary>
    private void OnEnemyHealthDeath()
    {
        if (isDying) return;
        
        isDying = true;
        
        // Уведомляем о начале процесса смерти
        OnEnemyDying?.Invoke(this);
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyController: {enemyName} начал процесс смерти");
        }
    }
    
    /// <summary>
    /// Обработчик получения урона
    /// </summary>
    private void OnEnemyDamageTaken(int damage)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyController: {enemyName} получил {damage} урона");
        }
        
        // Здесь можно добавить дополнительные эффекты при получении урона
        // Например, звуковые эффекты, частицы, временные бонусы и т.д.
    }
    
    #region Публичные методы
    
    /// <summary>
    /// Получает компонент здоровья
    /// </summary>
    public EnemyHealth GetEnemyHealth()
    {
        return enemyHealth;
    }
    
    /// <summary>
    /// Получает компонент ИИ
    /// </summary>
    public EnemyAI GetEnemyAI()
    {
        return enemyAI;
    }
    
    /// <summary>
    /// Получает полоску здоровья
    /// </summary>
    public EnemyHealthBar GetHealthBar()
    {
        return healthBar;
    }
    
    /// <summary>
    /// Получает аниматор
    /// </summary>
    public Animator GetAnimator()
    {
        return animator;
    }
    
    /// <summary>
    /// Проверяет, инициализирован ли враг
    /// </summary>
    public bool IsInitialized()
    {
        return isInitialized;
    }
    
    /// <summary>
    /// Проверяет, умирает ли враг
    /// </summary>
    public bool IsDying()
    {
        return isDying;
    }
    
    /// <summary>
    /// Получает имя врага
    /// </summary>
    public string GetEnemyName()
    {
        return enemyName;
    }
    
    /// <summary>
    /// Получает уровень врага
    /// </summary>
    public int GetEnemyLevel()
    {
        return enemyLevel;
    }
    
    /// <summary>
    /// Устанавливает уровень врага
    /// </summary>
    public void SetEnemyLevel(int level)
    {
        enemyLevel = Mathf.Max(1, level);
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyController: Уровень {enemyName} установлен на {enemyLevel}");
        }
    }
    
    /// <summary>
    /// Принудительно переинициализирует врага
    /// </summary>
    public void Reinitialize()
    {
        isInitialized = false;
        isDying = false;
        
        FindComponents();
        
        if (autoSetupComponents)
        {
            SetupComponents();
        }
        
        InitializeEnemy();
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyController: {enemyName} переинициализирован");
        }
    }
    
    #endregion
    
    #region События Unity для отладки
    
    /// <summary>
    /// Проверка корректности настроек в инспекторе
    /// </summary>
    void OnValidate()
    {
        if (string.IsNullOrEmpty(enemyName))
        {
            enemyName = gameObject.name;
        }
        
        if (enemyLevel <= 0)
        {
            enemyLevel = 1;
        }
    }
    
    #endregion
} 