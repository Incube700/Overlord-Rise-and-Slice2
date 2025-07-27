using UnityEngine;
using UnityEngine.Events;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Базовый класс для интерактивных объектов в подземелье
    /// Обеспечивает основную функциональность взаимодействия и лора
    /// </summary>
    public abstract class BaseInteractiveObject : MonoBehaviour
    {
        [Header("Базовые настройки")]
        public string objectName = "Интерактивный объект";
        public InteractiveType objectType = InteractiveType.Statue;
        
        [Header("Лор и описание")]
        [TextArea(3, 6)]
        public string loreDescription = "Древний объект, хранящий секреты прошлого...";
        [TextArea(2, 4)]
        public string interactionPrompt = "Нажмите E для взаимодействия";
        
        [Header("Настройки взаимодействия")]
        public float interactionRange = 2f;
        public bool canInteractMultipleTimes = true;
        public float cooldownTime = 1f;
        
        [Header("Визуальные эффекты")]
        public GameObject highlightEffect;
        public AudioClip interactionSound;
        public ParticleSystem interactionParticles;
        
        [Header("События")]
        public UnityEvent OnPlayerEnterRange;
        public UnityEvent OnPlayerExitRange;
        public UnityEvent OnInteraction;
        
        // Состояние объекта
        protected bool playerInRange = false;
        protected bool hasBeenInteracted = false;
        protected float lastInteractionTime = 0f;
        
        // Компоненты
        protected DungeonHierarchy dungeonHierarchy;
        protected AudioSource audioSource;
        protected SpriteRenderer spriteRenderer;
        
        // UI элементы
        protected GameObject promptUI;
        
        protected virtual void Start()
        {
            InitializeComponents();
            SetupVisuals();
        }
        
        protected virtual void Update()
        {
            CheckPlayerProximity();
            HandleInput();
        }
        
        /// <summary>
        /// Инициализация компонентов
        /// </summary>
        protected virtual void InitializeComponents()
        {
            dungeonHierarchy = FindFirstObjectByType<DungeonHierarchy>();
            audioSource = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }
        
        /// <summary>
        /// Настройка визуальных элементов
        /// </summary>
        protected virtual void SetupVisuals()
        {
            // Создаём UI подсказку
            CreatePromptUI();
            
            // Настраиваем эффект подсветки
            if (highlightEffect != null)
            {
                highlightEffect.SetActive(false);
            }
        }
        
        /// <summary>
        /// Проверка близости игрока
        /// </summary>
        protected virtual void CheckPlayerProximity()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            
            float distance = Vector3.Distance(transform.position, player.transform.position);
            bool inRange = distance <= interactionRange;
            
            if (inRange && !playerInRange)
            {
                OnPlayerEnterRange?.Invoke();
                ShowInteractionPrompt();
                playerInRange = true;
            }
            else if (!inRange && playerInRange)
            {
                OnPlayerExitRange?.Invoke();
                HideInteractionPrompt();
                playerInRange = false;
            }
        }
        
        /// <summary>
        /// Обработка ввода игрока
        /// </summary>
        protected virtual void HandleInput()
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                if (CanInteract())
                {
                    Interact();
                }
            }
        }
        
        /// <summary>
        /// Проверка возможности взаимодействия
        /// </summary>
        protected virtual bool CanInteract()
        {
            if (!canInteractMultipleTimes && hasBeenInteracted)
                return false;
                
            if (Time.time - lastInteractionTime < cooldownTime)
                return false;
                
            return true;
        }
        
        /// <summary>
        /// Основная логика взаимодействия
        /// </summary>
        public virtual void Interact()
        {
            lastInteractionTime = Time.time;
            hasBeenInteracted = true;
            
            // Воспроизводим звук
            PlayInteractionSound();
            
            // Запускаем частицы
            PlayInteractionParticles();
            
            // Вызываем событие
            OnInteraction?.Invoke();
            
            // Выполняем специфичную логику
            ExecuteInteraction();
            
            // Показываем лор
            ShowLoreText();
            
            Debug.Log($"[{objectName}] Взаимодействие выполнено игроком");
        }
        
        /// <summary>
        /// Специфичная логика взаимодействия (переопределяется в наследниках)
        /// </summary>
        protected abstract void ExecuteInteraction();
        
        /// <summary>
        /// Показывает текст лора
        /// </summary>
        protected virtual void ShowLoreText()
        {
            if (string.IsNullOrEmpty(loreDescription)) return;
            
            // Здесь можно добавить UI для отображения лора
            Debug.Log($"[Лор] {objectName}: {loreDescription}");
            
            // TODO: Интеграция с системой UI для показа лора
        }
        
        /// <summary>
        /// Воспроизводит звук взаимодействия
        /// </summary>
        protected virtual void PlayInteractionSound()
        {
            if (audioSource != null && interactionSound != null)
            {
                audioSource.PlayOneShot(interactionSound);
            }
        }
        
        /// <summary>
        /// Запускает частицы взаимодействия
        /// </summary>
        protected virtual void PlayInteractionParticles()
        {
            if (interactionParticles != null)
            {
                interactionParticles.Play();
            }
        }
        
        /// <summary>
        /// Показывает подсказку для взаимодействия
        /// </summary>
        protected virtual void ShowInteractionPrompt()
        {
            if (highlightEffect != null)
            {
                highlightEffect.SetActive(true);
            }
            
            if (promptUI != null)
            {
                promptUI.SetActive(true);
            }
        }
        
        /// <summary>
        /// Скрывает подсказку для взаимодействия
        /// </summary>
        protected virtual void HideInteractionPrompt()
        {
            if (highlightEffect != null)
            {
                highlightEffect.SetActive(false);
            }
            
            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }
        }
        
        /// <summary>
        /// Создаёт UI подсказку
        /// </summary>
        protected virtual void CreatePromptUI()
        {
            // Простая реализация через 3D текст
            GameObject promptObject = new GameObject("InteractionPrompt");
            promptObject.transform.SetParent(transform);
            promptObject.transform.localPosition = Vector3.up * 1.5f;
            
            TextMesh textMesh = promptObject.AddComponent<TextMesh>();
            textMesh.text = interactionPrompt;
            textMesh.fontSize = 20;
            textMesh.color = Color.white;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            
            promptUI = promptObject;
            promptUI.SetActive(false);
        }
        
        /// <summary>
        /// Получает награду в зависимости от текущего яруса
        /// </summary>
        protected virtual void GiveTierBasedReward()
        {
            if (dungeonHierarchy == null) return;
            
            var currentTier = dungeonHierarchy.CurrentTier;
            if (currentTier == null) return;
            
            // Даём монеты в зависимости от яруса
            int coinReward = Random.Range(currentTier.minCoins, currentTier.maxCoins + 1);
            Debug.Log($"[{objectName}] Получено монет: {coinReward}");
            
            // Шанс дропа предмета
            if (Random.value < currentTier.lootDropChance)
            {
                var loot = currentTier.lootTable?.GetRandomLoot();
                if (loot != null)
                {
                    Instantiate(loot, transform.position + Vector3.up, Quaternion.identity);
                    Debug.Log($"[{objectName}] Дроп предмета: {loot.name}");
                }
            }
        }
        
        /// <summary>
        /// Получает описание с учётом текущего яруса
        /// </summary>
        protected virtual string GetContextualDescription()
        {
            if (dungeonHierarchy == null) return loreDescription;
            
            string tierContext = dungeonHierarchy.GetAtmosphericDescription();
            return $"{loreDescription}\n\n{tierContext}";
        }
        
        // Визуализация зоны взаимодействия в редакторе
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
        
        // Публичные свойства
        public bool PlayerInRange => playerInRange;
        public bool HasBeenInteracted => hasBeenInteracted;
        public InteractiveType ObjectType => objectType;
    }
} 