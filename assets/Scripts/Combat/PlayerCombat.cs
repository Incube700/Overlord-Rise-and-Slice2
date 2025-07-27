using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Система боя игрока с современной архитектурой
    /// Использует IDamageable интерфейс и Physics2D.OverlapCircle
    /// </summary>
    public class PlayerCombat : MonoBehaviour
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
        private PlayerMovement playerMovement;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        
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
            playerMovement = GetComponent<PlayerMovement>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerCombat: Компоненты инициализированы");
            }
        }
        
        /// <summary>
        /// Выполняет атаку в направлении движения игрока
        /// </summary>
        public void PerformAttack()
        {
            if (!CanAttack()) return;
            
            // Получаем направление атаки
            Vector2 attackDir = GetAttackDirection();
            
            if (attackDir.magnitude > 0.1f)
            {
                StartAttack(attackDir);
            }
            else if (enableDebugLogs)
            {
                Debug.Log("PlayerCombat: Нет направления для атаки");
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
        
        private bool CanAttack()
        {
            if (isAttacking) return false;
            if (Time.time < lastAttackTime + attackCooldown) return false;
            if (playerMovement != null && playerMovement.IsDashing()) return false;
            
            return true;
        }
        
        private Vector2 GetAttackDirection()
        {
            // Приоритет: текущее движение -> последнее направление -> вправо
            if (playerMovement != null)
            {
                Vector2 moveDir = playerMovement.GetMovementDirection();
                if (moveDir.magnitude > 0.1f) return moveDir;
                
                Vector2 lastDir = playerMovement.GetLastMovementDirection();
                if (lastDir.magnitude > 0.1f) return lastDir;
            }
            
            return Vector2.right; // По умолчанию атакуем вправо
        }
        
        private void StartAttack(Vector2 direction)
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
                Debug.Log($"PlayerCombat: Начата атака в направлении {attackDirection}");
            }
        }
        
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
        
        private void CheckForHits()
        {
            // Определяем центр атаки
            Vector2 attackCenter = (Vector2)transform.position + attackDirection * (attackRange * 0.5f);
            
            // Ищем объекты в зоне атаки
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackCenter, attackRange * 0.5f, attackLayerMask);
            
            foreach (Collider2D hitCollider in hitColliders)
            {
                // Пропускаем самого игрока
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
                        Debug.Log($"PlayerCombat: Попадание! Нанесено {attackDamage} урона {hitCollider.name}");
                    }
                }
            }
        }
        
        private IEnumerator AttackHitEffects(Vector3 hitPosition)
        {
            // Эффект вспышки на игроке
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
        
        private void EndAttack()
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
                Debug.Log("PlayerCombat: Атака завершена");
            }
        }
        
        #region Публичные методы для других систем
        
        /// <summary>
        /// Проверяет, атакует ли игрок
        /// </summary>
        /// <returns>true если игрок атакует</returns>
        public bool IsAttacking()
        {
            return isAttacking;
        }
        
        /// <summary>
        /// Проверяет, может ли игрок атаковать
        /// </summary>
        /// <returns>true если атака доступна</returns>
        public bool CanAttack()
        {
            return !isAttacking && Time.time >= lastAttackTime + attackCooldown;
        }
        
        /// <summary>
        /// Получает направление последней атаки
        /// </summary>
        /// <returns>Направление атаки</returns>
        public Vector2 GetAttackDirection()
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
                Debug.Log($"PlayerCombat: Урон атаки изменен на {newDamage}");
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
                Debug.Log($"PlayerCombat: Дальность атаки изменена на {newRange}");
            }
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
                Debug.Log($"PlayerCombat: Перезарядка атаки изменена на {newCooldown}");
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