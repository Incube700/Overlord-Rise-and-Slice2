using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Универсальная система здоровья для игрока и врагов.
/// Поддерживает получение урона, лечение, события смерти и визуальные эффекты.
/// </summary>
public class HealthSystem : MonoBehaviour
{
    [Header("Настройки здоровья")]
    [SerializeField] private float maxHealth = 100f;          // Максимальное здоровье
    [SerializeField] private float currentHealth;             // Текущее здоровье
    
    [Header("Визуальные эффекты")]
    [SerializeField] private GameObject damageEffect;         // Эффект получения урона
    [SerializeField] private GameObject deathEffect;          // Эффект смерти
    [SerializeField] private SpriteRenderer spriteRenderer;   // Спрайт для эффекта мигания
    
    [Header("Отладка")]
    [SerializeField] private bool enableDebugLogs = true;     // Включить логгирование
    [SerializeField] private bool showHealthBar = true;       // Показывать полоску здоровья в редакторе
    
    // События
    [System.Serializable]
    public class HealthEvent : UnityEvent<float> { }
    
    [Header("События")]
    public UnityEvent onDeath;                                // Событие смерти
    public HealthEvent onHealthChanged;                       // Событие изменения здоровья
    public HealthEvent onDamageTaken;                         // Событие получения урона
    public HealthEvent onHealed;                              // Событие лечения
    
    // Состояние
    private bool isDead = false;
    private bool isInvulnerable = false;
    private float invulnerabilityTime = 0f;
    private float invulnerabilityDuration = 0.5f;
    
    // Эффекты
    private Color originalColor;
    private float damageFlashTime = 0.1f;
    private float lastDamageTime = 0f;
    
    void Start()
    {
        InitializeHealth();
        InitializeComponents();
    }
    
    void Update()
    {
        UpdateInvulnerability();
        UpdateDamageFlash();
    }
    
    /// <summary>
    /// Инициализирует здоровье
    /// </summary>
    private void InitializeHealth()
    {
        currentHealth = maxHealth;
        
        if (enableDebugLogs)
        {
            Debug.Log($"HealthSystem: Здоровье инициализировано для {gameObject.name} - {currentHealth}/{maxHealth}");
        }
    }
    
    /// <summary>
    /// Инициализирует компоненты
    /// </summary>
    private void InitializeComponents()
    {
        // Получаем SpriteRenderer если не назначен
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        // Сохраняем оригинальный цвет для эффекта мигания
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"HealthSystem: Компоненты инициализированы для {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Обновляет состояние неуязвимости
    /// </summary>
    private void UpdateInvulnerability()
    {
        if (isInvulnerable && Time.time >= invulnerabilityTime)
        {
            isInvulnerable = false;
            
            if (enableDebugLogs)
            {
                Debug.Log($"HealthSystem: {gameObject.name} больше не неуязвим");
            }
        }
    }
    
    /// <summary>
    /// Обновляет эффект мигания при получении урона
    /// </summary>
    private void UpdateDamageFlash()
    {
        if (spriteRenderer == null) return;
        
        if (Time.time - lastDamageTime < damageFlashTime)
        {
            // Мигаем красным цветом
            spriteRenderer.color = Color.red;
        }
        else
        {
            // Возвращаем оригинальный цвет
            spriteRenderer.color = originalColor;
        }
    }
    
    /// <summary>
    /// Наносит урон объекту
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead || isInvulnerable) return;
        
        // Применяем урон
        currentHealth = Mathf.Max(0, currentHealth - damage);
        lastDamageTime = Time.time;
        
        // Активируем неуязвимость
        isInvulnerable = true;
        invulnerabilityTime = Time.time + invulnerabilityDuration;
        
        // Вызываем события
        onHealthChanged?.Invoke(currentHealth);
        onDamageTaken?.Invoke(damage);
        
        // Создаем эффект урона
        CreateDamageEffect();
        
        if (enableDebugLogs)
        {
            Debug.Log($"HealthSystem: {gameObject.name} получил {damage} урона. Здоровье: {currentHealth}/{maxHealth}");
        }
        
