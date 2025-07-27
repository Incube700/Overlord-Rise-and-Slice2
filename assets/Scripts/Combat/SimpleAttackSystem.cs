using UnityEngine;
using System.Collections;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Простая система атаки для игрока и врагов
    /// </summary>
    public class SimpleAttackSystem : MonoBehaviour
    {
        [Header("Настройки атаки")]
        [SerializeField] private int attackDamage = 10;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float attackDuration = 0.3f;
        
        [Header("Настройки визуальных эффектов")]
        [SerializeField] private Color attackColor = Color.red;
        [SerializeField] private float attackFlashDuration = 0.1f;
        [SerializeField] private int attackFlashCount = 3;
        
        [Header("Настройки отладки")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool showAttackGizmos = true;
        [SerializeField] private LayerMask attackLayerMask = -1;
        
        // Компоненты
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private SimpleHealthSystem healthSystem;
        
        // Состояние атаки
        private bool isAttacking = false;
        private float lastAttackTime = 0f;
        private Vector2 attackDirection;
        
        // События
        public System.Action OnAttackStarted;
        public System.Action OnAttackEnded;
        public System.Action<IDamageable> OnEnemyHit;
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void InitializeComponents()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            healthSystem = GetComponent<SimpleHealthSystem>();
            
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleAttackSystem: Компоненты инициализированы для {gameObject.name}");
            }
        }
        
        /// <summary>
        /// Выполняет атаку в указанном направлении
        /// </summary>
        /// <param name="direction">Направление атаки</param>
        public void PerformAttack(Vector2 direction)
        {
            if (!CanAttack()) return;
            
            if (direction.magnitude > 0.1f)
            {
                StartAttack(direction.normalized);
            }
        }
        
        /// <summary>
        /// Выполняет атаку в направлении движения
        /// </summary>
        public void PerformAttack()
        {
            if (!CanAttack()) return;
            
            Vector2 attackDir = GetAttackDirection();
            
            if (attackDir.magnitude > 0.1f)
            {
                StartAttack(attackDir);
            }
            else if (enableDebugLogs)
            {
                Debug.Log("SimpleAttackSystem: Нет направления для атаки");
            }
        }
        
        /// <summary>
        /// Проверяет, может ли объект атаковать
        /// </summary>
        /// <returns>true если атака доступна</returns>
        public bool CanAttack()
        {
            if (isAttacking) return false;
            if (Time.time < lastAttackTime + attackCooldown) return false;
            if (healthSystem != null && healthSystem.IsDead()) return false;
            
            return true;
        }
        
        /// <summary>
        /// Получает направление атаки (переопределяется в наследниках)
        /// </summary>
        /// <returns>Направление атаки</returns>
        protected virtual Vector2 GetAttackDirection()
        {
            return Vector2.right; // По умолчанию атакуем вправо
        }
        
        /// <summary>
        /// Начинает атаку
        /// </summary>
        /// <param name="direction">Направление атаки</param>
        protected virtual void StartAttack(Vector2 direction)
        {
            attackDirection = direction;
            isAttacking = true;
            lastAttackTime = Time.time;
            
            // Уведомляем другие системы
            OnAttackStarted?.Invoke();
            
            // Запускаем анимацию
            if (animator != null)
            {
                animator.SetTrigger("Attack");
                animator.SetBool("isAttacking", true);
            }
            
            // Запускаем корутину атаки
            StartCoroutine(AttackRoutine());
            
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleAttackSystem: Начата атака в направлении {attackDirection}");
            }
        }
        
        /// <summary>
        /// Корутина атаки
        /// </summary>
        private IEnumerator AttackRoutine()
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < attackDuration)
            {
                // Проверяем попадания
                CheckForHits();
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Завершаем атаку
            EndAttack();
        }
        
        /// <summary>
        /// Проверяет попадания в зоне атаки
        /// </summary>
        protected virtual void CheckForHits()
        {
            // Определяем центр атаки
            Vector2 attackCenter = (Vector2)transform.position + attackDirection * (attackRange * 0.5f);
            
            // Ищем объекты в зоне атаки
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackCenter, attackRange * 0.5f, attackLayerMask);
            
            foreach (Collider2D hitCollider in hitColliders)
            {
                // Пропускаем самого атакующего
                if (hitCollider.gameObject == gameObject) continue;
                
                // Проверяем, может ли объект получать урон
                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                
                if (damageable != null && damageable.CanTakeDamage())
                {
                    // Наносим урон
                    damageable.TakeDamage(attackDamage);
                    
                    // Уведомляем о попадании
                    OnEnemyHit?.Invoke(damageable);
                    
                    // Запускаем визуальные эффекты
                    StartCoroutine(AttackHitEffects(hitCollider.transform.position));
                    
                    if (enableDebugLogs)
                    {
                        Debug.Log($"SimpleAttackSystem: Попадание! Нанесено {attackDamage} урона {hitCollider.name}");
                    }
                }
            }
        }
        
        /// <summary>
        /// Эффекты при попадании
        /// </summary>
        /// <param name="hitPosition">Позиция попадания</param>
        private IEnumerator AttackHitEffects(Vector3 hitPosition)
        {
            // Эффект вспышки на атакующем
            if (spriteRenderer != null)
            {
                Color originalColor = spriteRenderer.color;
                
                for (int i = 0; i < attackFlashCount; i++)
                {
                    spriteRenderer.color = attackColor;
                    yield return new WaitForSeconds(attackFlashDuration);
                    spriteRenderer.color = originalColor;
                    yield return new WaitForSeconds(attackFlashDuration);
                }
            }
            
            // Здесь можно добавить другие эффекты (частицы, звуки и т.д.)
        }
        
        /// <summary>
        /// Завершает атаку
        /// </summary>
        protected virtual void EndAttack()
        {
            isAttacking = false;
            
            // Уведомляем другие системы
            OnAttackEnded?.Invoke();
            
            // Обновляем аниматор
            if (animator != null)
            {
                animator.SetBool("isAttacking", false);
            }
            
            if (enableDebugLogs)
            {
                Debug.Log("SimpleAttackSystem: Атака завершена");
            }
        }
        
        #region Публичные методы для других систем
        
        /// <summary>
        /// Проверяет, атакует ли объект
        /// </summary>
        /// <returns>true если объект атакует</returns>
        public bool IsAttacking()
        {
            return isAttacking;
        }
        
        /// <summary>
        /// Получает направление последней атаки
        /// </summary>
        /// <returns>Направление атаки</returns>
        public Vector2 GetLastAttackDirection()
        {
            return attackDirection;
        }
        
        /// <summary>
        /// Устанавливает новый урон атаки
        /// </summary>
        /// <param name="newDamage">Новый урон</param>
        public void SetAttackDamage(int newDamage)
        {
            attackDamage = newDamage;
            
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleAttackSystem: Урон атаки изменен на {newDamage}");
            }
        }
        
        /// <summary>
        /// Устанавливает новую дальность атаки
        /// </summary>
        /// <param name="newRange">Новая дальность</param>
        public void SetAttackRange(float newRange)
        {
            attackRange = newRange;
            
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleAttackSystem: Дальность атаки изменена на {newRange}");
            }
        }
        
        /// <summary>
        /// Получает текущую дальность атаки
        /// </summary>
        /// <returns>Дальность атаки</returns>
        public float GetAttackRange()
        {
            return attackRange;
        }
        
        /// <summary>
        /// Устанавливает новую перезарядку атаки
        /// </summary>
        /// <param name="newCooldown">Новая перезарядка</param>
        public void SetAttackCooldown(float newCooldown)
        {
            attackCooldown = newCooldown;
            
            if (enableDebugLogs)
            {
                Debug.Log($"SimpleAttackSystem: Перезарядка атаки изменена на {newCooldown}");
            }
        }
        
        #endregion
        
        #region Отладка
        
        private void OnDrawGizmosSelected()
        {
            if (!showAttackGizmos) return;
            
            // Показываем зону атаки
            Vector2 attackCenter = (Vector2)transform.position + attackDirection * (attackRange * 0.5f);
            
            Gizmos.color = isAttacking ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(attackCenter, attackRange * 0.5f);
            
            // Показываем направление атаки
            if (attackDirection.magnitude > 0.1f)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, attackDirection * attackRange);
            }
        }
        
        #endregion
    }
} 