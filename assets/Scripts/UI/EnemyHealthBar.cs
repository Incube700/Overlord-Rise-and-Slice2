using UnityEngine;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Упрощённая полоска здоровья врага без зависимости от UnityEngine.UI.
    /// Использует SpriteRenderer для отображения здоровья над врагом.
    /// Автоматически привязывается к EnemyHealth и обновляется при изменениях.
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
{
    [Header("Настройки позиционирования")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0); // Смещение от врага
    [SerializeField] private bool followTarget = true; // Следовать за целью
    [SerializeField] private bool faceCamera = true; // Поворачиваться к камере
    
    [Header("Настройки отображения")]
    [SerializeField] private bool hideWhenFull = true; // Скрывать при полном здоровье
    [SerializeField] private bool hideWhenDead = true; // Скрывать при смерти
    [SerializeField] private float fadeSpeed = 2f; // Скорость появления/исчезновения
    [SerializeField] private Vector2 barSize = new Vector2(1f, 0.1f); // Размер полоски
    
    [Header("Цвета здоровья")]
    [SerializeField] private Color fullHealthColor = Color.green; // Цвет при полном здоровье
    [SerializeField] private Color halfHealthColor = Color.yellow; // Цвет при половине здоровья
    [SerializeField] private Color lowHealthColor = Color.red; // Цвет при низком здоровье
    [SerializeField] private Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f); // Цвет фона
    [SerializeField] private float lowHealthThreshold = 0.25f; // Порог низкого здоровья
    
    [Header("Отладка")]
    [SerializeField] private bool enableDebugLogs = false; // Включить логгирование
    
    // Компоненты и состояние
    private SimpleHealthSystem enemyHealth;
    private Transform targetTransform;
    private Camera mainCamera;
    
    // Визуальные компоненты
    private GameObject backgroundBar;
    private GameObject healthBar;
    private SpriteRenderer backgroundRenderer;
    private SpriteRenderer healthRenderer;
    
    // Состояние анимации
    private float targetAlpha = 1f;
    private float currentAlpha = 1f;
    private float currentHealthPercentage = 1f;
    
    void Start()
    {
        InitializeComponents();
        CreateHealthBar();
        SetupHealthBar();
        SubscribeToEvents();
    }
    
    void Update()
    {
        UpdatePosition();
        UpdateRotation();
        UpdateVisibility();
        UpdateHealthDisplay();
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    /// <summary>
    /// Инициализирует компоненты
    /// </summary>
    private void InitializeComponents()
    {
        // Получаем целевой объект (родитель или заданный)
        if (targetTransform == null)
        {
            targetTransform = transform.parent;
        }
        
        // Получаем SimpleHealthSystem
        if (targetTransform != null)
        {
            enemyHealth = targetTransform.GetComponent<SimpleHealthSystem>();
        }
        
        // Получаем основную камеру
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyHealthBar: Компоненты инициализированы для {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Создаёт визуальную полоску здоровья
    /// </summary>
    private void CreateHealthBar()
    {
        // Создаём фоновую полоску
        backgroundBar = new GameObject("HealthBar_Background");
        backgroundBar.transform.SetParent(transform);
        backgroundBar.transform.localPosition = Vector3.zero;
        backgroundBar.transform.localScale = new Vector3(barSize.x, barSize.y, 1f);
        
        backgroundRenderer = backgroundBar.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = CreateBarSprite();
        backgroundRenderer.color = backgroundColor;
        backgroundRenderer.sortingOrder = 10;
        
        // Создаём полоску здоровья
        healthBar = new GameObject("HealthBar_Fill");
        healthBar.transform.SetParent(transform);
        healthBar.transform.localPosition = Vector3.zero;
        healthBar.transform.localScale = new Vector3(barSize.x, barSize.y, 1f);
        
        healthRenderer = healthBar.AddComponent<SpriteRenderer>();
        healthRenderer.sprite = CreateBarSprite();
        healthRenderer.color = fullHealthColor;
        healthRenderer.sortingOrder = 11;
        
        if (enableDebugLogs)
        {
            Debug.Log("EnemyHealthBar: Визуальная полоска здоровья создана");
        }
    }
    
    /// <summary>
    /// Создаёт простой спрайт для полоски
    /// </summary>
    private Sprite CreateBarSprite()
    {
        // Создаём белую текстуру 1x1
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        // Создаём спрайт из текстуры
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }
    
    /// <summary>
    /// Настраивает полоску здоровья
    /// </summary>
    private void SetupHealthBar()
    {
        if (enemyHealth != null)
        {
            currentHealthPercentage = enemyHealth.GetHealthPercentage();
            UpdateHealthVisual();
        }
        
        // Устанавливаем начальную видимость
        if (hideWhenFull && enemyHealth != null && enemyHealth.GetHealthPercentage() >= 1f)
        {
            targetAlpha = 0f;
        }
    }
    
    /// <summary>
    /// Подписывается на события EnemyHealth
    /// </summary>
    private void SubscribeToEvents()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged += OnHealthChanged;
            enemyHealth.OnDeath += OnEnemyDeath;
            
            if (enableDebugLogs)
            {
                Debug.Log("EnemyHealthBar: Подписался на события EnemyHealth");
            }
        }
    }
    
    /// <summary>
    /// Отписывается от событий EnemyHealth
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged -= OnHealthChanged;
            enemyHealth.OnDeath -= OnEnemyDeath;
        }
    }
    
    /// <summary>
    /// Обновляет позицию полоски здоровья
    /// </summary>
    private void UpdatePosition()
    {
        if (!followTarget || targetTransform == null) return;
        
        transform.position = targetTransform.position + offset;
    }
    
    /// <summary>
    /// Обновляет поворот полоски здоровья
    /// </summary>
    private void UpdateRotation()
    {
        if (!faceCamera || mainCamera == null) return;
        
        // Поворачиваем к камере
        Vector3 lookDirection = mainCamera.transform.position - transform.position;
        lookDirection.y = 0; // Убираем наклон по Y
        
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
    
    /// <summary>
    /// Обновляет видимость полоски здоровья
    /// </summary>
    private void UpdateVisibility()
    {
        // Плавно изменяем прозрачность
        if (Mathf.Abs(currentAlpha - targetAlpha) > 0.01f)
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
            
            if (backgroundRenderer != null)
            {
                Color bgColor = backgroundRenderer.color;
                bgColor.a = currentAlpha * 0.8f;
                backgroundRenderer.color = bgColor;
            }
            
            if (healthRenderer != null)
            {
                Color healthColor = healthRenderer.color;
                healthColor.a = currentAlpha;
                healthRenderer.color = healthColor;
            }
        }
    }
    
    /// <summary>
    /// Обновляет отображение здоровья
    /// </summary>
    private void UpdateHealthDisplay()
    {
        if (enemyHealth == null || healthBar == null) return;
        
        float targetHealthPercentage = enemyHealth.GetHealthPercentage();
        
        // Плавно обновляем процент здоровья
        if (Mathf.Abs(currentHealthPercentage - targetHealthPercentage) > 0.01f)
        {
            currentHealthPercentage = Mathf.MoveTowards(currentHealthPercentage, targetHealthPercentage, 2f * Time.deltaTime);
            UpdateHealthVisual();
        }
    }
    
    /// <summary>
    /// Обновляет визуальное отображение здоровья
    /// </summary>
    private void UpdateHealthVisual()
    {
        if (healthBar == null || healthRenderer == null) return;
        
        // Обновляем масштаб полоски здоровья
        Vector3 scale = healthBar.transform.localScale;
        scale.x = barSize.x * currentHealthPercentage;
        healthBar.transform.localScale = scale;
        
        // Сдвигаем полоску влево при уменьшении
        Vector3 position = healthBar.transform.localPosition;
        position.x = -barSize.x * (1f - currentHealthPercentage) * 0.5f;
        healthBar.transform.localPosition = position;
        
        // Обновляем цвет в зависимости от процента здоровья
        UpdateHealthColor(currentHealthPercentage);
    }
    
    /// <summary>
    /// Обработчик изменения здоровья
    /// </summary>
    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        // Показываем полоску при получении урона
        if (hideWhenFull && currentHealth < maxHealth)
        {
            targetAlpha = 1f;
        }
        else if (hideWhenFull && currentHealth >= maxHealth)
        {
            targetAlpha = 0f;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyHealthBar: Здоровье обновлено {currentHealth}/{maxHealth}");
        }
    }
    
    /// <summary>
    /// Обработчик смерти врага
    /// </summary>
    private void OnEnemyDeath()
    {
        if (hideWhenDead)
        {
            targetAlpha = 0f;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("EnemyHealthBar: Враг умер");
        }
    }
    
    /// <summary>
    /// Обновляет цвет полоски здоровья
    /// </summary>
    private void UpdateHealthColor(float healthPercentage)
    {
        if (healthRenderer == null) return;
        
        Color targetColor;
        
        if (healthPercentage <= lowHealthThreshold)
        {
            // Низкое здоровье - красный
            targetColor = lowHealthColor;
        }
        else if (healthPercentage <= 0.5f)
        {
            // Интерполяция между красным и жёлтым
            float t = (healthPercentage - lowHealthThreshold) / (0.5f - lowHealthThreshold);
            targetColor = Color.Lerp(lowHealthColor, halfHealthColor, t);
        }
        else
        {
            // Интерполяция между жёлтым и зелёным
            float t = (healthPercentage - 0.5f) / 0.5f;
            targetColor = Color.Lerp(halfHealthColor, fullHealthColor, t);
        }
        
        // Сохраняем текущую прозрачность
        targetColor.a = healthRenderer.color.a;
        healthRenderer.color = targetColor;
    }
    
    #region Публичные методы
    
    /// <summary>
    /// Устанавливает цель для отслеживания
    /// </summary>
    public void SetTarget(Transform target)
    {
        UnsubscribeFromEvents();
        
        targetTransform = target;
        if (target != null)
        {
            enemyHealth = target.GetComponent<SimpleHealthSystem>();
        }
        
        SubscribeToEvents();
        SetupHealthBar();
    }
    
    /// <summary>
    /// Принудительно показывает полоску здоровья
    /// </summary>
    public void Show()
    {
        targetAlpha = 1f;
    }
    
    /// <summary>
    /// Принудительно скрывает полоску здоровья
    /// </summary>
    public void Hide()
    {
        targetAlpha = 0f;
    }
    
    #endregion
}
}