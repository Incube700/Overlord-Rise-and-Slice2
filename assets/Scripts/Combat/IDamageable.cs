using UnityEngine;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Интерфейс для объектов, которые могут получать урон
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Наносит урон объекту
        /// </summary>
        /// <param name="amount">Количество урона</param>
        void TakeDamage(int amount);
        
        /// <summary>
        /// Проверяет, может ли объект получать урон
        /// </summary>
        /// <returns>true если объект может получать урон</returns>
        bool CanTakeDamage();
        
        /// <summary>
        /// Получает текущее здоровье объекта
        /// </summary>
        /// <returns>Текущее здоровье</returns>
        int GetCurrentHealth();
        
        /// <summary>
        /// Получает максимальное здоровье объекта
        /// </summary>
        /// <returns>Максимальное здоровье</returns>
        int GetMaxHealth();
    }
} 