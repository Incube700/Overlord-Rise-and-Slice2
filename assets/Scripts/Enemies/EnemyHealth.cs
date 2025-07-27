using UnityEngine;
using System.Collections;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Система здоровья врага с поддержкой IDamageable интерфейса
    /// </summary>
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [Header("Настройки здоровья")]
        [SerializeField] private int maxHealth = 30;
        [SerializeField] private int currentHealth;
        
        [Header("Настройки неуязвимости")]
        [SerializeField] private float invulnerabilityDuration = 0.5f;
        [SerializeField] private bool isInvulnerable = false;
        
        [Header("Настройки визуальных эффектов")]
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private float damageFlashDuration = 0.1f;
        [SerializeField] private int damageFlashCount = 3;
        
        [Header("Настройки смерти")]
        [SerializeField] private float deathDelay = 0.5f;
        [SerializeField] private bool disableColliderOnDeath = true;
        
        [Header("Отладка")]
        [SerializeField] private bool enableDebugLogs = true;
        
        // Компоненты
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Collider2D enemyCollider;
        private Rigidbody2D rb2D;
        
        // События
        public System.Action<int, int> OnHealthChanged; // current, max
        public System.Action OnDeath;
        public System.Action<int> OnDamageTaken;
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Start()
        {
            currentHealth = maxHealth;
            
            if (enableDebugLogs)
            {
                Debug.Log($"EnemyHealth: Инициализировано здоровье {currentHealth}/{maxHealth}");
            }
        }
        
        private void InitializeComponents()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            enemyCollider = GetComponent<Collider2D>();
            rb2D = GetComponent<Rigidbody2D>();
            
            if (enableDebugLogs)
            {
                Debug.Log("EnemyHealth: Компоненты инициализированы");
            }
        }
        
        #region IDamageable Implementation
        
        /// <summary>
        /// Наносит урон врагу (реализация IDamageable)
        /// </summary>
        /// <param name="amount">Количество урона</param>
        public void TakeDamage(int amount)
        {
            if (!CanTakeDamage()) return;
            
            // Наносим урон
            currentHealth = Mathf.Max(0, currentHealth - amount);
            
            // Уведомляем о получении урона
            OnDamageTaken?.Invoke(amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            // Запускаем эффекты
            StartCoroutine(DamageFlashRoutine());
            StartCoroutine(InvulnerabilityRoutine());
            
            // Запускаем анимацию получения урона
            if (animator != null)
            {
                animator.SetTrigger("TakeDamage");
            }
            
            if (enableDebugLogs)
            {
                Debug.Log($"EnemyHealth: Получен урон {amount}. Здоровье: {currentHealth}/{maxHealth}");
            }
            
            // Проверяем смерть
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        /// <summary>
        /// Проверяет, может ли враг получать урон
        /// </summary>
        /// <returns>true если может получать урон</returns>
        public bool CanTakeDamage()
        {
            return !isInvulnerable && currentHealth > 0;
        }
        
        /// <summary>
        /// Получает текущее здоровье врага
        /// </summary>
        /// <returns>Текущее здоровье</returns>
        public int GetCurrentHealth()
        {
            return currentHealth;
        }
        
        /// <summary>
        /// Получает максимальное здоровье врага
        /// </summary>
        /// <returns>Максимальное здоровье</returns>
        public int GetMaxHealth()
        {
            return maxHealth;
        }
        
        #endregion
        
        private IEnumerator DamageFlashRoutine()
        {
            if (spriteRenderer == null) yield break;
            
            Color originalColor = spriteRenderer.color;
            
            for (int i = 0; i < damageFlashCount; i++)
            {
                spriteRenderer.color = damageColor;
                yield return new WaitForSeconds(damageFlashDuration);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(damageFlashDuration);
            }
        }
        
        private IEnumerator InvulnerabilityRoutine()
        {
            isInvulnerable = true;
            
            yield return new WaitForSeconds(invulnerabilityDuration);
            
            isInvulnerable = false;
            
            if (enableDebugLogs)
            {
                Debug.Log("EnemyHealth: Неуязвимость закончилась");
            }
        }
        
        private void Die()
        {
            if (enableDebugLogs)
            {
                Debug.Log("EnemyHealth: Враг умер!");
            }
            
            // Уведомляем о смерти
            OnDeath?.Invoke();
            
            // Запускаем анимацию смерти
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }
            
            // Отключаем коллайдер
            if (disableColliderOnDeath && enemyCollider != null)
            {
                enemyCollider.enabled = false;
            }
            
            // Останавливаем движение
            if (rb2D != null)
            {
                rb2D.velocity = Vector2.zero;
                rb2D.isKinematic = true;
            }
            
            // Уничтожаем объект через задержку
            StartCoroutine(DestroyAfterDelay());
        }
        
        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(deathDelay);
            
            if (enableDebugLogs)
            {
                Debug.Log("EnemyHealth: Уничтожение врага");
            }
            
            Destroy(gameObject);
        }
        
        #region Публичные методы для других систем
        
        /// <summary>
        /// Восстанавливает здоровье врага
        /// </summary>
        /// <param name="amount">Количество здоровья для восстановления</param>
        public void RestoreHealth(int amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            if (enableDebugLogs)
            {
                Debug.Log($"EnemyHealth: Восстановлено {amount} здоровья. Здоровье: {currentHealth}/{maxHealth}");
            }
        }
        
        /// <summary>
        /// Устанавливает максимальное здоровье
        /// </summary>
        /// <param name="newMaxHealth">Новое максимальное здоровье</param>
        public void SetMaxHealth(int newMaxHealth)
        {
            maxHealth = newMaxHealth;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            if (enableDebugLogs)
            {
                Debug.Log($"EnemyHealth: Максимальное здоровье изменено на {maxHealth}");
            }
        }
        
        /// <summary>
        /// Мгновенно убивает врага
        /// </summary>
        public void InstantKill()
        {
            currentHealth = 0;
            TakeDamage(0); // Запускает процесс смерти
        }
        
        /// <summary>
        /// Проверяет, мертв ли враг
        /// </summary>
        /// <returns>true если враг мертв</returns>
        public bool IsDead()
        {
            return currentHealth <= 0;
        }
        
        /// <summary>
        /// Проверяет, неуязвим ли враг
        /// </summary>
        /// <returns>true если враг неуязвим</returns>
        public bool IsInvulnerable()
        {
            return isInvulnerable;
        }
        
        /// <summary>
        /// Получает процент оставшегося здоровья
        /// </summary>
        /// <returns>Процент здоровья (0-1)</returns>
        public float GetHealthPercentage()
        {
            return maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
        }
        
        #endregion
    }
} 