using UnityEngine;
using System.Collections.Generic;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Система прогрессии игрока
    /// Управляет душами, уровнями, способностями и лором
    /// </summary>
    public class PlayerProgression : MonoBehaviour
    {
        [Header("Прогрессия")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int soulsCollected = 0;
        [SerializeField] private int soulsRequiredForNextLevel = 100;
        [SerializeField] private int maxLevel = 50;
        
        [Header("Способности")]
        [SerializeField] private List<PlayerAbility> unlockedAbilities = new List<PlayerAbility>();
        [SerializeField] private List<PlayerAbility> availableAbilities = new List<PlayerAbility>();
        
        [Header("Лор и история")]
        [SerializeField] private int dungeonLevel = 1;
        [SerializeField] private string currentLore = "Вы - падший лорд, ищущий силу в глубинах подземелий...";
        [SerializeField] private List<string> discoveredLore = new List<string>();
        
        [Header("Настройки")]
        [SerializeField] private bool enableProgressionReset = true;
        [SerializeField] private bool enableLoreDiscovery = true;
        
        [Header("Отладка")]
        [SerializeField] private bool enableDebugLogs = true;
        
        // События для других систем
        public System.Action<int, int> OnSoulsChanged; // current, required
        public System.Action<int> OnLevelUp;
        public System.Action<PlayerAbility> OnAbilityUnlocked;
        public System.Action<string> OnLoreDiscovered;
        public System.Action<int> OnDungeonLevelChanged;
        public System.Action OnProgressionReset;
        
        // Синглтон
        public static PlayerProgression Instance { get; private set; }
        
        private void Awake()
        {
            InitializeSingleton();
            InitializeProgression();
        }
        
        private void Start()
        {
            SubscribeToEvents();
            LoadProgression();
        }
        
        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                if (enableDebugLogs)
                {
                    Debug.Log("PlayerProgression: Синглтон инициализирован");
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeProgression()
        {
            // Инициализируем базовые способности
            InitializeBaseAbilities();
            
            // Инициализируем базовый лор
            InitializeBaseLore();
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerProgression: Прогрессия инициализирована");
            }
        }
        
        private void InitializeBaseAbilities()
        {
            // Базовые способности доступные с начала
            availableAbilities.Add(new PlayerAbility("Basic Attack", "Базовая атака", 1));
            availableAbilities.Add(new PlayerAbility("Dash", "Уклонение", 1));
            availableAbilities.Add(new PlayerAbility("Soul Sense", "Чувство душ", 2));
            
            // Разблокируем базовые способности
            UnlockAbility("Basic Attack");
            UnlockAbility("Dash");
        }
        
        private void InitializeBaseLore()
        {
            discoveredLore.Add("Древние подземелья хранят силу падших лордов...");
            discoveredLore.Add("Души врагов содержат фрагменты забытой магии...");
            discoveredLore.Add("Чем глубже спускаешься, тем сильнее становятся враги...");
        }
        
        private void SubscribeToEvents()
        {
            // Подписываемся на события смерти врагов
            EnemyHealth[] enemyHealths = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
            foreach (var enemyHealth in enemyHealths)
            {
                enemyHealth.OnDeath += OnEnemyDeath;
            }
        }
        
        #region Souls System
        
        /// <summary>
        /// Добавляет души игроку
        /// </summary>
        /// <param name="amount">Количество душ</param>
        public void AddSouls(int amount)
        {
            if (amount <= 0) return;
            
            soulsCollected += amount;
            
            OnSoulsChanged?.Invoke(soulsCollected, soulsRequiredForNextLevel);
            
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerProgression: Получено {amount} душ. Всего: {soulsCollected}");
            }
            
            // Проверяем возможность повышения уровня
            CheckLevelUp();
        }
        
        /// <summary>
        /// Тратит души на способности
        /// </summary>
        /// <param name="amount">Количество душ для траты</param>
        /// <returns>true если души потрачены успешно</returns>
        public bool SpendSouls(int amount)
        {
            if (soulsCollected < amount) return false;
            
            soulsCollected -= amount;
            
            OnSoulsChanged?.Invoke(soulsCollected, soulsRequiredForNextLevel);
            
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerProgression: Потрачено {amount} душ. Осталось: {soulsCollected}");
            }
            
            return true;
        }
        
        /// <summary>
        /// Проверяет возможность повышения уровня
        /// </summary>
        private void CheckLevelUp()
        {
            if (soulsCollected >= soulsRequiredForNextLevel && currentLevel < maxLevel)
            {
                LevelUp();
            }
        }
        
        /// <summary>
        /// Повышает уровень игрока
        /// </summary>
        private void LevelUp()
        {
            currentLevel++;
            soulsCollected -= soulsRequiredForNextLevel;
            
            // Увеличиваем требования для следующего уровня
            soulsRequiredForNextLevel = CalculateSoulsRequired(currentLevel);
            
            OnLevelUp?.Invoke(currentLevel);
            OnSoulsChanged?.Invoke(soulsCollected, soulsRequiredForNextLevel);
            
            // Разблокируем новые способности
            CheckNewAbilities();
            
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerProgression: Повышение уровня! Новый уровень: {currentLevel}");
            }
        }
        
        /// <summary>
        /// Вычисляет количество душ для следующего уровня
        /// </summary>
        /// <param name="level">Текущий уровень</param>
        /// <returns>Количество душ</returns>
        private int CalculateSoulsRequired(int level)
        {
            return 100 + (level - 1) * 50; // Увеличивается с каждым уровнем
        }
        
        #endregion
        
        #region Abilities System
        
        /// <summary>
        /// Разблокирует способность
        /// </summary>
        /// <param name="abilityName">Название способности</param>
        public void UnlockAbility(string abilityName)
        {
            PlayerAbility ability = availableAbilities.Find(a => a.name == abilityName);
            
            if (ability != null && !unlockedAbilities.Contains(ability))
            {
                unlockedAbilities.Add(ability);
                OnAbilityUnlocked?.Invoke(ability);
                
                if (enableDebugLogs)
                {
                    Debug.Log($"PlayerProgression: Разблокирована способность {abilityName}");
                }
            }
        }
        
        /// <summary>
        /// Проверяет доступность способности
        /// </summary>
        /// <param name="abilityName">Название способности</param>
        /// <returns>true если способность разблокирована</returns>
        public bool IsAbilityUnlocked(string abilityName)
        {
            return unlockedAbilities.Exists(a => a.name == abilityName);
        }
        
        /// <summary>
        /// Проверяет новые доступные способности
        /// </summary>
        private void CheckNewAbilities()
        {
            foreach (var ability in availableAbilities)
            {
                if (ability.requiredLevel <= currentLevel && !unlockedAbilities.Contains(ability))
                {
                    UnlockAbility(ability.name);
                }
            }
        }
        
        #endregion
        
        #region Lore System
        
        /// <summary>
        /// Добавляет новый лор
        /// </summary>
        /// <param name="lore">Текст лора</param>
        public void DiscoverLore(string lore)
        {
            if (!discoveredLore.Contains(lore))
            {
                discoveredLore.Add(lore);
                OnLoreDiscovered?.Invoke(lore);
                
                if (enableDebugLogs)
                {
                    Debug.Log($"PlayerProgression: Открыт новый лор: {lore}");
                }
            }
        }
        
        /// <summary>
        /// Изменяет уровень подземелья
        /// </summary>
        /// <param name="newLevel">Новый уровень подземелья</param>
        public void SetDungeonLevel(int newLevel)
        {
            if (newLevel != dungeonLevel)
            {
                dungeonLevel = newLevel;
                OnDungeonLevelChanged?.Invoke(dungeonLevel);
                
                // Добавляем лор в зависимости от уровня
                AddLoreForDungeonLevel(dungeonLevel);
                
                if (enableDebugLogs)
                {
                    Debug.Log($"PlayerProgression: Уровень подземелья изменен на {dungeonLevel}");
                }
            }
        }
        
        /// <summary>
        /// Добавляет лор для определенного уровня подземелья
        /// </summary>
        /// <param name="level">Уровень подземелья</param>
        private void AddLoreForDungeonLevel(int level)
        {
            switch (level)
            {
                case 1:
                    DiscoverLore("Первый уровень - здесь обитают слабые твари, оставшиеся от древних войн...");
                    break;
                case 2:
                    DiscoverLore("Второй уровень - следы древней магии становятся заметнее...");
                    break;
                case 3:
                    DiscoverLore("Третий уровень - здесь когда-то жили могущественные маги...");
                    break;
                case 5:
                    DiscoverLore("Пятый уровень - врата в глубины подземелий...");
                    break;
                case 10:
                    DiscoverLore("Десятый уровень - здесь обитают настоящие монстры...");
                    break;
            }
        }
        
        #endregion
        
        #region Event Handlers
        
        private void OnEnemyDeath()
        {
            // Добавляем души за убийство врага
            int soulsGained = CalculateSoulsForEnemy();
            AddSouls(soulsGained);
        }
        
        /// <summary>
        /// Вычисляет количество душ за врага
        /// </summary>
        /// <returns>Количество душ</returns>
        private int CalculateSoulsForEnemy()
        {
            // Базовые души + бонус за уровень подземелья
            int baseSouls = 10;
            int dungeonBonus = dungeonLevel * 2;
            
            return baseSouls + dungeonBonus;
        }
        
        #endregion
        
        #region Save/Load System
        
        /// <summary>
        /// Сохраняет прогрессию
        /// </summary>
        public void SaveProgression()
        {
            PlayerPrefs.SetInt("PlayerLevel", currentLevel);
            PlayerPrefs.SetInt("SoulsCollected", soulsCollected);
            PlayerPrefs.SetInt("DungeonLevel", dungeonLevel);
            
            // Сохраняем разблокированные способности
            string unlockedAbilitiesString = string.Join(",", unlockedAbilities.ConvertAll(a => a.name));
            PlayerPrefs.SetString("UnlockedAbilities", unlockedAbilitiesString);
            
            // Сохраняем открытый лор
            string discoveredLoreString = string.Join("|", discoveredLore);
            PlayerPrefs.SetString("DiscoveredLore", discoveredLoreString);
            
            PlayerPrefs.Save();
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerProgression: Прогрессия сохранена");
            }
        }
        
        /// <summary>
        /// Загружает прогрессию
        /// </summary>
        public void LoadProgression()
        {
            currentLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
            soulsCollected = PlayerPrefs.GetInt("SoulsCollected", 0);
            dungeonLevel = PlayerPrefs.GetInt("DungeonLevel", 1);
            
            // Загружаем разблокированные способности
            string unlockedAbilitiesString = PlayerPrefs.GetString("UnlockedAbilities", "");
            if (!string.IsNullOrEmpty(unlockedAbilitiesString))
            {
                string[] abilityNames = unlockedAbilitiesString.Split(',');
                foreach (string abilityName in abilityNames)
                {
                    UnlockAbility(abilityName);
                }
            }
            
            // Загружаем открытый лор
            string discoveredLoreString = PlayerPrefs.GetString("DiscoveredLore", "");
            if (!string.IsNullOrEmpty(discoveredLoreString))
            {
                string[] loreEntries = discoveredLoreString.Split('|');
                foreach (string lore in loreEntries)
                {
                    if (!string.IsNullOrEmpty(lore))
                    {
                        discoveredLore.Add(lore);
                    }
                }
            }
            
            // Обновляем требования для следующего уровня
            soulsRequiredForNextLevel = CalculateSoulsRequired(currentLevel);
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerProgression: Прогрессия загружена");
            }
        }
        
        /// <summary>
        /// Сбрасывает прогрессию
        /// </summary>
        public void ResetProgression()
        {
            currentLevel = 1;
            soulsCollected = 0;
            dungeonLevel = 1;
            unlockedAbilities.Clear();
            discoveredLore.Clear();
            
            // Инициализируем заново
            InitializeBaseAbilities();
            InitializeBaseLore();
            
            soulsRequiredForNextLevel = CalculateSoulsRequired(currentLevel);
            
            OnProgressionReset?.Invoke();
            
            if (enableDebugLogs)
            {
                Debug.Log("PlayerProgression: Прогрессия сброшена");
            }
        }
        
        #endregion
        
        #region Public Getters
        
        /// <summary>
        /// Получает текущий уровень игрока
        /// </summary>
        /// <returns>Текущий уровень</returns>
        public int GetCurrentLevel()
        {
            return currentLevel;
        }
        
        /// <summary>
        /// Получает количество собранных душ
        /// </summary>
        /// <returns>Количество душ</returns>
        public int GetSoulsCollected()
        {
            return soulsCollected;
        }
        
        /// <summary>
        /// Получает количество душ для следующего уровня
        /// </summary>
        /// <returns>Количество душ</returns>
        public int GetSoulsRequired()
        {
            return soulsRequiredForNextLevel;
        }
        
        /// <summary>
        /// Получает уровень подземелья
        /// </summary>
        /// <returns>Уровень подземелья</returns>
        public int GetDungeonLevel()
        {
            return dungeonLevel;
        }
        
        /// <summary>
        /// Получает все разблокированные способности
        /// </summary>
        /// <returns>Список способностей</returns>
        public List<PlayerAbility> GetUnlockedAbilities()
        {
            return new List<PlayerAbility>(unlockedAbilities);
        }
        
        /// <summary>
        /// Получает весь открытый лор
        /// </summary>
        /// <returns>Список лора</returns>
        public List<string> GetDiscoveredLore()
        {
            return new List<string>(discoveredLore);
        }
        
        #endregion
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveProgression();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                SaveProgression();
            }
        }
    }
    
    /// <summary>
    /// Класс способности игрока
    /// </summary>
    [System.Serializable]
    public class PlayerAbility
    {
        public string name;
        public string description;
        public int requiredLevel;
        public bool isUnlocked;
        
        public PlayerAbility(string name, string description, int requiredLevel)
        {
            this.name = name;
            this.description = description;
            this.requiredLevel = requiredLevel;
            this.isUnlocked = false;
        }
    }
} 