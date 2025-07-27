using UnityEngine;
using System.Collections;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Простая система здоровья для игрока с возможностью возрождения
    /// </summary>
    public class PlayerHealthSystem : SimpleHealthSystem
    {
        [Header("Настройки игрока")]
        [SerializeField] private Vector3 respawnPosition = Vector3.zero;
        [SerializeField] private float respawnDelay = 2f;
        [SerializeField] private bool respawnAtStartPosition = true;
        
        [Header("Настройки отладки")]
        [SerializeField] private bool enableDebugLogs = true;
        
        // Компоненты игрока
        private PlayerInputController playerInput;
        private PlayerMovement playerMovement;
        private SimpleAttackSystem playerAttack;
        
        // Состояние
        private Vector3 startPosition;
        private bool isRespawning = false;
        
        protected override void Awake()
        {
            base.Awake();
            InitializePlayerComponents();
        }
        
        protected override void Start()
        {
            base.Start();
            
            // Запоминаем начальную позицию
            if (respawnAtStartPosition)
            {
                startPosition = transform.position;
            }
            else
            {
                startPosition = respawnPosition;
            }
            
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerHealthSystem: Инициализирован для игрока {gameObject.name}");
            }
        }
        
        private void InitializePlayerComponents()
        {
            playerInput = GetComponent<PlayerInputController>();
            playerMovement = GetComponent<PlayerMovement>();
            playerAttack = GetComponent<SimpleAttackSystem>();
            
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerHealthSystem: Компоненты игрока инициализированы для {gameObject.name}");
            }
        }
        
        /// <summary>
        /// Переопределяем смерть игрока для добавления логики возрождения
        /// </summary>
        protected override void Die()
        {
            if (GetCurrentHealth() > 0) return;
            
            // Отключаем ввод игрока
            if (playerInput != null)
            {
                playerInput.enabled = false;
            }
            
            // Останавливаем движение
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
            
            // Отключаем атаку
            if (playerAttack != null)
            {
                playerAttack.enabled = false;
            }
            
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerHealthSystem: Игрок {gameObject.name} умер, начинаем возрождение");
            }
            
            // Запускаем возрождение
            StartCoroutine(RespawnRoutine());
        }
        
        /// <summary>
        /// Корутина возрождения игрока
        /// </summary>
        private IEnumerator RespawnRoutine()
        {
            isRespawning = true;
            
            // Ждем задержку возрождения
            yield return new WaitForSeconds(respawnDelay);
            
            // Возрождаем игрока
            RespawnPlayer();
            
            isRespawning = false;
        }
        
        /// <summary>
        /// Возрождает игрока
        /// </summary>
        private void RespawnPlayer()
        {
            // Восстанавливаем здоровье
            RestoreFullHealth();
            
            // Перемещаем в начальную позицию
            transform.position = startPosition;
            
            // Включаем ввод игрока
            if (playerInput != null)
            {
                playerInput.enabled = true;
            }
            
            // Включаем движение
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
            
            // Включаем атаку
            if (playerAttack != null)
            {
                playerAttack.enabled = true;
            }
            
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerHealthSystem: Игрок {gameObject.name} возрожден в позиции {startPosition}");
            }
        }
        
        /// <summary>
        /// Устанавливает позицию возрождения
        /// </summary>
        /// <param name="position">Новая позиция возрождения</param>
        public void SetRespawnPosition(Vector3 position)
        {
            startPosition = position;
            
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerHealthSystem: Позиция возрождения изменена на {position}");
            }
        }
        
        /// <summary>
        /// Проверяет, возрождается ли игрок
        /// </summary>
        /// <returns>true если игрок возрождается</returns>
        public bool IsRespawning()
        {
            return isRespawning;
        }
        
        /// <summary>
        /// Получает позицию возрождения
        /// </summary>
        /// <returns>Позиция возрождения</returns>
        public Vector3 GetRespawnPosition()
        {
            return startPosition;
        }
    }
} 