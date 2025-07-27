using UnityEngine;

/// <summary>
/// Система движения игрока для top-down 2D игры.
/// Обрабатывает ввод с клавиатуры, физическое движение через Rigidbody2D,
/// и коллизии с окружающими объектами.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 5f; // Скорость движения игрока
    
    [Header("Отладка")]
    [SerializeField] private bool enableDebugLogs = true; // Включить логгирование для отладки
    
    // Компоненты
    private Rigidbody2D rb2D;
    
    // Переменные ввода
    private Vector2 movementInput;
    private Vector2 lastMovementDirection;
    
    void Start()
    {
        InitializeComponents();
        ConfigureRigidbody();
    }
    
    void Update()
    {
        HandleInput();
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
        rb2D = GetComponent<Rigidbody2D>();
        
        if (rb2D == null)
        {
            Debug.LogError($"PlayerMovement: На объекте {gameObject.name} отсутствует компонент Rigidbody2D!");
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerMovement: Компоненты инициализированы для {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Настраивает Rigidbody2D для top-down движения
    /// </summary>
    private void ConfigureRigidbody()
    {
        if (rb2D == null) return;
        
        // Отключаем гравитацию для top-down вида
        rb2D.gravityScale = 0f;
        
        // Замораживаем вращение, чтобы игрок не поворачивался при столкновениях
        rb2D.freezeRotation = true;
        
        if (enableDebugLogs)
        {
            Debug.Log("PlayerMovement: Rigidbody2D настроен для top-down движения");
        }
    }
    
    /// <summary>
    /// Обрабатывает ввод с клавиатуры
    /// </summary>
    private void HandleInput()
    {
        // Получаем ввод по горизонтали и вертикали (WASD или стрелки)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        // Сохраняем текущий ввод
        movementInput = new Vector2(horizontalInput, verticalInput);
        
        // Сохраняем направление движения для других систем
        if (movementInput.magnitude > 0.1f)
        {
            lastMovementDirection = movementInput.normalized;
        }
    }
    
    /// <summary>
    /// Обрабатывает физическое движение игрока
    /// </summary>
    private void HandleMovement()
    {
        if (rb2D == null) return;
        
        // Создаем вектор движения
        Vector2 movement = movementInput;
        
        // Нормализуем диагональное движение, чтобы скорость была одинаковой во всех направлениях
        if (movement.magnitude > 1f)
        {
            movement = movement.normalized;
        }
        
        // Применяем скорость движения
        movement *= moveSpeed;
        
        // Устанавливаем скорость Rigidbody2D
        rb2D.linearVelocity = movement;
    }
    
    /// <summary>
    /// Обрабатывает столкновения с другими объектами
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerMovement: Столкновение с объектом '{collision.gameObject.name}' (тег: '{collision.gameObject.tag}')");
        }
        
        // Специальная обработка столкновений со стенами
        if (collision.gameObject.CompareTag("Wall"))
        {
            HandleWallCollision(collision);
        }
    }
    
    /// <summary>
    /// Обрабатывает начало триггерных столкновений
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerMovement: Вход в триггер '{other.gameObject.name}' (тег: '{other.gameObject.tag}')");
        }
        
        // Здесь можно добавить обработку триггеров (например, сбор предметов, переходы между уровнями)
    }
    
    /// <summary>
    /// Специальная обработка столкновений со стенами
    /// </summary>
    private void HandleWallCollision(Collision2D collision)
    {
        if (enableDebugLogs)
        {
            Vector2 contactPoint = collision.contacts[0].point;
            Vector2 contactNormal = collision.contacts[0].normal;
            
            Debug.Log($"PlayerMovement: Столкновение со стеной в точке {contactPoint}, нормаль: {contactNormal}");
        }
        
        // Здесь можно добавить специальную логику для стен:
        // - Звуковые эффекты
        // - Частицы
        // - Проверка на возможность разрушения стены
    }
    
    #region Публичные методы для других систем
    
    /// <summary>
    /// Возвращает текущее направление движения (нормализованное)
    /// </summary>
    public Vector2 GetMovementDirection()
    {
        return movementInput.magnitude > 0.1f ? movementInput.normalized : Vector2.zero;
    }
    
    /// <summary>
    /// Возвращает последнее направление движения (для анимаций когда игрок стоит)
    /// </summary>
    public Vector2 GetLastMovementDirection()
    {
        return lastMovementDirection;
    }
    
    /// <summary>
    /// Проверяет, движется ли игрок в данный момент
    /// </summary>
    public bool IsMoving()
    {
        return movementInput.magnitude > 0.1f;
    }
    
    /// <summary>
    /// Получает текущую скорость игрока
    /// </summary>
    public float GetCurrentSpeed()
    {
        return rb2D != null ? rb2D.linearVelocity.magnitude : 0f;
    }
    
    /// <summary>
    /// Устанавливает новую скорость движения (для временных эффектов)
    /// </summary>
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerMovement: Скорость движения изменена на {newSpeed}");
        }
    }
    
    /// <summary>
    /// Останавливает игрока (например, для кат-сцен или паузы)
    /// </summary>
    public void StopMovement()
    {
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
        }
        
        movementInput = Vector2.zero;
    }
    
    #endregion
} 