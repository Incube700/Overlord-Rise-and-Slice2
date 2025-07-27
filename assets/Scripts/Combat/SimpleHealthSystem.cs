using UnityEngine;
using System.Collections;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Простая система здоровья для игрока и врагов
    /// </summary>
    public class SimpleHealthSystem : MonoBehaviour, IDamageable
    {
        [Header("Настройки здоровья")]
        [SerializeField] private int maxHealth = 100;
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
        [SerializeField] private bool destroyOnDeath = true;
        
        [Header("Отладка")]
        [SerializeField] private bool enableDebugLogs = true;
        
        // Компоненты
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Collider2D entityCollider;
        private Rigidbody2D rb2D;
        
        // События
        public System.Action<int, int> OnHealthChanged; // current, max
        public System.Action OnDeath;
        public System.Action<int> OnDamageTaken;
        public System.Action<int> OnHealed;
        
        protected virtual void Awake()
        {
            InitializeComponents();
        }
        
        protected virtual void Start()
        {
            currentHealth = maxHealth;
            
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleHealthSystem: Инициализировано здоровье {currentHealth}/{maxHealth} для {gameObject.name}");
            }
        }
        
        private void InitializeComponents()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            entityCollider = GetComponent<Collider2D>();
            rb2D = GetComponent<Rigidbody2D>();
            
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleHealthSystem: Компоненты инициализированы для {gameObject.name}");
            }
        }
        
        #region IDamageable Implementation
        
        /// <summary>
        /// Наносит урон объекту (реализация IDamageable)
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
                Debug.Log($"SimpleHealthSystem: {gameObject.name} получил {amount} урона. Здоровье: {currentHealth}/{maxHealth}");
            }
            
            // Проверяем смерть
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        /// <summary>
        /// Проверяет, может ли объект получать урон
        /// </summary>
        /// <returns>true если объект может получать урон</returns>
        public bool CanTakeDamage()
        {
            return !isInvulnerable && currentHealth > 0;
        }
        
        /// <summary>
        /// Получает текущее здоровье объекта
        /// </summary>
        /// <returns>Текущее здоровье</returns>
        public int GetCurrentHealth()
        {
            return currentHealth;
        }
        
        /// <summary>
        /// Получает максимальное здоровье объекта
        /// </summary>
        /// <returns>Максимальное здоровье</returns>
        public int GetMaxHealth()
        {
            return maxHealth;
        }
        
        #endregion
        
        #region Дополнительные методы
        
        /// <summary>
        /// Лечит объект
        /// </summary>
        /// <param name="amount">Количество здоровья для восстановления</param>
        public void Heal(int amount)
        {
            if (currentHealth <= 0) return;
            
            int oldHealth = currentHealth;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            int actualHeal = currentHealth - oldHealth;
            
            if (actualHeal > 0)
            {
                OnHealed?.Invoke(actualHeal);
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
                
                if (enableDebugLogs)
                {
                    Debug.Log($"SimpleHealthSystem: {gameObject.name} вылечен на {actualHeal}. Здоровье: {currentHealth}/{maxHealth}");
                }
            }
        }
        
        /// <summary>
        /// Полностью восстанавливает здоровье
        /// </summary>
        public void RestoreFullHealth()
        {
            Heal(maxHealth - currentHealth);
        }
        
        /// <summary>
        /// Устанавливает новое максимальное здоровье
        /// </summary>
        /// <param name="newMaxHealth">Новое максимальное здоровье</param>
        public void SetMaxHealth(int newMaxHealth)
        {
            float healthPercentage = GetHealthPercentage();
            maxHealth = newMaxHealth;
            currentHealth = Mathf.RoundToInt(maxHealth * healthPercentage);
            
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleHealthSystem: {gameObject.name} получил новое максимальное здоровье: {maxHealth}");
            }
        }
        
        /// <summary>
        /// Мгновенно убивает объект
        /// </summary>
        public void InstantKill()
        {
            currentHealth = 0;
            Die();
        }
        
        /// <summary>
        /// Проверяет, мертв ли объект
        /// </summary>
        /// <returns>true если объект мертв</returns>
        public bool IsDead()
        {
            return currentHealth <= 0;
        }
        
        /// <summary>
        /// Проверяет, неуязвим ли объект
        /// </summary>
        /// <returns>true если объект неуязвим</returns>
        public bool IsInvulnerable()
        {
            return isInvulnerable;
        }
        
        /// <summary>
        /// Получает процент здоровья (0-1)
        /// </summary>
        /// <returns>Процент здоровья</returns>
        public float GetHealthPercentage()
        {
            return maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
        }
        
        #endregion
        
        #region Приватные методы
        
        /// <summary>
        /// Эффект мигания при получении урона
        /// </summary>
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
        
        /// <summary>
        /// Временная неуязвимость после получения урона
        /// </summary>
        private IEnumerator InvulnerabilityRoutine()
        {
            isInvulnerable = true;
            
            yield return new WaitForSeconds(invulnerabilityDuration);
            
            isInvulnerable = false;
            
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleHealthSystem: {gameObject.name} больше не неуязвим");
            }
        }
        
        /// <summary>
        /// Обрабатывает смерть объекта
        /// </summary>
        protected virtual void Die()
        {
            if (currentHealth > 0) return;
            
            // Уведомляем о смерти
            OnDeath?.Invoke();
            
            // Запускаем анимацию смерти
            if (animator != null)
            {
                animator.SetTrigger("Die");
                animator.SetBool("isDead", true);
            }
            
            // Отключаем коллайдер если нужно
            if (disableColliderOnDeath && entityCollider != null)
            {
                entityCollider.enabled = false;
            }
            
            // Останавливаем движение
            if (rb2D != null)
            {
                rb2D.velocity = Vector2.zero;
                rb2D.isKinematic = true;
            }
            
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleHealthSystem: {gameObject.name} умер");
            }
            
            // Уничтожаем объект если нужно
            if (destroyOnDeath)
            {
                StartCoroutine(DestroyAfterDelay());
            }
        }
        
        /// <summary>
        /// Уничтожает объект с задержкой
        /// </summary>
        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(deathDelay);
            
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleHealthSystem: Уничтожаем {gameObject.name}");
            }
            
            Destroy(gameObject);
        }
        
        #endregion
    }
} 