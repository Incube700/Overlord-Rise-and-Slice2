using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Система боя игрока с современной архитектурой
    /// Использует PlayerAttackSystem для атак
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Настройки отладки")]
        [SerializeField] private bool enableDebugLogs = true;
        
        // Компоненты
        private PlayerMovement playerMovement;
        private SimpleAttackSystem attackSystem;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        
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
            attackSystem = GetComponent<SimpleAttackSystem>();
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
            if (attackSystem != null)
            {
                attackSystem.PerformAttack();
            }
        }
        
        /// <summary>
        /// Выполняет атаку в указанном направлении
        /// </summary>
        /// <param name="direction">Направление атаки</param>
        public void PerformAttack(Vector2 direction)
        {
            if (attackSystem != null)
            {
                attackSystem.PerformAttack(direction);
            }
        }
        
        /// <summary>
        /// Проверяет, может ли игрок атаковать
        /// </summary>
        /// <returns>true если атака доступна</returns>
        public bool CanAttack()
        {
            return attackSystem != null && attackSystem.CanAttack();
        }
        
        #region Публичные методы для других систем
        
        /// <summary>
        /// Проверяет, атакует ли игрок
        /// </summary>
        /// <returns>true если игрок атакует</returns>
        public bool IsAttacking()
        {
            return attackSystem != null && attackSystem.IsAttacking();
        }
        
        /// <summary>
        /// Получает направление последней атаки
        /// </summary>
        /// <returns>Направление атаки</returns>
        public Vector2 GetAttackDirection()
        {
            return attackSystem != null ? attackSystem.GetLastAttackDirection() : Vector2.right;
        }
        
        /// <summary>
        /// Устанавливает новый урон атаки
        /// </summary>
        /// <param name="newDamage">Новый урон</param>
        public void SetAttackDamage(int newDamage)
        {
            if (attackSystem != null)
            {
                attackSystem.SetAttackDamage(newDamage);
            }
        }
        
        /// <summary>
        /// Устанавливает новую дальность атаки
        /// </summary>
        /// <param name="newRange">Новая дальность</param>
        public void SetAttackRange(float newRange)
        {
            if (attackSystem != null)
            {
                attackSystem.SetAttackRange(newRange);
            }
        }
        
        /// <summary>
        /// Устанавливает новую перезарядку атаки
        /// </summary>
        /// <param name="newCooldown">Новая перезарядка</param>
        public void SetAttackCooldown(float newCooldown)
        {
            if (attackSystem != null)
            {
                attackSystem.SetAttackCooldown(newCooldown);
            }
        }
        
        #endregion
        

    }
} 