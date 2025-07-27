using UnityEngine;
using System.Collections;

/// <summary>
/// Система ближней атаки игрока с анимацией и визуальными эффектами.
/// Обрабатывает ввод, наносит урон врагам в зоне атаки, и управляет анимацией удара.
/// </summary>
public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("Настройки атаки")]
    [SerializeField] private float attackDamage = 25f; // Урон атаки
    [SerializeField] private float attackRange = 1.2f; // Радиус атаки
    [SerializeField] private float attackCooldown = 0.4f; // Перезарядка между атаками
    [SerializeField] private float attackDuration = 0.3f; // Длительность анимации атаки
    [SerializeField] private Vector2 attackOffset = new Vector2(1f, 0f); // Смещение зоны атаки от игрока
    [SerializeField] private LayerMask enemyLayer; // Слой врагов

    [Header("Визуальные эффекты")]
    [SerializeField] private Color attackColor = Color.red; // Цвет вспышки при атаке
    [SerializeField] private float flashDuration = 0.1f; // Длительность вспышки

    [Header("Отладка")]
    [SerializeField] private bool enableDebugLogs = true; // Включить логгирование
    [SerializeField] private bool showAttackGizmos = true; // Показывать гизмо зоны атаки

    // Состояние атаки
    private bool isAttacking = false;
    private float lastAttackTime = -999f;

    // Компоненты
    private Transform playerTransform;
    private SpriteRenderer playerSprite;
    private Animator animator;
    private PlayerMovementSimple playerMovement;

    // Визуальные эффекты
    private Color originalColor;

    // События для других систем
    public System.Action OnAttackStarted; // Уведомление о начале атаки
    public System.Action OnAttackEnded; // Уведомление об окончании атаки
    public System.Action<GameObject> OnEnemyHit; // Уведомление о попадании по врагу

    private void Awake()
    {
        InitializeComponents();
    }

    private void Start()
    {
        ValidateComponents();
    }

    void Update()
    {
        HandleAttackInput();
    }

    /// <summary>
    /// Инициализирует компоненты
    /// </summary>
    private void InitializeComponents()
    {
        playerTransform = transform;
        playerSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovementSimple>();

        if (playerSprite != null)
        {
            originalColor = playerSprite.color;
        }
    }

    /// <summary>
    /// Проверяет наличие необходимых компонентов
    /// </summary>
    private void ValidateComponents()
    {
        if (playerSprite == null && enableDebugLogs)
        {
            Debug.LogWarning("PlayerMeleeAttack: SpriteRenderer не найден. Визуальные эффекты не будут работать.");
        }

        if (animator == null && enableDebugLogs)
        {
            Debug.LogWarning("PlayerMeleeAttack: Animator не найден. Анимации атаки не будут работать.");
        }

        if (playerMovement == null && enableDebugLogs)
        {
            Debug.LogWarning("PlayerMeleeAttack: PlayerMovement не найден. Интеграция с движением недоступна.");
        }
    }

    /// <summary>
    /// Обрабатывает ввод для атаки
    /// </summary>
    private void HandleAttackInput()
    {
        // Проверяем нажатие Mouse0 и возможность атаки
        if (Input.GetMouseButtonDown(0) && CanAttack())
        {
            StartAttack();
        }
    }

    /// <summary>
    /// Проверяет, может ли игрок атаковать
    /// </summary>
    private bool CanAttack()
    {
        // Не можем атаковать если уже атакуем
        if (isAttacking) return false;

        // Проверяем кулдаун
        if (Time.time < lastAttackTime + attackCooldown) return false;

        // Не можем атаковать во время dash (если есть PlayerMovement)
        if (playerMovement != null && playerMovement.IsDashing()) return false;

        return true;
    }

    /// <summary>
    /// Начинает атаку
    /// </summary>
    private void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Уведомляем другие системы
        OnAttackStarted?.Invoke();

        // Запускаем анимацию атаки
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("isAttacking", true);
        }

        // Выполняем атаку
        PerformAttack();

        // Запускаем корутину завершения атаки
        StartCoroutine(AttackRoutine());

        if (enableDebugLogs)
        {
            Debug.Log("PlayerMeleeAttack: Начата атака");
        }
    }

    /// <summary>
    /// Корутина выполнения атаки
    /// </summary>
    private IEnumerator AttackRoutine()
    {
        // Ждём завершения анимации атаки
        yield return new WaitForSeconds(attackDuration);

        // Завершаем атаку
        EndAttack();
    }

    /// <summary>
    /// Завершает атаку
    /// </summary>
    private void EndAttack()
    {
        isAttacking = false;

        // Обновляем аниматор
        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
        }

        // Уведомляем другие системы
        OnAttackEnded?.Invoke();

        if (enableDebugLogs)
        {
            Debug.Log("PlayerMeleeAttack: Атака завершена");
        }
    }

    /// <summary>
    /// Выполняет логику атаки (поиск и нанесение урона врагам)
    /// </summary>
    private void PerformAttack()
    {
        // Определяем центр атаки
        Vector2 attackCenter = CalculateAttackCenter();

        if (enableDebugLogs)
        {
            Debug.Log($"PlayerMeleeAttack: Центр атаки: {attackCenter}, Радиус: {attackRange}");
        }

        // Ищем всех врагов в зоне атаки
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackCenter, attackRange, enemyLayer);

        if (enableDebugLogs)
        {
            Debug.Log($"PlayerMeleeAttack: Найдено {hitColliders.Length} коллайдеров в зоне атаки");
        }

        bool hitAnyEnemy = false;

        // Обрабатываем каждый найденный коллайдер
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                if (DamageEnemy(collider.gameObject))
                {
                    hitAnyEnemy = true;
                }
            }
        }

        // Визуальная обратная связь
        StartCoroutine(FlashAttack());

        if (!hitAnyEnemy && enableDebugLogs)
        {
            Debug.Log("PlayerMeleeAttack: Не попали ни по одному врагу");
        }
    }

    /// <summary>
    /// Вычисляет центр зоны атаки
    /// </summary>
    private Vector2 CalculateAttackCenter()
    {
        // Получаем направление атаки
        Vector2 attackDirection = GetAttackDirection();

        // Вычисляем центр атаки с учётом направления и смещения
        Vector2 center = (Vector2)playerTransform.position + attackDirection * attackOffset.x;

        return center;
    }

    /// <summary>
    /// Определяет направление атаки
    /// </summary>
    private Vector2 GetAttackDirection()
    {
        // Если есть PlayerMovement, используем его направление
        if (playerMovement != null)
        {
            Vector2 movementDir = playerMovement.GetLastMovementDirection();
            if (movementDir.magnitude > 0.1f)
            {
                return movementDir.normalized;
            }
        }

        // По умолчанию атакуем вправо
        return Vector2.right;
    }

    /// <summary>
    /// Наносит урон врагу
    /// </summary>
    private bool DamageEnemy(GameObject enemy)
    {
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage((int)attackDamage);

            // Уведомляем о попадании
            OnEnemyHit?.Invoke(enemy);

            if (enableDebugLogs)
            {
                Debug.Log($"PlayerMeleeAttack: Нанесён урон {attackDamage} врагу {enemy.name}");
            }

            return true;
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning($"PlayerMeleeAttack: У врага {enemy.name} нет компонента EnemyHealth!");
        }

        return false;
    }

    /// <summary>
    /// Корутина визуальной вспышки при атаке
    /// </summary>
    private IEnumerator FlashAttack()
    {
        if (playerSprite != null)
        {
            playerSprite.color = attackColor;
            yield return new WaitForSeconds(flashDuration);
            playerSprite.color = originalColor;
        }
    }

    #region Публичные методы для других систем

    /// <summary>
    /// Проверяет, атакует ли игрок в данный момент
    /// </summary>
    public bool IsAttacking()
    {
        return isAttacking;
    }

    /// <summary>
    /// Получает текущий урон атаки
    /// </summary>
    public float GetAttackDamage()
    {
        return attackDamage;
    }

    /// <summary>
    /// Устанавливает новый урон атаки
    /// </summary>
    public void SetAttackDamage(float newDamage)
    {
        attackDamage = newDamage;

        if (enableDebugLogs)
        {
            Debug.Log($"PlayerMeleeAttack: Урон атаки изменён на {newDamage}");
        }
    }

    /// <summary>
    /// Получает радиус атаки
    /// </summary>
    public float GetAttackRange()
    {
        return attackRange;
    }

    /// <summary>
    /// Устанавливает новый радиус атаки
    /// </summary>
    public void SetAttackRange(float newRange)
    {
        attackRange = newRange;

        if (enableDebugLogs)
        {
            Debug.Log($"PlayerMeleeAttack: Радиус атаки изменён на {newRange}");
        }
    }

    /// <summary>
    /// Принудительно прерывает атаку (для других систем)
    /// </summary>
    public void InterruptAttack()
    {
        if (isAttacking)
        {
            StopAllCoroutines();
            EndAttack();

            if (enableDebugLogs)
            {
                Debug.Log("PlayerMeleeAttack: Атака прервана принудительно");
            }
        }
    }

    #endregion

    #region Гизмо для отладки

    /// <summary>
    /// Отображает зону атаки в редакторе (при выделении объекта)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!showAttackGizmos) return;

        DrawAttackGizmo(Color.red);
    }

    /// <summary>
    /// Отображает зону атаки в редакторе (всегда видимо)
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showAttackGizmos) return;

        DrawAttackGizmo(new Color(1, 0, 0, 0.3f)); // Полупрозрачный красный
    }

    /// <summary>
    /// Рисует гизмо зоны атаки
    /// </summary>
    private void DrawAttackGizmo(Color color)
    {
        if (playerTransform == null)
            playerTransform = transform;

        Vector2 attackCenter = CalculateAttackCenter();
        
        Gizmos.color = color;
        Gizmos.DrawWireSphere(attackCenter, attackRange);

        // Рисуем направление атаки
        Gizmos.color = Color.yellow;
        Vector2 attackDirection = GetAttackDirection();
        Gizmos.DrawRay(playerTransform.position, attackDirection * attackOffset.x);
    }

    #endregion
} 