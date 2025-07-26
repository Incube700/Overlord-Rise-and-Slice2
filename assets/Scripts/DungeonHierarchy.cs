using UnityEngine;
using System.Collections.Generic;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Менеджер иерархии подземелья
    /// Управляет прогрессией через 100 уровней замка
    /// </summary>
    public class DungeonHierarchy : MonoBehaviour
    {
        [Header("Настройки подземелья")]
        public int currentLevel = 100; // Начинаем с самого дна
        public int maxLevel = 100;
        public int minLevel = 1;
        
        [Header("Ярусы иерархии")]
        public LevelTier[] tiers;
        
        [Header("Прогрессия Overlord")]
        public int baseAttackDamage = 5;
        public int baseHealth = 50;
        public float baseMovementSpeed = 3f;
        
        [Header("Отладка")]
        public bool showDebugInfo = true;
        
        // События
        public System.Action<int> OnLevelChanged;
        public System.Action<LevelTier> OnTierChanged;
        public System.Action<float> OnPowerRestored;
        
        // Текущие значения
        private LevelTier currentTier;
        private float currentPowerLevel;
        
        // Компоненты
        private LevelGenerator levelGenerator;
        
        void Start()
        {
            InitializeHierarchy();
            UpdateCurrentTier();
            CalculatePowerLevel();
        }
        
        /// <summary>
        /// Инициализация системы иерархии
        /// </summary>
        void InitializeHierarchy()
        {
            if (tiers == null || tiers.Length == 0)
            {
                CreateDefaultTiers();
            }
            
            levelGenerator = FindObjectOfType<LevelGenerator>();
            
            if (showDebugInfo)
            {
                Debug.Log($"[DungeonHierarchy] Инициализация завершена. Текущий уровень: {currentLevel}");
            }
        }
        
        /// <summary>
        /// Создаёт стандартные ярусы иерархии
        /// </summary>
        void CreateDefaultTiers()
        {
            tiers = new LevelTier[]
            {
                new LevelTier
                {
                    minLevel = 1, maxLevel = 19,
                    tierName = "Узурпаторы трона",
                    tierDescription = "На вершине сидят те, кто считает себя новыми хозяевами...",
                    difficultyMultiplier = 5.0f,
                    minEnemiesPerRoom = 1, maxEnemiesPerRoom = 2,
                    lootDropChance = 0.8f, minCoins = 50, maxCoins = 200,
                    ambientLight = new Color(1f, 0.9f, 0.7f, 1f) // Золотистый свет
                },
                new LevelTier
                {
                    minLevel = 20, maxLevel = 39,
                    tierName = "Лорды войны и командиры",
                    tierDescription = "Здесь правят те, кто когда-то командовал армиями...",
                    difficultyMultiplier = 3.5f,
                    minEnemiesPerRoom = 2, maxEnemiesPerRoom = 3,
                    lootDropChance = 0.6f, minCoins = 25, maxCoins = 100,
                    ambientLight = new Color(0.9f, 0.7f, 0.9f, 1f) // Пурпурный свет
                },
                new LevelTier
                {
                    minLevel = 40, maxLevel = 69,
                    tierName = "Профессиональные наёмники",
                    tierDescription = "Эти уровни контролируют профессионалы...",
                    difficultyMultiplier = 2.0f,
                    minEnemiesPerRoom = 2, maxEnemiesPerRoom = 4,
                    lootDropChance = 0.4f, minCoins = 10, maxCoins = 50,
                    ambientLight = new Color(0.8f, 0.8f, 1f, 1f) // Холодный синий
                },
                new LevelTier
                {
                    minLevel = 70, maxLevel = 89,
                    tierName = "Мародёры и охотники за сокровищами",
                    tierDescription = "Здесь обосновались те, кто пришёл за лёгкой наживой...",
                    difficultyMultiplier = 1.2f,
                    minEnemiesPerRoom = 3, maxEnemiesPerRoom = 5,
                    lootDropChance = 0.25f, minCoins = 3, maxCoins = 15,
                    ambientLight = new Color(1f, 0.6f, 0.4f, 1f) // Оранжевый огонь
                },
                new LevelTier
                {
                    minLevel = 90, maxLevel = 100,
                    tierName = "Падальщики и отчаявшиеся",
                    tierDescription = "На самом дне живут те, кому некуда больше идти...",
                    difficultyMultiplier = 0.5f,
                    minEnemiesPerRoom = 4, maxEnemiesPerRoom = 8,
                    lootDropChance = 0.1f, minCoins = 1, maxCoins = 5,
                    ambientLight = new Color(0.5f, 0.5f, 0.5f, 1f) // Тусклый серый
                }
            };
        }
        
        /// <summary>
        /// Поднимается на следующий уровень
        /// </summary>
        public void AscendLevel()
        {
            if (currentLevel > minLevel)
            {
                currentLevel--;
                OnLevelChanged?.Invoke(currentLevel);
                
                UpdateCurrentTier();
                CalculatePowerLevel();
                
                if (showDebugInfo)
                {
                    Debug.Log($"[DungeonHierarchy] Поднялись на уровень {currentLevel}. Ярус: {currentTier?.tierName}");
                }
                
                // Генерируем новый уровень
                if (levelGenerator != null)
                {
                    levelGenerator.GenerateRoom();
                }
            }
        }
        
        /// <summary>
        /// Спускается на предыдущий уровень
        /// </summary>
        public void DescendLevel()
        {
            if (currentLevel < maxLevel)
            {
                currentLevel++;
                OnLevelChanged?.Invoke(currentLevel);
                
                UpdateCurrentTier();
                CalculatePowerLevel();
                
                if (showDebugInfo)
                {
                    Debug.Log($"[DungeonHierarchy] Спустились на уровень {currentLevel}. Ярус: {currentTier?.tierName}");
                }
            }
        }
        
        /// <summary>
        /// Обновляет текущий ярус
        /// </summary>
        void UpdateCurrentTier()
        {
            LevelTier newTier = GetTierForLevel(currentLevel);
            if (newTier != currentTier)
            {
                currentTier = newTier;
                OnTierChanged?.Invoke(currentTier);
                
                if (showDebugInfo && currentTier != null)
                {
                    Debug.Log($"[DungeonHierarchy] Вошли в ярус: {currentTier.tierName}");
                }
            }
        }
        
        /// <summary>
        /// Получает ярус для указанного уровня
        /// </summary>
        public LevelTier GetTierForLevel(int level)
        {
            foreach (var tier in tiers)
            {
                if (tier.IsLevelInTier(level))
                    return tier;
            }
            return null;
        }
        
        /// <summary>
        /// Вычисляет текущий уровень силы Overlord (0.0 - 1.0)
        /// </summary>
        void CalculatePowerLevel()
        {
            // Чем выше поднимается, тем больше силы восстанавливает
            currentPowerLevel = 1.0f - ((float)(currentLevel - minLevel) / (maxLevel - minLevel));
            OnPowerRestored?.Invoke(currentPowerLevel);
            
            if (showDebugInfo)
            {
                Debug.Log($"[DungeonHierarchy] Уровень силы: {currentPowerLevel:F2} ({currentPowerLevel * 100:F0}%)");
            }
        }
        
        /// <summary>
        /// Получает модифицированную атаку игрока
        /// </summary>
        public int GetPlayerAttackDamage()
        {
            return Mathf.RoundToInt(baseAttackDamage * (1.0f + currentPowerLevel * 3.0f));
        }
        
        /// <summary>
        /// Получает модифицированное здоровье игрока
        /// </summary>
        public int GetPlayerMaxHealth()
        {
            return Mathf.RoundToInt(baseHealth * (1.0f + currentPowerLevel * 2.0f));
        }
        
        /// <summary>
        /// Получает модифицированную скорость игрока
        /// </summary>
        public float GetPlayerMovementSpeed()
        {
            return baseMovementSpeed * (1.0f + currentPowerLevel * 0.5f);
        }
        
        /// <summary>
        /// Получает описание текущего уровня для UI
        /// </summary>
        public string GetCurrentLevelDescription()
        {
            if (currentTier == null) return "Неизвестный уровень";
            
            string powerPercent = (currentPowerLevel * 100).ToString("F0");
            return $"Уровень {currentLevel}\n{currentTier.tierName}\nСила восстановлена на {powerPercent}%";
        }
        
        /// <summary>
        /// Получает атмосферное описание для текущего уровня
        /// </summary>
        public string GetAtmosphericDescription()
        {
            if (currentTier == null) return "";
            return currentTier.tierDescription;
        }
        
        // Публичные свойства для доступа к данным
        public int CurrentLevel => currentLevel;
        public LevelTier CurrentTier => currentTier;
        public float PowerLevel => currentPowerLevel;
        public bool IsAtTop => currentLevel == minLevel;
        public bool IsAtBottom => currentLevel == maxLevel;
        
        // Отладочная информация в редакторе
        void OnGUI()
        {
            if (!showDebugInfo) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"Уровень: {currentLevel}");
            GUILayout.Label($"Ярус: {currentTier?.tierName ?? "Неизвестный"}");
            GUILayout.Label($"Сила: {currentPowerLevel:F2} ({currentPowerLevel * 100:F0}%)");
            GUILayout.Label($"Атака: {GetPlayerAttackDamage()}");
            GUILayout.Label($"Здоровье: {GetPlayerMaxHealth()}");
            GUILayout.Label($"Скорость: {GetPlayerMovementSpeed():F1}");
            
            if (GUILayout.Button("Подняться"))
                AscendLevel();
            if (GUILayout.Button("Спуститься"))
                DescendLevel();
            
            GUILayout.EndArea();
        }
    }
} 