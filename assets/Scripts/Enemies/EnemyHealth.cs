using UnityEngine;
using System.Collections;

/// <summary>
/// Система здоровья врага с анимацией урона, смерти и событиями для UI.
/// Обрабатывает получение урона, неуязвимость, анимации и уничтожение объекта.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Настройки здоровья")]
    [SerializeField] private int maxHealth = 50; // Максимальное здоровье
    [SerializeField] private int currentHealth; // Текущее здоровье
    
    [Header("Настройки неуязвимости")]
    [SerializeField] private float invulnerabilityDuration = 0.2f; // Длительность неуязвимости после урона
    [SerializeField] private bool isInvulnerable = false; // Состояние неуязвимости
    
    [Header("Визуальные эффекты урона")]
    [SerializeField] private Color damageColor = Color.red; // Цвет при получении урона
    [SerializeField] private float damageFlashDuration = 0.1f; // Длительность вспышки урона
    [SerializeField] private int damageFlashCount = 3; // Количество вспышек
    
    [Header("Настройки смерти")]
    [SerializeField] private float deathDelay = 2f; // Задержка перед уничтожением после смерти
    [SerializeField] private bool disableColliderOnDeath = true; // Отключать коллайдер при смерти
    
    [Header("Отладка")]
    [SerializeField] private bool enableDebugLogs = true; // Включить логгирование
    
    // Состояние
    private bool isDead = false;
    
    // Компоненты
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Animator animator;
    private Collider2D enemyCollider;
    private EnemyAI enemyAI;
    
    // События для UI и других систем
    public System.Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
    public System.Action OnDeath; // Уведомление о смерти
    public System.Action<int> OnDamageTaken; // Уведомление о получении урона
    
    void Start()
    {
        InitializeComponents();
        InitializeHealth();
    }
    
    /// <summary>
    /// Инициализирует компоненты
    /// </summary>
    private void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();
        enemyAI = GetComponent<EnemyAI>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyHealth: Компоненты инициализированы для {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Инициализирует здоровье
    /// </summary>
    private void InitializeHealth()
    {
        currentHealth = maxHealth;
        
        // Уведомляем о начальном состоянии здоровья
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyHealth: Здоровье инициализировано {currentHealth}/{maxHealth}");
        }
    }
    
    /// <summary>
    /// Наносит урон врагу
    /// </summary>
    /// <param name="amount">Количество урона</param>
    public void TakeDamage(int amount)
    {
        // Проверяем возможность получения урона
        if (isDead || isInvulnerable || amount <= 0)
        {
            if (enableDebugLogs && amount <= 0)
            {
                Debug.LogWarning($"EnemyHealth: Попытка нанести некорректный урон: {amount}");
            }
            return;
        }
        
        // Применяем урон
        int actualDamage = Mathf.Min(amount, currentHealth);
        currentHealth -= actualDamage;
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyHealth: {gameObject.name} получил {actualDamage} урона. Здоровье: {currentHealth}/{maxHealth}");
        }
        
        // Уведомляем о получении урона
        OnDamageTaken?.Invoke(actualDamage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Запускаем визуальные эффекты урона
        StartCoroutine(DamageFlashRoutine());
        
        // Запускаем неуязвимость
        StartCoroutine(InvulnerabilityRoutine());
        
        // Проверяем смерть
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Анимация получения урона (если есть аниматор)
            if (animator != null)
            {
                animator.SetTrigger("TakeDamage");
            }
        }
    }
    
    /// <summary>
    /// Обрабатывает смерть врага
    /// </summary>
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyHealth: {gameObject.name} умер");
        }
        
        // Отключаем коллайдер
        if (disableColliderOnDeath && enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }
        
        // Отключаем ИИ
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }
        
        // Запускаем анимацию смерти
        if (animator != null)
        {
            animator.SetTrigger("Death");
            animator.SetBool("isDead", true);
        }
        
        // Уведомляем о смерти
        OnDeath?.Invoke();
        
        // Уничтожаем объект через задержку
        Destroy(gameObject, deathDelay);
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyHealth: {gameObject.name} будет уничтожен через {deathDelay} секунд");
        }
    }
    
    /// <summary>
    /// Корутина вспышки при получении урона
    /// </summary>
    private IEnumerator DamageFlashRoutine()
    {
        if (spriteRenderer == null) yield break;
        
        for (int i = 0; i < damageFlashCount; i++)
        {
            // Меняем цвет на цвет урона
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(damageFlashDuration);
            
            // Возвращаем оригинальный цвет
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(damageFlashDuration);
        }
    }
    
    /// <summary>
    /// Корутина неуязвимости после получения урона
    /// </summary>
    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyHealth: {gameObject.name} стал неуязвимым на {invulnerabilityDuration} секунд");
        }
        
        yield return new WaitForSeconds(invulnerabilityDuration);
        
        isInvulnerable = false;
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyHealth: {gameObject.name} больше не неуязвим");
        }
    }
    
    /// <summary>
    /// Восстанавливает здоровье
    /// </summary>
    /// <param name="amount">Количество восстанавливаемого здоровья</param>
    public void RestoreHealth(int amount)
    {
        if (isDead || amount <= 0) return;
        
        int oldHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        int actualRestore = currentHealth - oldHealth;
        
        if (actualRestore > 0)
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            if (enableDebugLogs)
            {
                Debug.Log($"EnemyHealth: {gameObject.name} восстановил {actualRestore} здоровья. Здоровье: {currentHealth}/{maxHealth}");
            }
        }
    }
    
    /// <summary>
    /// Устанавливает максимальное здоровье
    /// </summary>
    /// <param name="newMaxHealth">Новое максимальное здоровье</param>
    public void SetMaxHealth(int newMaxHealth)
    {
        if (newMaxHealth <= 0) return;
        
        float healthRatio = (float)currentHealth / maxHealth;
        maxHealth = newMaxHealth;
        currentHealth = Mathf.RoundToInt(maxHealth * healthRatio);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyHealth: Максимальное здоровье изменено на {maxHealth}. Текущее: {currentHealth}");
        }
    }
    
    #region Публичные методы для других систем
    
    /// <summary>
    /// Проверяет, мёртв ли враг
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }
    
    /// <summary>
    /// Проверяет, неуязвим ли враг
    /// </summary>
    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }
    
    /// <summary>
    /// Получает текущее здоровье
    /// </summary>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    /// <summary>
    /// Получает максимальное здоровье
    /// </summary>
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    /// <summary>
    /// Получает процент здоровья (0.0 - 1.0)
    /// </summary>
    public float GetHealthPercentage()
    {
        return maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
    }
    
    /// <summary>
    /// Мгновенно убивает врага (для отладки)
    /// </summary>
    public void InstantKill()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"EnemyHealth: {gameObject.name} мгновенно убит");
        }
        
        currentHealth = 0;
        Die();
    }
    
    #endregion
    
    #region События Unity
    
    /// <summary>
    /// Отображает информацию о здоровье в инспекторе
    /// </summary>
    void OnValidate()
    {
        if (maxHealth <= 0)
        {
            maxHealth = 1;
        }
        
        if (Application.isPlaying)
        {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
        else
        {
            currentHealth = maxHealth;
        }
    }
    
    #endregion
} 