using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// База данных тайлов для системы генерации уровней.
/// Содержит все тайлы, используемые в игре, разделённые по категориям.
/// Назначайте тайлы через инспектор Unity.
/// </summary>
public class TileDatabase : MonoBehaviour
{
    [Header("🟫 Тайлы пола")]
    [Tooltip("Основной тайл пола - вариант 1")]
    public TileBase floorTile_1;
    
    [Tooltip("Основной тайл пола - вариант 2")]
    public TileBase floorTile_2;
    
    [Tooltip("Основной тайл пола - вариант 3")]
    public TileBase floorTile_3;

    [Header("🧱 Тайлы стен")]
    [Tooltip("Основной тайл стены - вариант 1")]
    public TileBase wallTile_1;
    
    [Tooltip("Основной тайл стены - вариант 2")]
    public TileBase wallTile_2;

    [Header("🌿 Декоративные тайлы")]
    [Tooltip("Декоративный элемент - вариант 1 (например, трава)")]
    public TileBase decorTile_1;
    
    [Tooltip("Декоративный элемент - вариант 2 (например, камни)")]
    public TileBase decorTile_2;
    
    [Tooltip("Декоративный элемент - вариант 3 (например, цветы)")]
    public TileBase decorTile_3;

    [Header("🔧 Настройки")]
    [SerializeField] private bool enableDebugLogs = true;

    /// <summary>
    /// Получает случайный тайл пола
    /// </summary>
    public TileBase GetRandomFloorTile()
    {
        TileBase[] floorTiles = { floorTile_1, floorTile_2, floorTile_3 };
        
        // Фильтруем null значения
        var validTiles = System.Array.FindAll(floorTiles, tile => tile != null);
        
        if (validTiles.Length == 0)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning("TileDatabase: Нет доступных тайлов пола!");
            }
            return null;
        }
        
        return validTiles[Random.Range(0, validTiles.Length)];
    }

    /// <summary>
    /// Получает случайный тайл стены
    /// </summary>
    public TileBase GetRandomWallTile()
    {
        TileBase[] wallTiles = { wallTile_1, wallTile_2 };
        
        // Фильтруем null значения
        var validTiles = System.Array.FindAll(wallTiles, tile => tile != null);
        
        if (validTiles.Length == 0)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning("TileDatabase: Нет доступных тайлов стен!");
            }
            return null;
        }
        
        return validTiles[Random.Range(0, validTiles.Length)];
    }

    /// <summary>
    /// Получает случайный декоративный тайл
    /// </summary>
    public TileBase GetRandomDecorTile()
    {
        TileBase[] decorTiles = { decorTile_1, decorTile_2, decorTile_3 };
        
        // Фильтруем null значения
        var validTiles = System.Array.FindAll(decorTiles, tile => tile != null);
        
        if (validTiles.Length == 0)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning("TileDatabase: Нет доступных декоративных тайлов!");
            }
            return null;
        }
        
        return validTiles[Random.Range(0, validTiles.Length)];
    }

    /// <summary>
    /// Получает конкретный тайл пола по индексу (1-3)
    /// </summary>
    public TileBase GetFloorTile(int index)
    {
        switch (index)
        {
            case 1: return floorTile_1;
            case 2: return floorTile_2;
            case 3: return floorTile_3;
            default:
                if (enableDebugLogs)
                {
                    Debug.LogWarning($"TileDatabase: Неверный индекс тайла пола: {index}. Используйте 1-3.");
                }
                return floorTile_1;
        }
    }

    /// <summary>
    /// Получает конкретный тайл стены по индексу (1-2)
    /// </summary>
    public TileBase GetWallTile(int index)
    {
        switch (index)
        {
            case 1: return wallTile_1;
            case 2: return wallTile_2;
            default:
                if (enableDebugLogs)
                {
                    Debug.LogWarning($"TileDatabase: Неверный индекс тайла стены: {index}. Используйте 1-2.");
                }
                return wallTile_1;
        }
    }

    /// <summary>
    /// Получает конкретный декоративный тайл по индексу (1-3)
    /// </summary>
    public TileBase GetDecorTile(int index)
    {
        switch (index)
        {
            case 1: return decorTile_1;
            case 2: return decorTile_2;
            case 3: return decorTile_3;
            default:
                if (enableDebugLogs)
                {
                    Debug.LogWarning($"TileDatabase: Неверный индекс декоративного тайла: {index}. Используйте 1-3.");
                }
                return decorTile_1;
        }
    }

    /// <summary>
    /// Проверяет, что все необходимые тайлы назначены
    /// </summary>
    public bool ValidateDatabase()
    {
        bool isValid = true;
        
        // Проверяем тайлы пола
        if (floorTile_1 == null && floorTile_2 == null && floorTile_3 == null)
        {
            Debug.LogError("TileDatabase: Не назначен ни один тайл пола!");
            isValid = false;
        }
        
        // Проверяем тайлы стен
        if (wallTile_1 == null && wallTile_2 == null)
        {
            Debug.LogError("TileDatabase: Не назначен ни один тайл стены!");
            isValid = false;
        }
        
        // Декоративные тайлы опциональны, но предупредим
        if (decorTile_1 == null && decorTile_2 == null && decorTile_3 == null)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning("TileDatabase: Не назначен ни один декоративный тайл. Декор не будет генерироваться.");
            }
        }
        
        if (isValid && enableDebugLogs)
        {
            Debug.Log("TileDatabase: База данных тайлов валидна!");
        }
        
        return isValid;
    }

    void Start()
    {
        ValidateDatabase();
    }
} 