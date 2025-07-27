using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Менеджер системы частиц для визуальных эффектов
    /// Управляет всеми эффектами частиц в игре
    /// </summary>
    public class ParticleEffectsManager : MonoBehaviour
    {
        [Header("Префабы эффектов")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private GameObject dashEffectPrefab;
        [SerializeField] private GameObject deathEffectPrefab;
        [SerializeField] private GameObject damageEffectPrefab;
        [SerializeField] private GameObject healEffectPrefab;
        
        [Header("Настройки пула")]
        [SerializeField] private int initialPoolSize = 10;
        [SerializeField] private int maxPoolSize = 50;
        [SerializeField] private bool expandPool = true;
        
        [Header("Настройки эффектов")]
        [SerializeField] private float defaultEffectDuration = 2f;
        [SerializeField] private bool autoDestroyEffects = true;
        
        [Header("Отладка")]
        [SerializeField] private bool enableDebugLogs = true;
        
        // Пул объектов для переиспользования
        private Dictionary<string, Queue<GameObject>> effectPools;
        private Dictionary<string, GameObject> effectPrefabs;
        
        // Синглтон
        public static ParticleEffectsManager Instance { get; private set; }
        
        private void Awake()
        {
            InitializeSingleton();
            InitializePools();
        }
        
        private void Start()
        {
            SubscribeToEvents();
        }
        
        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                if (enableDebugLogs)
                {
                    Debug.Log("ParticleEffectsManager: Синглтон инициализирован");
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializePools()
        {
            effectPools = new Dictionary<string, Queue<GameObject>>();
            effectPrefabs = new Dictionary<string, GameObject>();
            
            // Регистрируем префабы эффектов
            RegisterEffectPrefab("Hit", hitEffectPrefab);
            RegisterEffectPrefab("Dash", dashEffectPrefab);
            RegisterEffectPrefab("Death", deathEffectPrefab);
            RegisterEffectPrefab("Damage", damageEffectPrefab);
            RegisterEffectPrefab("Heal", healEffectPrefab);
            
            // Создаем начальные пулы
            foreach (var effectType in effectPrefabs.Keys)
            {
                CreatePool(effectType, initialPoolSize);
            }
            
            if (enableDebugLogs)
            {
                Debug.Log("ParticleEffectsManager: Пул эффектов инициализирован");
            }
        }
        
        private void RegisterEffectPrefab(string effectType, GameObject prefab)
        {
            if (prefab != null)
            {
                effectPrefabs[effectType] = prefab;
                
                if (enableDebugLogs)
                {
                    Debug.Log($"ParticleEffectsManager: Зарегистрирован эффект {effectType}");
                }
            }
        }
        
        private void CreatePool(string effectType, int size)
        {
            if (!effectPrefabs.ContainsKey(effectType)) return;
            
            Queue<GameObject> pool = new Queue<GameObject>();
            
            for (int i = 0; i < size; i++)
            {
                GameObject effect = CreateEffectObject(effectType);
                effect.SetActive(false);
                pool.Enqueue(effect);
            }
            
            effectPools[effectType] = pool;
        }
        
        private GameObject CreateEffectObject(string effectType)
        {
            GameObject prefab = effectPrefabs[effectType];
            GameObject effect = Instantiate(prefab, transform);
            
            // Добавляем компонент для автоматического возврата в пул
            ParticleEffectController controller = effect.GetComponent<ParticleEffectController>();
            if (controller == null)
            {
                controller = effect.AddComponent<ParticleEffectController>();
            }
            
            controller.Initialize(effectType, this);
            
            return effect;
        }
        
        private void SubscribeToEvents()
        {
            // Подписываемся на события игрока
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.OnDashStateChanged += OnPlayerDashStateChanged;
            }
            
            // Подписываемся на события боя
            PlayerCombat playerCombat = FindFirstObjectByType<PlayerCombat>();
            if (playerCombat != null)
            {
                playerCombat.OnEnemyHit += OnPlayerHitEnemy;
            }
            
            // Подписываемся на события врагов
            EnemyHealth[] enemyHealths = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
            foreach (var enemyHealth in enemyHealths)
            {
                enemyHealth.OnDeath += OnEnemyDeath;
                enemyHealth.OnDamageTaken += OnEnemyDamageTaken;
            }
        }
        
        #region Public Methods
        
        /// <summary>
        /// Создает эффект в указанной позиции
        /// </summary>
        /// <param name="effectType">Тип эффекта</param>
        /// <param name="position">Позиция эффекта</param>
        /// <param name="rotation">Поворот эффекта</param>
        /// <param name="duration">Длительность эффекта</param>
        /// <returns>Созданный эффект</returns>
        public GameObject CreateEffect(string effectType, Vector3 position, Quaternion rotation = default, float duration = -1f)
        {
            if (!effectPrefabs.ContainsKey(effectType))
            {
                Debug.LogWarning($"ParticleEffectsManager: Неизвестный тип эффекта {effectType}");
                return null;
            }
            
            GameObject effect = GetEffectFromPool(effectType);
            if (effect == null) return null;
            
            effect.transform.position = position;
            effect.transform.rotation = rotation;
            effect.SetActive(true);
            
            // Запускаем эффект
            ParticleEffectController controller = effect.GetComponent<ParticleEffectController>();
            if (controller != null)
            {
                float effectDuration = duration > 0 ? duration : defaultEffectDuration;
                controller.PlayEffect(effectDuration);
            }
            
            if (enableDebugLogs)
            {
                Debug.Log($"ParticleEffectsManager: Создан эффект {effectType} в позиции {position}");
            }
            
            return effect;
        }
        
        /// <summary>
        /// Создает эффект попадания
        /// </summary>
        /// <param name="position">Позиция попадания</param>
        /// <param name="damage">Урон для настройки эффекта</param>
        public void CreateHitEffect(Vector3 position, int damage = 0)
        {
            CreateEffect("Hit", position);
            
            // Дополнительные эффекты в зависимости от урона
            if (damage > 20)
            {
                CreateEffect("Damage", position);
            }
        }
        
        /// <summary>
        /// Создает эффект dash
        /// </summary>
        /// <param name="position">Позиция игрока</param>
        /// <param name="direction">Направление dash</param>
        public void CreateDashEffect(Vector3 position, Vector2 direction)
        {
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);
            CreateEffect("Dash", position, rotation, 0.5f);
        }
        
        /// <summary>
        /// Создает эффект смерти
        /// </summary>
        /// <param name="position">Позиция смерти</param>
        public void CreateDeathEffect(Vector3 position)
        {
            CreateEffect("Death", position, default, 3f);
        }
        
        /// <summary>
        /// Создает эффект лечения
        /// </summary>
        /// <param name="position">Позиция лечения</param>
        public void CreateHealEffect(Vector3 position)
        {
            CreateEffect("Heal", position, default, 2f);
        }
        
        #endregion
        
        #region Pool Management
        
        private GameObject GetEffectFromPool(string effectType)
        {
            if (!effectPools.ContainsKey(effectType))
            {
                CreatePool(effectType, 1);
            }
            
            Queue<GameObject> pool = effectPools[effectType];
            
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            else if (expandPool && effectPools[effectType].Count < maxPoolSize)
            {
                // Расширяем пул
                CreatePool(effectType, 5);
                return pool.Dequeue();
            }
            
            return null;
        }
        
        /// <summary>
        /// Возвращает эффект в пул
        /// </summary>
        /// <param name="effect">Эффект для возврата</param>
        /// <param name="effectType">Тип эффекта</param>
        public void ReturnEffectToPool(GameObject effect, string effectType)
        {
            if (effect == null || !effectPools.ContainsKey(effectType)) return;
            
            effect.SetActive(false);
            effect.transform.SetParent(transform);
            effectPools[effectType].Enqueue(effect);
            
            if (enableDebugLogs)
            {
                Debug.Log($"ParticleEffectsManager: Эффект {effectType} возвращен в пул");
            }
        }
        
        #endregion
        
        #region Event Handlers
        
        private void OnPlayerDashStateChanged(bool isDashing)
        {
            if (isDashing)
            {
                PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
                if (playerMovement != null)
                {
                    Vector2 dashDirection = playerMovement.GetLastMovementDirection();
                    CreateDashEffect(playerMovement.transform.position, dashDirection);
                }
            }
        }
        
        private void OnPlayerHitEnemy(IDamageable enemy)
        {
            if (enemy is MonoBehaviour enemyMono)
            {
                CreateHitEffect(enemyMono.transform.position);
            }
        }
        
        private void OnEnemyDeath()
        {
            // Находим позицию врага
            EnemyHealth enemyHealth = FindFirstObjectByType<EnemyHealth>();
            if (enemyHealth != null)
            {
                CreateDeathEffect(enemyHealth.transform.position);
            }
        }
        
        private void OnEnemyDamageTaken(int damage)
        {
            // Эффект получения урона
            EnemyHealth enemyHealth = FindFirstObjectByType<EnemyHealth>();
            if (enemyHealth != null)
            {
                CreateEffect("Damage", enemyHealth.transform.position, default, 0.5f);
            }
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// Очищает все активные эффекты
        /// </summary>
        public void ClearAllEffects()
        {
            ParticleSystem[] particleSystems = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
            foreach (var ps in particleSystems)
            {
                if (ps.transform.parent == transform)
                {
                    ps.Stop();
                    ps.gameObject.SetActive(false);
                }
            }
            
            if (enableDebugLogs)
            {
                Debug.Log("ParticleEffectsManager: Все эффекты очищены");
            }
        }
        
        /// <summary>
        /// Получает статистику пула
        /// </summary>
        /// <returns>Строка со статистикой</returns>
        public string GetPoolStatistics()
        {
            string stats = "Particle Effects Pool Statistics:\n";
            
            foreach (var pool in effectPools)
            {
                stats += $"{pool.Key}: {pool.Value.Count} objects\n";
            }
            
            return stats;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Контроллер отдельного эффекта частиц
    /// </summary>
    public class ParticleEffectController : MonoBehaviour
    {
        [Header("Настройки эффекта")]
        [SerializeField] private float autoDestroyDelay = 2f;
        [SerializeField] private bool useAutoDestroy = true;
        
        private string effectType;
        private ParticleEffectsManager manager;
        private ParticleSystem particleEffect;
        private Coroutine autoDestroyCoroutine;
        
        public void Initialize(string effectType, ParticleEffectsManager manager)
        {
            this.effectType = effectType;
            this.manager = manager;
            particleEffect = GetComponent<ParticleSystem>();
        }
        
        public void PlayEffect(float duration = -1f)
        {
            if (particleEffect != null)
            {
                particleEffect.Play();
            }
            
            if (useAutoDestroy)
            {
                float destroyDelay = duration > 0 ? duration : autoDestroyDelay;
                autoDestroyCoroutine = StartCoroutine(AutoDestroyCoroutine(destroyDelay));
            }
        }
        
        private IEnumerator AutoDestroyCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (manager != null)
            {
                manager.ReturnEffectToPool(gameObject, effectType);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnDestroy()
        {
            if (autoDestroyCoroutine != null)
            {
                StopCoroutine(autoDestroyCoroutine);
            }
        }
    }
} 