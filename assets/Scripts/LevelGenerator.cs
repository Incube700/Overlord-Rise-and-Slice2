using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Генератор уровней для создания комнат через Unity Tilemap.
/// Создает базовые комнаты с полом и стенами по периметру.
/// Поддерживает настройку размеров и автоматическую генерацию коллизий.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [Header("Настройки генерации")]
    [SerializeField] private int roomWidth = 10;  // Ширина комнаты в тайлах
    [SerializeField] private int roomHeight = 10; // Высота комнаты в тайлах
    [SerializeField] private bool generateOnStart = true; // Генерировать при старте
    
    [Header("Tilemap компоненты")]
    [SerializeField] private Tilemap floorTilemap;  // Тайлмап для пола
    [SerializeField] private Tilemap wallTilemap;   // Тайлмап для стен
    [SerializeField] private TileBase floorTile;    // Тайл пола
    [SerializeField] private TileBase wallTile;     // Тайл стены
    
    [Header("Коллизии")]
    [SerializeField] private bool setupCollisions = true; // Настраивать коллизии автоматически
    
    void Start()
    {
        if (generateOnStart)
        {
            GenerateRoom();
        }
    }
    
    /// <summary>
    /// Генерирует базовую комнату с полом и стенами по периметру
    /// </summary>
    public void GenerateRoom()
    {
        if (!ValidateComponents())
        {
            Debug.LogError("LevelGenerator: Не все компоненты настроены правильно!");
            return;
        }
        
        ClearExistingLevel();
        GenerateFloor();
        GenerateWalls();
        
        if (setupCollisions)
        {
            SetupCollisions();
        }
        
        Debug.Log($"LevelGenerator: Сгенерирована комната размером {roomWidth}x{roomHeight}");
    }
    
    /// <summary>
    /// Проверяет, что все необходимые компоненты настроены
    /// </summary>
    private bool ValidateComponents()
    {
        if (floorTilemap == null)
        {
            Debug.LogError("LevelGenerator: Floor Tilemap не назначен!");
            return false;
        }
        
        if (wallTilemap == null)
        {
            Debug.LogError("LevelGenerator: Wall Tilemap не назначен!");
            return false;
        }
        
        if (floorTile == null)
        {
            Debug.LogWarning("LevelGenerator: Floor Tile не назначен, пол не будет сгенерирован");
        }
        
        if (wallTile == null)
        {
            Debug.LogWarning("LevelGenerator: Wall Tile не назначен, стены не будут сгенерированы");
        }
        
        return true;
    }
    
    /// <summary>
    /// Очищает существующий уровень
    /// </summary>
    private void ClearExistingLevel()
    {
        if (floorTilemap != null)
            floorTilemap.SetTilesBlock(new BoundsInt(-50, -50, 0, 100, 100, 1), new TileBase[100 * 100]);
        
        if (wallTilemap != null)
            wallTilemap.SetTilesBlock(new BoundsInt(-50, -50, 0, 100, 100, 1), new TileBase[100 * 100]);
    }
    
    /// <summary>
    /// Генерирует пол комнаты
    /// </summary>
    private void GenerateFloor()
    {
        if (floorTile == null || floorTilemap == null) return;
        
        // Заполняем всю комнату полом
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                floorTilemap.SetTile(position, floorTile);
            }
        }
    }
    
    /// <summary>
    /// Генерирует стены по периметру комнаты
    /// </summary>
    private void GenerateWalls()
    {
        if (wallTile == null || wallTilemap == null) return;
        
        // Верхняя и нижняя стены
        for (int x = 0; x < roomWidth; x++)
        {
            // Нижняя стена (y = 0)
            Vector3Int bottomWall = new Vector3Int(x, 0, 0);
            wallTilemap.SetTile(bottomWall, wallTile);
            
            // Верхняя стена (y = roomHeight - 1)
            Vector3Int topWall = new Vector3Int(x, roomHeight - 1, 0);
            wallTilemap.SetTile(topWall, wallTile);
        }
        
        // Левая и правая стены
        for (int y = 0; y < roomHeight; y++)
        {
            // Левая стена (x = 0)
            Vector3Int leftWall = new Vector3Int(0, y, 0);
            wallTilemap.SetTile(leftWall, wallTile);
            
            // Правая стена (x = roomWidth - 1)
            Vector3Int rightWall = new Vector3Int(roomWidth - 1, y, 0);
            wallTilemap.SetTile(rightWall, wallTile);
        }
    }
    
    /// <summary>
    /// Настраивает коллизии для тайлмапов
    /// </summary>
    private void SetupCollisions()
    {
        // Настройка коллизий для стен
        if (wallTilemap != null)
        {
            SetupTilemapCollision(wallTilemap, "Wall");
        }
        
        // Пол обычно не нуждается в коллизиях для top-down игры,
        // но можно добавить для специальных эффектов
        Debug.Log("LevelGenerator: Коллизии настроены");
    }
    
    /// <summary>
    /// Настраивает коллизии для конкретного тайлмапа
    /// </summary>
    private void SetupTilemapCollision(Tilemap tilemap, string tag)
    {
        GameObject tilemapObject = tilemap.gameObject;
        
        // Добавляем TilemapCollider2D если его нет
        TilemapCollider2D tilemapCollider = tilemapObject.GetComponent<TilemapCollider2D>();
        if (tilemapCollider == null)
        {
            tilemapCollider = tilemapObject.AddComponent<TilemapCollider2D>();
        }
        
        // Добавляем CompositeCollider2D для оптимизации коллизий
        CompositeCollider2D compositeCollider = tilemapObject.GetComponent<CompositeCollider2D>();
        if (compositeCollider == null)
        {
            compositeCollider = tilemapObject.AddComponent<CompositeCollider2D>();
            
            // Настраиваем TilemapCollider2D для работы с CompositeCollider2D
            // В Unity 6.0 usedByComposite устарело, используем новый способ
            tilemapCollider.includeLayers = new LayerMask();
            tilemapCollider.excludeLayers = new LayerMask();
            
            // Добавляем Rigidbody2D (требуется для CompositeCollider2D)
            Rigidbody2D rb = tilemapObject.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = tilemapObject.AddComponent<Rigidbody2D>();
            }
            
            // Настраиваем Rigidbody2D как статический объект
            rb.bodyType = RigidbodyType2D.Static;
        }
        
        // Устанавливаем тег
        if (!string.IsNullOrEmpty(tag))
        {
            tilemapObject.tag = tag;
        }
    }
    
    /// <summary>
    /// Генерирует новую комнату с заданными размерами
    /// </summary>
    public void GenerateRoom(int width, int height)
    {
        roomWidth = width;
        roomHeight = height;
        GenerateRoom();
    }
    
    /// <summary>
    /// Получает центр комнаты в мировых координатах
    /// </summary>
    public Vector3 GetRoomCenter()
    {
        Vector3 centerInTiles = new Vector3(roomWidth / 2f, roomHeight / 2f, 0);
        
        // Конвертируем координаты тайлов в мировые координаты
        if (floorTilemap != null)
        {
            return floorTilemap.CellToWorld(Vector3Int.FloorToInt(centerInTiles));
        }
        
        return centerInTiles;
    }
    
    /// <summary>
    /// Получает случайную позицию внутри комнаты (не на стенах)
    /// </summary>
    public Vector3 GetRandomPositionInRoom()
    {
        int randomX = Random.Range(1, roomWidth - 1);  // Исключаем стены
        int randomY = Random.Range(1, roomHeight - 1); // Исключаем стены
        
        Vector3Int tilePosition = new Vector3Int(randomX, randomY, 0);
        
        if (floorTilemap != null)
        {
            return floorTilemap.CellToWorld(tilePosition);
        }
        
        return new Vector3(randomX, randomY, 0);
    }
} 