using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    
    private EnemyHealth enemyHealth;
    private float lastHealthPercentage;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        
        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealth component not found on " + gameObject.name);
            return;
        }

        lastHealthPercentage = enemyHealth.GetHealthPercentage();
        Debug.Log($"Enemy {gameObject.name} health bar initialized. Health: {enemyHealth.GetCurrentHealth()}/{enemyHealth.GetMaxHealth()}");
    }

    private void Update()
    {
        if (enemyHealth == null) return;

        float currentHealthPercentage = enemyHealth.GetHealthPercentage();
        
        // Логируем изменения здоровья
        if (currentHealthPercentage != lastHealthPercentage)
        {
            Debug.Log($"Enemy {gameObject.name} health: {enemyHealth.GetCurrentHealth()}/{enemyHealth.GetMaxHealth()} ({currentHealthPercentage:P0})");
            lastHealthPercentage = currentHealthPercentage;
        }

        // Скрываем если враг мёртв
        if (enemyHealth.IsDead())
        {
            Debug.Log($"Enemy {gameObject.name} is dead!");
            enabled = false;
        }
    }

    /// <summary>
    /// Обновляет полоску здоровья (заглушка для будущей UI реализации)
    /// </summary>
    public void UpdateHealthBar()
    {
        if (enemyHealth != null)
        {
            float healthPercent = enemyHealth.GetHealthPercentage();
            Debug.Log($"Health bar updated: {healthPercent:P0}");
        }
    }

    /// <summary>
    /// Показывает полоску здоровья (заглушка)
    /// </summary>
    public void ShowHealthBar()
    {
        Debug.Log($"Health bar shown for {gameObject.name}");
    }

    /// <summary>
    /// Скрывает полоску здоровья (заглушка)
    /// </summary>
    public void HideHealthBar()
    {
        Debug.Log($"Health bar hidden for {gameObject.name}");
    }
} 