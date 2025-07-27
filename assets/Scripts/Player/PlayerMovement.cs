using UnityEngine;
using System.Collections;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Система движения игрока с современной архитектурой
    /// Получает ввод от PlayerInputController и использует Rigidbody2D.MovePosition
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Настройки движения")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float dashSpeed = 15f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 1f;
        
        [Header("Настройки Dash")]
        [SerializeField] private bool dashMakesInvulnerable = true;
        [SerializeField] private LayerMask dashCollisionMask = -1;
        
        [Header("Отладка")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool showMovementGizmos = true;
        
        // Компоненты
        private Rigidbody2D rb2D;
        private Animator animator;
        private PlayerInputController inputController;
        
        // Состояние движения
        private Vector2 movementInput;
        private Vector2 lastMovementDirection = Vector2.right;
        private bool isDashing = false;
        private bool canDash = true;
        private Vector2 dashDirection;
        private bool isInvulnerable = false;
        
        // События
        public System.Action<bool> OnDashStateChanged;
        public System.Action<bool> OnInvulnerabilityChanged;
        public System.Action<Vector2> OnMovementChanged;
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Start()
        {
            ConfigureRigidbody();
        }
        
        private void FixedUpdate()
        {
            if (!isDashing)
            {
                HandleMovement();
            }
        }
        
        private void InitializeComponents()
        {
            rb2D = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            inputController = GetComponent<PlayerInputController>();
            
            if (rb2D == null)
            {
                Debug.LogError("PlayerMovement: Отсутствует Rigidbody2D!");
            }
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerMovement: Компоненты инициализированы");
            }
        }
        
        private void ConfigureRigidbody()
        {
            if (rb2D == null) return;
            
            // Настройки для top-down движения
            rb2D.gravityScale = 0f;
            rb2D.freezeRotation = true;
            rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerMovement: Rigidbody2D настроен");
            }
        }
        
        /// <summary>
        /// Устанавливает ввод движения от PlayerInputController
        /// </summary>
        /// <param name="input">Вектор движения</param>
        public void SetMovementInput(Vector2 input)
        {
            movementInput = input;
            
            // Сохраняем последнее направление для анимаций
            if (input.magnitude > 0.1f)
            {
                lastMovementDirection = input.normalized;
            }
            
            // Обновляем аниматор
            UpdateAnimator();
            
            // Уведомляем другие системы
            OnMovementChanged?.Invoke(input);
        }
        
        private void HandleMovement()
        {
            if (rb2D == null || movementInput.magnitude < 0.1f) return;
            
            // Вычисляем скорость движения
            Vector2 targetVelocity = movementInput * moveSpeed;
            
            // Используем MovePosition для плавного движения
            Vector2 targetPosition = rb2D.position + targetVelocity * Time.fixedDeltaTime;
            rb2D.MovePosition(targetPosition);
            
            if (enableDebugLogs && movementInput.magnitude > 0.1f)
            {
                Debug.Log($"PlayerMovement: Движение {movementInput} со скоростью {targetVelocity}");
            }
        }
        
        /// <summary>
        /// Выполняет dash в текущем направлении движения
        /// </summary>
        public void PerformDash()
        {
            if (isDashing || !canDash) return;
            
            // Используем последнее направление движения или текущий ввод
            Vector2 dashDir = movementInput.magnitude > 0.1f ? movementInput.normalized : lastMovementDirection;
            
            if (dashDir.magnitude > 0.1f)
            {
                StartDash(dashDir);
            }
        }
        
        /// <summary>
        /// Выполняет dash в указанном направлении
        /// </summary>
        /// <param name="direction">Направление dash</param>
        public void PerformDash(Vector2 direction)
        {
            if (direction.magnitude > 0.1f)
            {
                StartDash(direction.normalized);
            }
        }
        
        private void StartDash(Vector2 direction)
        {
            if (isDashing || !canDash) return;
            
            dashDirection = direction;
            isDashing = true;
            canDash = false;
            
            // Устанавливаем неуязвимость
            if (dashMakesInvulnerable)
            {
                SetInvulnerability(true);
            }
            
            // Уведомляем другие системы
            OnDashStateChanged?.Invoke(true);
            
            // Обновляем аниматор
            if (animator != null)
            {
                animator.SetBool("isDashing", true);
            }
            
            // Запускаем корутину dash
            StartCoroutine(DashRoutine());
            
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerMovement: Начат dash в направлении {dashDirection}");
            }
        }
        
        private IEnumerator DashRoutine()
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < dashDuration)
            {
                // Применяем скорость dash
                if (rb2D != null)
                {
                    Vector2 dashVelocity = dashDirection * dashSpeed;
                    Vector2 targetPosition = rb2D.position + dashVelocity * Time.fixedDeltaTime;
                    rb2D.MovePosition(targetPosition);
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Завершаем dash
            EndDash();
            
            // Запускаем корутину перезарядки
            StartCoroutine(DashCooldownRoutine());
        }
        
        private void EndDash()
        {
            isDashing = false;
            
            // Останавливаем игрока
            if (rb2D != null)
            {
                rb2D.velocity = Vector2.zero;
            }
            
            // Убираем неуязвимость
            if (dashMakesInvulnerable)
            {
                SetInvulnerability(false);
            }
            
            // Уведомляем другие системы
            OnDashStateChanged?.Invoke(false);
            
            // Обновляем аниматор
            if (animator != null)
            {
                animator.SetBool("isDashing", false);
            }
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerMovement: Dash завершён");
            }
        }
        
        private IEnumerator DashCooldownRoutine()
        {
            yield return new WaitForSeconds(dashCooldown);
            
            canDash = true;
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerMovement: Dash готов к использованию");
            }
        }
        
        private void SetInvulnerability(bool invulnerable)
        {
            if (isInvulnerable == invulnerable) return;
            
            isInvulnerable = invulnerable;
            OnInvulnerabilityChanged?.Invoke(isInvulnerable);
            
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerMovement: Неуязвимость {(isInvulnerable ? "включена" : "выключена")}");
            }
        }
        
        private void UpdateAnimator()
        {
            if (animator == null) return;
            
            // Устанавливаем параметры анимации
            animator.SetBool("isMoving", movementInput.magnitude > 0.1f && !isDashing);
            animator.SetFloat("moveX", movementInput.x);
            animator.SetFloat("moveY", movementInput.y);
            animator.SetFloat("lastMoveX", lastMovementDirection.x);
            animator.SetFloat("lastMoveY", lastMovementDirection.y);
        }
        
        #region Публичные методы для других систем
        
        /// <summary>
        /// Получает текущее направление движения
        /// </summary>
        /// <returns>Нормализованный вектор движения</returns>
        public Vector2 GetMovementDirection()
        {
            return movementInput.magnitude > 0.1f ? movementInput.normalized : Vector2.zero;
        }
        
        /// <summary>
        /// Получает последнее направление движения
        /// </summary>
        /// <returns>Последнее направление движения</returns>
        public Vector2 GetLastMovementDirection()
        {
            return lastMovementDirection;
        }
        
        /// <summary>
        /// Проверяет, движется ли игрок
        /// </summary>
        /// <returns>true если игрок движется</returns>
        public bool IsMoving()
        {
            return movementInput.magnitude > 0.1f && !isDashing;
        }
        
        /// <summary>
        /// Проверяет, выполняет ли игрок dash
        /// </summary>
        /// <returns>true если игрок в dash</returns>
        public bool IsDashing()
        {
            return isDashing;
        }
        
        /// <summary>
        /// Проверяет, может ли игрок использовать dash
        /// </summary>
        /// <returns>true если dash доступен</returns>
        public bool CanDash()
        {
            return canDash && !isDashing;
        }
        
        /// <summary>
        /// Проверяет, неуязвим ли игрок
        /// </summary>
        /// <returns>true если игрок неуязвим</returns>
        public bool IsInvulnerable()
        {
            return isInvulnerable;
        }
        
        /// <summary>
        /// Получает текущую скорость игрока
        /// </summary>
        /// <returns>Текущая скорость</returns>
        public float GetCurrentSpeed()
        {
            return rb2D != null ? rb2D.velocity.magnitude : 0f;
        }
        
        /// <summary>
        /// Устанавливает новую скорость движения
        /// </summary>
        /// <param name="newSpeed">Новая скорость</param>
        public void SetMoveSpeed(float newSpeed)
        {
            moveSpeed = newSpeed;
            
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerMovement: Скорость движения изменена на {newSpeed}");
            }
        }
        
        /// <summary>
        /// Останавливает игрока
        /// </summary>
        public void StopMovement()
        {
            movementInput = Vector2.zero;
            
            if (rb2D != null)
            {
                rb2D.velocity = Vector2.zero;
            }
            
            // Прерываем dash если он активен
            if (isDashing)
            {
                StopAllCoroutines();
                EndDash();
                canDash = true;
            }
            
            UpdateAnimator();
        }
        
        #endregion
        
        #region Отладка
        
        private void OnDrawGizmosSelected()
        {
            if (!showMovementGizmos) return;
            
            // Показываем направление движения
            if (lastMovementDirection.magnitude > 0.1f)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, lastMovementDirection * 2f);
            }
            
            // Показываем зону dash
            if (isDashing)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 1f);
            }
        }
        
        #endregion
    }
} 