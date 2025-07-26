using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Полоска здоровья врага, отображаемая над ним.
/// Использует Canvas и Slider для визуализации текущего здоровья.
/// Автоматически привязывается к EnemyHealth и обновляется при изменениях.
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [Header("Настройки UI")]
    [SerializeField] private Canvas healthBarCanvas; // Canvas для полоски здоровья
    [SerializeField] private Slider healthSlider; // Слайдер здоровья
    [SerializeField] private Image fillImage; // Изображение заполнения
    [SerializeField] private Image backgroundImage; // Фоновое изображение
    
    [Header("Настройки позиционирования")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0); // Смещение от врага
    [SerializeField] private bool followTarget = true; // Следовать за целью
    [SerializeField] private bool faceCamera = true; // Поворачиваться к камере
    
    [Header("Настройки отображения")]
    [SerializeField] private bool hideWhenFull = true; // Скрывать при полном здоровье
    [SerializeField] private bool hideWhenDead = true; // Скрывать при смерти
    [SerializeField] private float fadeSpeed = 2f; // Скорость появления/исчезновения
    
    [Header("Цвета здоровья")]
    [SerializeField] private Color fullHealthColor = Color.green; // Цвет при полном здоровье
    [SerializeField] private Color halfHealthColor = Color.yellow; // Цвет при половине здоровья
    [SerializeField] private Color lowHealthColor = Color.red; // Цвет при низком здоровье
    [SerializeField] private float lowHealthThreshold = 0.25f; // Порог низкого здоровья
    
    [Header("Отладка")]
    [SerializeField] private bool enableDebugLogs = false; // Включить логгирование
    
    // Компоненты и состояние
    private EnemyHealth enemyHealth;
    private Transform targetTransform;
    private Camera mainCamera;
    private CanvasGroup canvasGroup;
    
    // Состояние анимации
    private float targetAlpha = 1f;
    private float currentAlpha = 1f;
    
    void Start()
    {
        InitializeComponents();
        SetupHealthBar();
        SubscribeToEvents();
    }
    
    void Update()
    {
        UpdatePosition();
        UpdateRotation();
        UpdateVisibility();
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
        
        // Получаем EnemyHealth
        if (targetTransform != null)
        {
            enemyHealth = targetTransform.GetComponent<EnemyHealth>();
        }
        
        // Получаем основную камеру
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        // Создаём Canvas если его нет
        if (healthBarCanvas == null)
        {
            CreateHealthBarCanvas();
        }
        
        // Получаем CanvasGroup для анимации прозрачности
        canvasGroup = healthBarCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = healthBarCanvas.gameObject.AddComponent<CanvasGroup>();
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyHealthBar: Компоненты инициализированы для {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Создаёт Canvas для полоски здоровья
    /// </summary>
    private void CreateHealthBarCanvas()
    {
        // Создаём GameObject для Canvas
        GameObject canvasObject = new GameObject("HealthBarCanvas");
        canvasObject.transform.SetParent(transform);
        canvasObject.transform.localPosition = Vector3.zero;
        
        // Добавляем Canvas
        healthBarCanvas = canvasObject.AddComponent<Canvas>();
        healthBarCanvas.renderMode = RenderMode.WorldSpace;
        healthBarCanvas.worldCamera = mainCamera;
        healthBarCanvas.sortingOrder = 10; // Поверх других UI элементов
        
        // Настраиваем размер Canvas
        RectTransform canvasRect = healthBarCanvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(2f, 0.3f);
        canvasRect.localScale = Vector3.one * 0.01f; // Масштабируем для мирового пространства
        
        // Создаём Slider
        CreateHealthSlider(canvasObject);
        
        if (enableDebugLogs)
        {
            Debug.Log("EnemyHealthBar: Canvas создан автоматически");
        }
    }
    
    /// <summary>
    /// Создаёт слайдер здоровья
    /// </summary>
    private void CreateHealthSlider(GameObject parent)
    {
        // Создаём GameObject для слайдера
        GameObject sliderObject = new GameObject("HealthSlider");
        sliderObject.transform.SetParent(parent.transform);
        
        // Настраиваем RectTransform
        RectTransform sliderRect = sliderObject.AddComponent<RectTransform>();
        sliderRect.anchorMin = Vector2.zero;
        sliderRect.anchorMax = Vector2.one;
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;
        
        // Добавляем Slider
        healthSlider = sliderObject.AddComponent<Slider>();
        healthSlider.minValue = 0f;
        healthSlider.maxValue = 1f;
        healthSlider.value = 1f;
        
        // Создаём фон
        CreateSliderBackground(sliderObject);
        
        // Создаём заполнение
        CreateSliderFill(sliderObject);
    }
    
    /// <summary>
    /// Создаёт фон слайдера
    /// </summary>
    private void CreateSliderBackground(GameObject sliderParent)
    {
        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(sliderParent.transform);
        
        RectTransform bgRect = backgroundObject.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); // Тёмно-серый фон
        
        // Устанавливаем как фон слайдера
        healthSlider.targetGraphic = backgroundImage;
    }
    
    /// <summary>
    /// Создаёт заполнение слайдера
    /// </summary>
    private void CreateSliderFill(GameObject sliderParent)
    {
        GameObject fillAreaObject = new GameObject("Fill Area");
        fillAreaObject.transform.SetParent(sliderParent.transform);
        
        RectTransform fillAreaRect = fillAreaObject.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;
        
        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(fillAreaObject.transform);
        
        RectTransform fillRect = fillObject.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        fillImage = fillObject.AddComponent<Image>();
        fillImage.color = fullHealthColor;
        
        // Устанавливаем область заполнения
        healthSlider.fillRect = fillRect;
    }
    
    /// <summary>
    /// Настраивает полоску здоровья
    /// </summary>
    private void SetupHealthBar()
    {
        if (enemyHealth != null)
        {
            UpdateHealthDisplay(enemyHealth.GetCurrentHealth(), enemyHealth.GetMaxHealth());
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
        if (canvasGroup == null) return;
        
        // Плавно изменяем прозрачность
        if (Mathf.Abs(currentAlpha - targetAlpha) > 0.01f)
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
            canvasGroup.alpha = currentAlpha;
        }
    }
    
    /// <summary>
    /// Обработчик изменения здоровья
    /// </summary>
    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        UpdateHealthDisplay(currentHealth, maxHealth);
        
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
    /// Обновляет отображение здоровья
    /// </summary>
    private void UpdateHealthDisplay(int currentHealth, int maxHealth)
    {
        if (healthSlider == null) return;
        
        float healthPercentage = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
        healthSlider.value = healthPercentage;
        
        // Обновляем цвет в зависимости от процента здоровья
        UpdateHealthColor(healthPercentage);
    }
    
    /// <summary>
    /// Обновляет цвет полоски здоровья
    /// </summary>
    private void UpdateHealthColor(float healthPercentage)
    {
        if (fillImage == null) return;
        
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
        
        fillImage.color = targetColor;
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
            enemyHealth = target.GetComponent<EnemyHealth>();
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