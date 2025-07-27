using UnityEngine;
using System.Collections;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Контроллер анимаций игрока с современной архитектурой
    /// Управляет всеми анимациями игрока через события
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerCombat))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("Компоненты")]
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerCombat playerCombat;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Header("Настройки анимаций")]
        [SerializeField] private float animationBlendSpeed = 5f;
        [SerializeField] private bool enableAnimationEvents = true;
        
        [Header("Визуальные эффекты")]
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private Color dashColor = Color.cyan;
        [SerializeField] private float effectDuration = 0.1f;
        
        [Header("Отладка")]
        [SerializeField] private bool enableDebugLogs = true;
        
        // Состояние анимации
        private bool isMoving = false;
        private bool isDashing = false;
        private bool isAttacking = false;
        private Vector2 lastMovementDirection = Vector2.right;
        
        // События для других систем
        public System.Action OnAttackAnimationStarted;
        public System.Action OnAttackAnimationEnded;
        public System.Action OnDashAnimationStarted;
        public System.Action OnDashAnimationEnded;
        public System.Action OnDamageAnimationStarted;
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Start()
        {
            SubscribeToEvents();
            SetupAnimator();
        }
        
        private void Update()
        {
            UpdateMovementAnimation();
        }
        
        private void InitializeComponents()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
                
            if (playerMovement == null)
                playerMovement = GetComponent<PlayerMovement>();
                
            if (playerCombat == null)
                playerCombat = GetComponent<PlayerCombat>();
                
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
                
            if (enableDebugLogs)
            {
                Debug.Log("PlayerAnimationController: Компоненты инициализированы");
            }
        }
        
        private void SubscribeToEvents()
        {
            // Подписываемся на события движения
            if (playerMovement != null)
            {
                playerMovement.OnMovementChanged += OnMovementChanged;
                playerMovement.OnDashStateChanged += OnDashStateChanged;
                playerMovement.OnInvulnerabilityChanged += OnInvulnerabilityChanged;
            }
            
            // Подписываемся на события боя
            if (playerCombat != null)
            {
                playerCombat.OnAttackStarted += OnAttackStarted;
                playerCombat.OnAttackEnded += OnAttackEnded;
                playerCombat.OnEnemyHit += OnEnemyHit;
            }
        }
        
        private void SetupAnimator()
        {
            if (animator == null) return;
            
            // Устанавливаем начальные параметры
            animator.SetBool("isMoving", false);
            animator.SetBool("isDashing", false);
            animator.SetBool("isAttacking", false);
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", 0f);
            animator.SetFloat("lastMoveX", 1f);
            animator.SetFloat("lastMoveY", 0f);
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerAnimationController: Animator настроен");
            }
        }
        
        private void UpdateMovementAnimation()
        {
            if (playerMovement == null || animator == null) return;
            
            Vector2 movementDirection = playerMovement.GetMovementDirection();
            Vector2 lastDirection = playerMovement.GetLastMovementDirection();
            
            // Обновляем параметры движения
            animator.SetFloat("moveX", movementDirection.x);
            animator.SetFloat("moveY", movementDirection.y);
            animator.SetFloat("lastMoveX", lastDirection.x);
            animator.SetFloat("lastMoveY", lastDirection.y);
            
            // Обновляем состояние движения
            bool shouldBeMoving = movementDirection.magnitude > 0.1f && !isDashing && !isAttacking;
            
            if (isMoving != shouldBeMoving)
            {
                isMoving = shouldBeMoving;
                animator.SetBool("isMoving", isMoving);
                
                if (enableDebugLogs)
                {
                    Debug.Log($"PlayerAnimationController: Движение {(isMoving ? "началось" : "остановилось")}");
                }
            }
        }
        
        #region Event Handlers
        
        private void OnMovementChanged(Vector2 movement)
        {
            // Обновление анимации движения происходит в Update
            if (enableDebugLogs && movement.magnitude > 0.1f)
            {
                Debug.Log($"PlayerAnimationController: Направление движения {movement}");
            }
        }
        
        private void OnDashStateChanged(bool isDashing)
        {
            this.isDashing = isDashing;
            
            if (animator != null)
            {
                animator.SetBool("isDashing", isDashing);
            }
            
            if (isDashing)
            {
                OnDashAnimationStarted?.Invoke();
                StartCoroutine(DashVisualEffect());
                
                if (enableDebugLogs)
                {
                    Debug.Log("PlayerAnimationController: Dash анимация началась");
                }
            }
            else
            {
                OnDashAnimationEnded?.Invoke();
                
                if (enableDebugLogs)
                {
                    Debug.Log("PlayerAnimationController: Dash анимация завершилась");
                }
            }
        }
        
        private void OnInvulnerabilityChanged(bool isInvulnerable)
        {
            if (isInvulnerable && spriteRenderer != null)
            {
                StartCoroutine(InvulnerabilityVisualEffect());
            }
        }
        
        private void OnAttackStarted()
        {
            isAttacking = true;
            
            if (animator != null)
            {
                animator.SetBool("isAttacking", true);
                animator.SetTrigger("Attack");
            }
            
            OnAttackAnimationStarted?.Invoke();
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerAnimationController: Анимация атаки началась");
            }
        }
        
        private void OnAttackEnded()
        {
            isAttacking = false;
            
            if (animator != null)
            {
                animator.SetBool("isAttacking", false);
            }
            
            OnAttackAnimationEnded?.Invoke();
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerAnimationController: Анимация атаки завершилась");
            }
        }
        
        private void OnEnemyHit(IDamageable enemy)
        {
            // Эффект попадания
            StartCoroutine(HitEffect());
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerAnimationController: Эффект попадания");
            }
        }
        
        #endregion
        
        #region Visual Effects
        
        private IEnumerator DashVisualEffect()
        {
            if (spriteRenderer == null) yield break;
            
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = dashColor;
            
            yield return new WaitForSeconds(effectDuration);
            
            spriteRenderer.color = originalColor;
        }
        
        private IEnumerator InvulnerabilityVisualEffect()
        {
            if (spriteRenderer == null) yield break;
            
            Color originalColor = spriteRenderer.color;
            float elapsedTime = 0f;
            float invulnerabilityDuration = 0.5f; // Время неуязвимости
            
            while (elapsedTime < invulnerabilityDuration)
            {
                // Мигание
                spriteRenderer.color = Color.Lerp(originalColor, Color.white, 0.5f);
                yield return new WaitForSeconds(0.05f);
                
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.05f);
                
                elapsedTime += 0.1f;
            }
            
            spriteRenderer.color = originalColor;
        }
        
        private IEnumerator HitEffect()
        {
            if (spriteRenderer == null) yield break;
            
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = damageColor;
            
            yield return new WaitForSeconds(effectDuration);
            
            spriteRenderer.color = originalColor;
        }
        
        #endregion
        
        #region Animation Events (вызываются из анимаций)
        
        /// <summary>
        /// Вызывается в анимации атаки для определения момента попадания
        /// </summary>
        public void OnAttackHitFrame()
        {
            if (enableAnimationEvents)
            {
                // Здесь можно добавить звуки или дополнительные эффекты
                if (enableDebugLogs)
                {
                    Debug.Log("PlayerAnimationController: Кадр попадания атаки");
                }
            }
        }
        
        /// <summary>
        /// Вызывается в анимации атаки для определения конца атаки
        /// </summary>
        public void OnAttackEndFrame()
        {
            if (enableAnimationEvents)
            {
                // Уведомляем систему боя о завершении атаки
                if (playerCombat != null)
                {
                    // playerCombat.EndAttack(); // Если нужно
                }
                
                if (enableDebugLogs)
                {
                    Debug.Log("PlayerAnimationController: Кадр завершения атаки");
                }
            }
        }
        
        #endregion
        
        #region Публичные методы для других систем
        
        /// <summary>
        /// Принудительно обновляет анимацию
        /// </summary>
        public void ForceAnimationUpdate()
        {
            if (animator != null)
            {
                animator.Update(0f);
            }
        }
        
        /// <summary>
        /// Устанавливает скорость анимации
        /// </summary>
        /// <param name="speed">Скорость анимации</param>
        public void SetAnimationSpeed(float speed)
        {
            if (animator != null)
            {
                animator.speed = speed;
            }
        }
        
        /// <summary>
        /// Получает текущее состояние анимации
        /// </summary>
        /// <returns>Строка с текущим состоянием</returns>
        public string GetCurrentAnimationState()
        {
            if (animator == null) return "No Animator";
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Idle") ? "Idle" :
                   stateInfo.IsName("Walk") ? "Walk" :
                   stateInfo.IsName("Dash") ? "Dash" :
                   stateInfo.IsName("Attack") ? "Attack" : "Unknown";
        }
        
        /// <summary>
        /// Проверяет, играет ли анимация
        /// </summary>
        /// <param name="animationName">Имя анимации</param>
        /// <returns>true если анимация играет</returns>
        public bool IsPlayingAnimation(string animationName)
        {
            if (animator == null) return false;
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(animationName);
        }
        
        #endregion
        
        private void OnDestroy()
        {
            // Отписываемся от событий
            if (playerMovement != null)
            {
                playerMovement.OnMovementChanged -= OnMovementChanged;
                playerMovement.OnDashStateChanged -= OnDashStateChanged;
                playerMovement.OnInvulnerabilityChanged -= OnInvulnerabilityChanged;
            }
            
            if (playerCombat != null)
            {
                playerCombat.OnAttackStarted -= OnAttackStarted;
                playerCombat.OnAttackEnded -= OnAttackEnded;
                playerCombat.OnEnemyHit -= OnEnemyHit;
            }
        }
    }
} 