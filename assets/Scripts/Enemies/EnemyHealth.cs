using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerabilityTime = 0.5f;

    private int currentHealth;
    private float lastDamageTime;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Наносит урон врагу
    /// </summary>
    /// <param name="amount">Количество урона</param>
    public void TakeDamage(int amount)
    {
        // Проверка на повторное получение урона
        if (Time.time - lastDamageTime < invulnerabilityTime)
        {
            return;
        }

        // Проверка на смерть
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        lastDamageTime = Time.time;

        // Проверка смерти
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Обработка смерти врага
    /// </summary>
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Enemy Dead: " + gameObject.name);
        
        // TODO: Добавить эффекты смерти (анимация, звук, частицы)
        
        // Уничтожаем объект
        Destroy(gameObject);
    }

    /// <summary>
    /// Возвращает текущее здоровье
    /// </summary>
    /// <returns>Текущее количество здоровья</returns>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Возвращает максимальное здоровье
    /// </summary>
    /// <returns>Максимальное количество здоровья</returns>
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Возвращает процент здоровья (0-1)
    /// </summary>
    /// <returns>Процент здоровья от 0 до 1</returns>
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    /// <summary>
    /// Проверяет, мёртв ли враг
    /// </summary>
    /// <returns>True если враг мёртв</returns>
    public bool IsDead()
    {
        return isDead;
    }
} 