        // Проверяем смерть
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Лечит объект
    /// </summary>
    public void Heal(float healAmount)
    {
        if (isDead) return;
        
        float oldHealth = currentHealth;
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        float actualHeal = currentHealth - oldHealth;
        
        if (actualHeal > 0)
        {
            // Вызываем события
            onHealthChanged?.Invoke(currentHealth);
            onHealed?.Invoke(actualHeal);
            
            if (enableDebugLogs)
            {
                Debug.Log($"HealthSystem: {gameObject.name} вылечен на {actualHeal}. Здоровье: {currentHealth}/{maxHealth}");
            }
        }
    }
    
    /// <summary>
    /// Полностью восстанавливает здоровье
    /// </summary>
    public void RestoreFullHealth()
    {
        if (isDead) return;
        
        float healAmount = maxHealth - currentHealth;
        Heal(healAmount);
    }
    
    /// <summary>
    /// Увеличивает максимальное здоровье
    /// </summary>
    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount; // Также увеличиваем текущее здоровье
        
        onHealthChanged?.Invoke(currentHealth);
        
        if (enableDebugLogs)
        {
            Debug.Log($"HealthSystem: {gameObject.name} получил +{amount} к максимальному здоровью. Новый максимум: {maxHealth}");
        }
    }
    
    /// <summary>
    /// Обрабатывает смерть объекта
    /// </summary>
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Вызываем событие смерти
        onDeath?.Invoke();
        
        // Создаем эффект смерти
        CreateDeathEffect();
        
        if (enableDebugLogs)
        {
            Debug.Log($"HealthSystem: {gameObject.name} умер");
        }
        
        // Уничтожаем объект через некоторое время
        Destroy(gameObject, 2f);
    }
    
    /// <summary>
    /// Создает эффект получения урона
    /// </summary>
    private void CreateDamageEffect()
    {
        if (damageEffect != null)
        {
            GameObject effect = Instantiate(damageEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }
    
    /// <summary>
    /// Создает эффект смерти
    /// </summary>
    private void CreateDeathEffect()
    {
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 3f);
        }
    }
    
    #region Публичные методы для других систем
    
    /// <summary>
    /// Проверяет, мертв ли объект
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }
    
    /// <summary>
    /// Проверяет, неуязвим ли объект
    /// </summary>
    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }
    
    /// <summary>
    /// Получает текущее здоровье
    /// </summary>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    
    /// <summary>
    /// Получает максимальное здоровье
    /// </summary>
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    
    /// <summary>
    /// Получает процент здоровья (0-1)
    /// </summary>
    public float GetHealthPercentage()
    {
        return maxHealth > 0 ? currentHealth / maxHealth : 0f;
    }
    
    /// <summary>
    /// Устанавливает новое максимальное здоровье
    /// </summary>
    public void SetMaxHealth(float newMaxHealth)
    {
        float healthPercentage = GetHealthPercentage();
        maxHealth = newMaxHealth;
        currentHealth = maxHealth * healthPercentage;
        
        onHealthChanged?.Invoke(currentHealth);
        
        if (enableDebugLogs)
        {
            Debug.Log($"HealthSystem: {gameObject.name} получил новое максимальное здоровье: {maxHealth}");
        }
    }
    
    /// <summary>
    /// Устанавливает время неуязвимости
    /// </summary>
    public void SetInvulnerabilityDuration(float duration)
    {
        invulnerabilityDuration = duration;
    }
    
    /// <summary>
    /// Принудительно делает объект неуязвимым
    /// </summary>
    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
        if (invulnerable)
        {
            invulnerabilityTime = Time.time + invulnerabilityDuration;
        }
    }
    
    #endregion
    
    #region Отладка в редакторе
    
    void OnDrawGizmosSelected()
    {
        if (!showHealthBar) return;
        
        // Рисуем полоску здоровья над объектом
        Vector3 position = transform.position + Vector3.up * 1.5f;
        
        // Фон полоски здоровья
        Gizmos.color = Color.red;
        Gizmos.DrawCube(position, new Vector3(2f, 0.2f, 0.1f));
        
        // Текущее здоровье
        Gizmos.color = Color.green;
        float healthWidth = 2f * GetHealthPercentage();
        Gizmos.DrawCube(position - Vector3.right * (1f - healthWidth * 0.5f), 
                        new Vector3(healthWidth, 0.2f, 0.1f));
    }
    
    #endregion
} 