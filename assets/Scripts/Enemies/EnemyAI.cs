using UnityEngine;

namespace OverlordRiseAndSlice
{
    public class EnemyAI : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 1f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool showGizmos = true;

        private Transform playerTransform;
        private Rigidbody2D rb2D;
        private SimpleHealthSystem enemyHealth;
        private SimpleAttackSystem enemyAttack;
        private Animator animator;
        private float lastAttackTime;
        
        // Состояние для анимации
        private bool isMoving = false;
        private bool isAttacking = false;

        private void Start()
        {
            rb2D = GetComponent<Rigidbody2D>();
            enemyHealth = GetComponent<SimpleHealthSystem>();
            enemyAttack = GetComponent<SimpleAttackSystem>();
            animator = GetComponent<Animator>();
            
            // Проверяем наличие компонентов
            if (rb2D == null && enableDebugLogs)
            {
                Debug.LogWarning("EnemyAI: Rigidbody2D not found on " + gameObject.name);
            }
            
            if (enemyHealth == null && enableDebugLogs)
            {
                Debug.LogWarning("EnemyAI: SimpleHealthSystem not found on " + gameObject.name);
            }
            
            if (enemyAttack == null && enableDebugLogs)
            {
                Debug.LogWarning("EnemyAI: SimpleAttackSystem not found on " + gameObject.name);
            }
            
            // Ищем игрока по тегу
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning("EnemyAI: Player not found! Make sure player has 'Player' tag.");
            }
            
            if (animator == null && enableDebugLogs)
            {
                Debug.LogWarning("EnemyAI: Animator not found. Animations will not work.");
            }
        }

        private void Update()
        {
            if (playerTransform == null || (enemyHealth != null && enemyHealth.IsDead()))
            {
                SetMoving(false);
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // Проверяем, в зоне ли обнаружения
            if (distanceToPlayer <= detectionRange)
            {
                // Если в зоне атаки
                if (distanceToPlayer <= attackRange)
                {
                    SetMoving(false);
                    
                    // Атакуем игрока
                    if (enemyAttack != null && enemyAttack.CanAttack())
                    {
                        enemyAttack.PerformAttack();
                    }
                }
                else
                {
                    // Двигаемся к игроку
                    SetMoving(true);
                    MoveTowardsPlayer();
                }
            }
            else
            {
                // Игрок вне зоны обнаружения
                SetMoving(false);
            }
        }

        /// <summary>
        /// Движение к игроку
        /// </summary>
        private void MoveTowardsPlayer()
        {
            if (rb2D == null || playerTransform == null) return;
            
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            Vector2 newPosition = rb2D.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb2D.MovePosition(newPosition);
        }

        /// <summary>
        /// Возвращает расстояние до игрока
        /// </summary>
        /// <returns>Расстояние до игрока</returns>
        public float GetDistanceToPlayer()
        {
            if (playerTransform == null) return float.MaxValue;
            return Vector2.Distance(transform.position, playerTransform.position);
        }

        /// <summary>
        /// Проверяет, видит ли враг игрока
        /// </summary>
        /// <returns>True если игрок в зоне обнаружения</returns>
        public bool CanSeePlayer()
        {
            if (playerTransform == null) return false;
            return GetDistanceToPlayer() <= detectionRange;
        }

        /// <summary>
        /// Проверяет, может ли враг атаковать
        /// </summary>
        /// <returns>True если игрок в зоне атаки</returns>
        public bool CanAttackPlayer()
        {
            if (playerTransform == null) return false;
            if (enemyAttack == null) return false;
            return GetDistanceToPlayer() <= attackRange && enemyAttack.CanAttack();
        }
        
        /// <summary>
        /// Устанавливает состояние движения для анимации
        /// </summary>
        private void SetMoving(bool moving)
        {
            if (isMoving == moving) return;
            
            isMoving = moving;
            
            if (animator != null)
            {
                animator.SetBool("isMoving", isMoving);
            }
            
            if (enableDebugLogs)
            {
                Debug.Log($"EnemyAI: Enemy {(isMoving ? "started" : "stopped")} moving");
            }
        }
        
        /// <summary>
        /// Устанавливает состояние атаки для анимации
        /// </summary>
        private void SetAttacking(bool attacking)
        {
            if (isAttacking == attacking) return;
            
            isAttacking = attacking;
            
            if (animator != null)
            {
                animator.SetBool("isAttacking", isAttacking);
                
                if (attacking)
                {
                    animator.SetTrigger("Attack");
                }
            }
            
            if (enableDebugLogs)
            {
                Debug.Log($"EnemyAI: Enemy {(isAttacking ? "started" : "stopped")} attacking");
            }
        }

        // Гизмо для отладки в редакторе
        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;
            
            // Зона обнаружения
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            // Зона атаки
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
} 