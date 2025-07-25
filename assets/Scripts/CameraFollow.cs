using UnityEngine;

/// <summary>
/// Система следования камеры за игроком для top-down 2D игры.
/// Обеспечивает плавное движение камеры с настраиваемой скоростью.
/// Поддерживает ограничения по осям и зоны мёртвого пространства.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("🎯 Цель следования")]
    [Tooltip("Игрок или другой объект, за которым следует камера")]
    [SerializeField] private Transform target;

    [Header("⚡ Настройки движения")]
    [Tooltip("Скорость следования камеры (0.1 = медленно, 1.0 = мгновенно)")]
    [Range(0.01f, 1f)]
    [SerializeField] private float followSpeed = 0.125f;

    [Tooltip("Смещение камеры относительно игрока")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("🚫 Зона мёртвого пространства")]
    [Tooltip("Включить зону мёртвого пространства (камера не двигается, пока игрок в зоне)")]
    [SerializeField] private bool useDeadZone = false;

    [Tooltip("Размер зоны мёртвого пространства")]
    [SerializeField] private Vector2 deadZoneSize = new Vector2(1f, 1f);

    [Header("🔒 Ограничения движения")]
    [Tooltip("Ограничить движение камеры по границам уровня")]
    [SerializeField] private bool useBounds = false;

    [Tooltip("Минимальные координаты камеры")]
    [SerializeField] private Vector2 minBounds = new Vector2(-10, -10);

    [Tooltip("Максимальные координаты камеры")]
    [SerializeField] private Vector2 maxBounds = new Vector2(10, 10);

    [Header("🔧 Отладка")]
    [SerializeField] private bool enableDebugLogs = false;
    [SerializeField] private bool showGizmos = true;

    // Внутренние переменные
    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private Vector3 lastTargetPosition;

    void Start()
    {
        InitializeCamera();
        FindPlayerIfNeeded();
    }

    void LateUpdate()
    {
        if (target != null)
        {
            FollowTarget();
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning("CameraFollow: Цель не назначена! Камера не будет следовать.");
        }
    }

    /// <summary>
    /// Инициализирует компонент камеры
    /// </summary>
    private void InitializeCamera()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraFollow: Скрипт должен быть на объекте с компонентом Camera!");
        }

        if (enableDebugLogs)
        {
            Debug.Log("CameraFollow: Камера инициализирована");
        }
    }

    /// <summary>
    /// Ищет игрока автоматически, если цель не назначена
    /// </summary>
    private void FindPlayerIfNeeded()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                if (enableDebugLogs)
                {
                    Debug.Log($"CameraFollow: Автоматически найден игрок: {player.name}");
                }
            }
            else
            {
                Debug.LogWarning("CameraFollow: Игрок не найден! Убедитесь, что у игрока есть тег 'Player' или назначьте цель вручную.");
            }
        }
    }

    /// <summary>
    /// Основная логика следования за целью
    /// </summary>
    private void FollowTarget()
    {
        Vector3 targetPosition = target.position + offset;

        // Проверяем зону мёртвого пространства
        if (useDeadZone && IsInDeadZone(target.position))
        {
            return; // Не двигаем камеру, если цель в мёртвой зоне
        }

        // Вычисляем желаемую позицию
        Vector3 desiredPosition = targetPosition;

        // Применяем ограничения по границам
        if (useBounds)
        {
            desiredPosition = ApplyBounds(desiredPosition);
        }

        // Плавно перемещаем камеру
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSpeed);
        transform.position = smoothedPosition;

        // Логгирование для отладки
        if (enableDebugLogs && Vector3.Distance(lastTargetPosition, target.position) > 0.1f)
        {
            Debug.Log($"CameraFollow: Следую за целью. Позиция камеры: {transform.position}");
            lastTargetPosition = target.position;
        }
    }

    /// <summary>
    /// Проверяет, находится ли цель в зоне мёртвого пространства
    /// </summary>
    private bool IsInDeadZone(Vector3 targetPos)
    {
        Vector3 cameraPos = transform.position;
        Vector3 difference = targetPos - cameraPos;

        return Mathf.Abs(difference.x) <= deadZoneSize.x / 2f && 
               Mathf.Abs(difference.y) <= deadZoneSize.y / 2f;
    }

    /// <summary>
    /// Применяет ограничения по границам уровня
    /// </summary>
    private Vector3 ApplyBounds(Vector3 desiredPosition)
    {
        float clampedX = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);

        return new Vector3(clampedX, clampedY, desiredPosition.z);
    }

    #region Публичные методы для других систем

    /// <summary>
    /// Устанавливает новую цель для следования
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        
        if (enableDebugLogs)
        {
            Debug.Log($"CameraFollow: Новая цель установлена: {(newTarget != null ? newTarget.name : "null")}");
        }
    }

    /// <summary>
    /// Получает текущую цель
    /// </summary>
    public Transform GetTarget()
    {
        return target;
    }

    /// <summary>
    /// Устанавливает скорость следования
    /// </summary>
    public void SetFollowSpeed(float newSpeed)
    {
        followSpeed = Mathf.Clamp01(newSpeed);
        
        if (enableDebugLogs)
        {
            Debug.Log($"CameraFollow: Скорость следования изменена на {followSpeed}");
        }
    }

    /// <summary>
    /// Устанавливает смещение камеры
    /// </summary>
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
        
        if (enableDebugLogs)
        {
            Debug.Log($"CameraFollow: Смещение камеры изменено на {offset}");
        }
    }

    /// <summary>
    /// Мгновенно перемещает камеру к цели (без плавности)
    /// </summary>
    public void SnapToTarget()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;
            
            if (useBounds)
            {
                targetPosition = ApplyBounds(targetPosition);
            }
            
            transform.position = targetPosition;
            
            if (enableDebugLogs)
            {
                Debug.Log("CameraFollow: Камера мгновенно перемещена к цели");
            }
        }
    }

    /// <summary>
    /// Включает/выключает использование границ
    /// </summary>
    public void SetUseBounds(bool use, Vector2 min, Vector2 max)
    {
        useBounds = use;
        minBounds = min;
        maxBounds = max;
        
        if (enableDebugLogs)
        {
            Debug.Log($"CameraFollow: Границы {(use ? "включены" : "выключены")}. Min: {min}, Max: {max}");
        }
    }

    /// <summary>
    /// Включает/выключает зону мёртвого пространства
    /// </summary>
    public void SetDeadZone(bool use, Vector2 size)
    {
        useDeadZone = use;
        deadZoneSize = size;
        
        if (enableDebugLogs)
        {
            Debug.Log($"CameraFollow: Мёртвая зона {(use ? "включена" : "выключена")}. Размер: {size}");
        }
    }

    #endregion

    #region Отладка в редакторе

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // Рисуем зону мёртвого пространства
        if (useDeadZone)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneSize.x, deadZoneSize.y, 0));
            
            Gizmos.color = new Color(1, 1, 0, 0.1f);
            Gizmos.DrawCube(transform.position, new Vector3(deadZoneSize.x, deadZoneSize.y, 0));
        }

        // Рисуем границы уровня
        if (useBounds)
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2f, (minBounds.y + maxBounds.y) / 2f, 0);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0);
            Gizmos.DrawWireCube(center, size);
        }

        // Рисуем линию к цели
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
            
            // Рисуем позицию цели с учётом смещения
            Vector3 targetWithOffset = target.position + offset;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(targetWithOffset, 0.5f);
        }
    }

    #endregion
} 