using UnityEngine;
using UnityEngine.InputSystem;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Контроллер ввода игрока, использующий новую Input System
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputController : MonoBehaviour
    {
        [Header("Компоненты")]
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerCombat playerCombat;
        
        [Header("Настройки")]
        [SerializeField] private bool enableDebugLogs = true;
        
        // События для других систем
        public System.Action<Vector2> OnMoveInput;
        public System.Action OnAttackInput;
        public System.Action OnDashInput;
        
        private PlayerInput playerInput;
        private InputAction moveAction;
        private InputAction attackAction;
        private InputAction dashAction;
        
        private void Awake()
        {
            InitializeComponents();
            SetupInputActions();
        }
        
        private void InitializeComponents()
        {
            playerInput = GetComponent<PlayerInput>();
            
            if (playerMovement == null)
                playerMovement = GetComponent<PlayerMovement>();
                
            if (playerCombat == null)
                playerCombat = GetComponent<PlayerCombat>();
                
            if (enableDebugLogs)
            {
                Debug.Log("PlayerInputController: Компоненты инициализированы");
            }
        }
        
        private void SetupInputActions()
        {
            if (playerInput == null) return;
            
            // Получаем действия из Input Actions
            moveAction = playerInput.actions["Move"];
            attackAction = playerInput.actions["Attack"];
            dashAction = playerInput.actions["Dash"];
            
            // Подписываемся на события
            attackAction.performed += OnAttackPerformed;
            dashAction.performed += OnDashPerformed;
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerInputController: Input Actions настроены");
            }
        }
        
        private void Update()
        {
            HandleMoveInput();
        }
        
        private void HandleMoveInput()
        {
            if (moveAction == null) return;
            
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            
            // Нормализуем диагональное движение
            if (moveInput.magnitude > 0.1f)
            {
                moveInput = moveInput.normalized;
            }
            
            // Вызываем движение
            if (playerMovement != null)
            {
                playerMovement.SetMovementInput(moveInput);
            }
            
            // Уведомляем другие системы
            OnMoveInput?.Invoke(moveInput);
        }
        
        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if (enableDebugLogs)
            {
                Debug.Log("PlayerInputController: Атака!");
            }
            
            // Вызываем атаку
            if (playerCombat != null)
            {
                playerCombat.PerformAttack();
            }
            
            // Уведомляем другие системы
            OnAttackInput?.Invoke();
        }
        
        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            if (enableDebugLogs)
            {
                Debug.Log("PlayerInputController: Dash!");
            }
            
            // Вызываем dash
            if (playerMovement != null)
            {
                playerMovement.PerformDash();
            }
            
            // Уведомляем другие системы
            OnDashInput?.Invoke();
        }
        
        private void OnDestroy()
        {
            // Отписываемся от событий
            if (attackAction != null)
                attackAction.performed -= OnAttackPerformed;
                
            if (dashAction != null)
                dashAction.performed -= OnDashPerformed;
        }
        
        /// <summary>
        /// Получает текущий ввод движения
        /// </summary>
        /// <returns>Вектор движения</returns>
        public Vector2 GetMovementInput()
        {
            return moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }
        
        /// <summary>
        /// Проверяет, нажата ли кнопка атаки
        /// </summary>
        /// <returns>true если атака нажата</returns>
        public bool IsAttackPressed()
        {
            return attackAction?.IsPressed() ?? false;
        }
        
        /// <summary>
        /// Проверяет, нажата ли кнопка dash
        /// </summary>
        /// <returns>true если dash нажат</returns>
        public bool IsDashPressed()
        {
            return dashAction?.IsPressed() ?? false;
        }
    }
} 