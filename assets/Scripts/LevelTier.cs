using UnityEngine;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Система иерархии уровней подземелья
    /// Определяет характеристики каждого яруса замка
    /// </summary>
    [System.Serializable]
    public class LevelTier
    {
        [Header("Диапазон уровней")]
        public int minLevel = 1;
        public int maxLevel = 20;
        
        [Header("Название и описание")]
        public string tierName = "Узурпаторы трона";
        [TextArea(2, 4)]
        public string tierDescription = "На вершине сидят те, кто считает себя новыми хозяевами...";
        
        [Header("Визуальные настройки")]
        public TileSet floorTiles;
        public TileSet wallTiles;
        public TileSet decorTiles;
        public Color ambientLight = Color.white;
        
        [Header("Враги и сложность")]
        public EnemySpawnData[] enemies;
        public float difficultyMultiplier = 1.0f;
        public int minEnemiesPerRoom = 1;
        public int maxEnemiesPerRoom = 3;
        
        [Header("Лут и награды")]
        public LootTable lootTable;
        public float lootDropChance = 0.3f;
        public int minCoins = 1;
        public int maxCoins = 10;
        
        [Header("Интерактивы")]
        public InteractiveObject[] possibleInteractives;
        public float interactiveSpawnChance = 0.15f;
        
        /// <summary>
        /// Проверяет, подходит ли уровень для этого яруса
        /// </summary>
        public bool IsLevelInTier(int level)
        {
            return level >= minLevel && level <= maxLevel;
        }
        
        /// <summary>
        /// Получает множитель сложности для конкретного уровня в ярусе
        /// </summary>
        public float GetDifficultyForLevel(int level)
        {
            if (!IsLevelInTier(level)) return difficultyMultiplier;
            
            // Чем выше уровень в ярусе, тем сложнее
            float tierProgress = (float)(level - minLevel) / (maxLevel - minLevel);
            return difficultyMultiplier * (1.0f + tierProgress * 0.5f);
        }
        
        /// <summary>
        /// Получает случайного врага для этого яруса
        /// </summary>
        public EnemySpawnData GetRandomEnemy()
        {
            if (enemies == null || enemies.Length == 0) return null;
            return enemies[Random.Range(0, enemies.Length)];
        }
        
        /// <summary>
        /// Получает случайный интерактивный объект
        /// </summary>
        public InteractiveObject GetRandomInteractive()
        {
            if (possibleInteractives == null || possibleInteractives.Length == 0) return null;
            return possibleInteractives[Random.Range(0, possibleInteractives.Length)];
        }
    }
    
    /// <summary>
    /// Набор тайлов для определённого яруса
    /// </summary>
    [System.Serializable]
    public class TileSet
    {
        public TileBase[] tiles;
        
        public TileBase GetRandomTile()
        {
            if (tiles == null || tiles.Length == 0) return null;
            return tiles[Random.Range(0, tiles.Length)];
        }
    }
    
    /// <summary>
    /// Данные для спавна врагов
    /// </summary>
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public string enemyName;
        public int baseHealth = 20;
        public float spawnWeight = 1.0f; // Вероятность спавна
        
        [TextArea(1, 3)]
        public string loreDescription;
    }
    
    /// <summary>
    /// Таблица лута для яруса
    /// </summary>
    [System.Serializable]
    public class LootTable
    {
        public LootItem[] items;
        
        public GameObject GetRandomLoot()
        {
            if (items == null || items.Length == 0) return null;
            
            float totalWeight = 0f;
            foreach (var item in items)
                totalWeight += item.dropWeight;
            
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;
            
            foreach (var item in items)
            {
                currentWeight += item.dropWeight;
                if (randomValue <= currentWeight)
                    return item.itemPrefab;
            }
            
            return items[0].itemPrefab; // Fallback
        }
    }
    
    /// <summary>
    /// Предмет лута с весом выпадения
    /// </summary>
    [System.Serializable]
    public class LootItem
    {
        public GameObject itemPrefab;
        public string itemName;
        public float dropWeight = 1.0f;
        
        [TextArea(1, 2)]
        public string itemDescription;
    }
    
    /// <summary>
    /// Интерактивный объект для яруса
    /// </summary>
    [System.Serializable]
    public class InteractiveObject
    {
        public GameObject prefab;
        public string objectName;
        public InteractiveType type;
        
        [TextArea(1, 3)]
        public string loreText;
    }
    
    /// <summary>
    /// Типы интерактивных объектов
    /// </summary>
    public enum InteractiveType
    {
        Statue,      // Статуя
        Chest,       // Сундук
        Altar,       // Алтарь
        BookShelf,   // Книжная полка
        Throne,      // Трон
        Fountain,    // Фонтан
        Grave,       // Могила
        Portal,      // Портал
        Merchant,    // Торговец
        Puzzle       // Головоломка
    }
} 