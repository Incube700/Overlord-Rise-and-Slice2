using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Древняя статуя - интерактивный объект, рассказывающий историю уровня
    /// Даёт разные награды и лор в зависимости от яруса подземелья
    /// </summary>
    public class AncientStatue : BaseInteractiveObject
    {
        [Header("Настройки статуи")]
        public StatueType statueType = StatueType.OverlordMemory;
        public bool glowsWhenNear = true;
        public float glowIntensity = 2f;
        public Color glowColor = Color.blue;
        
        [Header("Эффекты восстановления силы")]
        public bool restoresPlayerPower = true;
        public int healthRestoreAmount = 10;
        public float powerBoostDuration = 30f;
        public float powerBoostMultiplier = 1.2f;
        
        [Header("Анимация")]
        public float activationAnimationDuration = 2f;
        public AnimationCurve activationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        // Компоненты для эффектов
        private Light2D statueLight;
        private Animator animator;
        private bool isActivated = false;
        
        protected override void Start()
        {
            base.Start();
            SetupStatueSpecifics();
            SetupTierBasedLore();
        }
        
        /// <summary>
        /// Настройка специфичных для статуи компонентов
        /// </summary>
        void SetupStatueSpecifics()
        {
            // Настраиваем освещение
            statueLight = GetComponentInChildren<Light2D>();
            if (statueLight == null && glowsWhenNear)
            {
                CreateStatueLight();
            }
            
            if (statueLight != null)
            {
                statueLight.color = glowColor;
                statueLight.intensity = 0f; // Начинаем с выключенного света
            }
            
            // Получаем аниматор
            animator = GetComponent<Animator>();
        }
        
        /// <summary>
        /// Создаёт источник света для статуи
        /// </summary>
        void CreateStatueLight()
        {
            GameObject lightObject = new GameObject("StatueLight");
            lightObject.transform.SetParent(transform);
            lightObject.transform.localPosition = Vector3.zero;
            
            statueLight = lightObject.AddComponent<Light2D>();
            statueLight.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Point;
            statueLight.color = glowColor;
            statueLight.intensity = 0f;
            statueLight.pointLightOuterRadius = 3f;
        }
        
        /// <summary>
        /// Настраивает лор в зависимости от текущего яруса
        /// </summary>
        void SetupTierBasedLore()
        {
            if (dungeonHierarchy == null) return;
            
            var currentTier = dungeonHierarchy.CurrentTier;
            if (currentTier == null) return;
            
            // Устанавливаем лор в зависимости от яруса
            switch (dungeonHierarchy.CurrentLevel)
            {
                case int level when level >= 90:
                    SetupBottomTierLore();
                    break;
                case int level when level >= 70:
                    SetupLowTierLore();
                    break;
                case int level when level >= 40:
                    SetupMidTierLore();
                    break;
                case int level when level >= 20:
                    SetupHighTierLore();
                    break;
                default:
                    SetupTopTierLore();
                    break;
            }
        }
        
        /// <summary>
        /// Лор для самых нижних уровней (90-100)
        /// </summary>
        void SetupBottomTierLore()
        {
            objectName = "Разбитая статуя";
            loreDescription = "Когда-то это была величественная статуя Overlord, но теперь от неё остались лишь обломки. " +
                            "Крысы свили гнёзда в трещинах мрамора, а плесень покрывает то, что когда-то было лицом власти. " +
                            "Но даже в таком состоянии она излучает слабый отблеск былого могущества...";
            
            glowColor = Color.gray;
            healthRestoreAmount = 5;
        }
        
        /// <summary>
        /// Лор для низких уровней (70-89)
        /// </summary>
        void SetupLowTierLore()
        {
            objectName = "Осквернённая статуя";
            loreDescription = "Мародёры исписали статуя граффити и украли все ценные детали, но основа всё ещё стоит. " +
                            "На постаменте видны следы крови — здесь явно делили добычу. " +
                            "Тем не менее, древняя магия всё ещё течёт через камень...";
            
            glowColor = Color.red;
            healthRestoreAmount = 8;
        }
        
        /// <summary>
        /// Лор для средних уровней (40-69)
        /// </summary>
        void SetupMidTierLore()
        {
            objectName = "Статуя наёмника";
            loreDescription = "Профессиональные наёмники превратили эту статую в своеобразный алтарь удачи. " +
                            "Они оставляют здесь монеты и оружие, прося благословения перед боем. " +
                            "Странно, но статуя словно отвечает на их просьбы...";
            
            glowColor = Color.blue;
            healthRestoreAmount = 12;
        }
        
        /// <summary>
        /// Лор для высоких уровней (20-39)
        /// </summary>
        void SetupHighTierLore()
        {
            objectName = "Статуя павшего героя";
            loreDescription = "Лорды войны установили здесь статую одного из героев, убивших Overlord. " +
                            "Но что-то не так... лицо героя искажено болью, а его поза больше напоминает мольбу о пощаде. " +
                            "Возможно, победа была не такой безоговорочной, как они думали...";
            
            glowColor = Color.yellow;
            healthRestoreAmount = 15;
        }
        
        /// <summary>
        /// Лор для верхних уровней (1-19)
        /// </summary>
        void SetupTopTierLore()
        {
            objectName = "Трон-статуя узурпатора";
            loreDescription = "На вершине замка узурпаторы установили золотую статую своего лидера на троне Overlord. " +
                            "Но золото потускнело, а лицо статуи начинает напоминать... самого Overlord. " +
                            "Кажется, даже их победные символы не могут избежать его влияния...";
            
            glowColor = Color.white;
            healthRestoreAmount = 20;
        }
        
        /// <summary>
        /// Основная логика взаимодействия со статуей
        /// </summary>
        protected override void ExecuteInteraction()
        {
            if (isActivated) return;
            
            StartCoroutine(ActivateStatue());
        }
        
        /// <summary>
        /// Анимация активации статуи
        /// </summary>
        IEnumerator ActivateStatue()
        {
            isActivated = true;
            
            // Запускаем анимацию активации
            if (animator != null)
            {
                animator.SetTrigger("Activate");
            }
            
            // Плавно включаем свет
            float elapsedTime = 0f;
            while (elapsedTime < activationAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / activationAnimationDuration;
                float curveValue = activationCurve.Evaluate(progress);
                
                if (statueLight != null)
                {
                    statueLight.intensity = curveValue * glowIntensity;
                }
                
                yield return null;
            }
            
            // Даём награды
            GiveStatueRewards();
            
            // Если это одноразовая статуя, отключаем возможность повторного взаимодействия
            if (!canInteractMultipleTimes)
            {
                canInteractMultipleTimes = false;
            }
        }
        
        /// <summary>
        /// Даёт награды от статуи
        /// </summary>
        void GiveStatueRewards()
        {
            // Восстанавливаем здоровье игрока
            RestorePlayerHealth();
            
            // Даём временный бафф силы
            if (restoresPlayerPower)
            {
                GivePowerBoost();
            }
            
            // Даём награды в зависимости от яруса
            GiveTierBasedReward();
            
            Debug.Log($"[{objectName}] Статуя активирована! Здоровье восстановлено на {healthRestoreAmount}");
        }
        
        /// <summary>
        /// Восстанавливает здоровье игрока
        /// </summary>
        void RestorePlayerHealth()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            
            // TODO: Интеграция с системой здоровья игрока
            Debug.Log($"[{objectName}] Игрок восстановил {healthRestoreAmount} здоровья");
        }
        
        /// <summary>
        /// Даёт временный бафф силы
        /// </summary>
        void GivePowerBoost()
        {
            StartCoroutine(PowerBoostCoroutine());
        }
        
        /// <summary>
        /// Корутина для временного баффа силы
        /// </summary>
        IEnumerator PowerBoostCoroutine()
        {
            Debug.Log($"[{objectName}] Получен бафф силы на {powerBoostDuration} секунд (x{powerBoostMultiplier})");
            
            // TODO: Интеграция с системой баффов игрока
            
            yield return new WaitForSeconds(powerBoostDuration);
            
            Debug.Log($"[{objectName}] Бафф силы закончился");
        }
        
        /// <summary>
        /// Показывает эффект свечения при входе игрока в зону
        /// </summary>
        protected override void ShowInteractionPrompt()
        {
            base.ShowInteractionPrompt();
            
            if (glowsWhenNear && statueLight != null && !isActivated)
            {
                StartCoroutine(PulseGlow());
            }
        }
        
        /// <summary>
        /// Эффект пульсирующего свечения
        /// </summary>
        IEnumerator PulseGlow()
        {
            while (playerInRange && !isActivated)
            {
                // Пульсация от 0.5 до 1.0 интенсивности
                float time = Time.time * 2f;
                float intensity = 0.5f + 0.5f * Mathf.Sin(time);
                
                if (statueLight != null)
                {
                    statueLight.intensity = intensity;
                }
                
                yield return null;
            }
            
            // Возвращаем к исходному состоянию
            if (statueLight != null && !isActivated)
            {
                statueLight.intensity = 0f;
            }
        }
        
        /// <summary>
        /// Получает специальное описание статуи с учётом её типа
        /// </summary>
        public string GetStatueTypeDescription()
        {
            switch (statueType)
            {
                case StatueType.OverlordMemory:
                    return "Эта статуя хранит воспоминания о былом величии Overlord...";
                case StatueType.FallenHero:
                    return "Памятник герою, который думал, что победил зло...";
                case StatueType.AncientGuardian:
                    return "Древний страж, всё ещё охраняющий секреты замка...";
                case StatueType.Usurper:
                    return "Символ новой власти, но власть эта оказалась иллюзией...";
                default:
                    return loreDescription;
            }
        }
    }
    
    /// <summary>
    /// Типы статуй в подземелье
    /// </summary>
    public enum StatueType
    {
        OverlordMemory,    // Воспоминание об Overlord
        FallenHero,        // Павший герой
        AncientGuardian,   // Древний страж
        Usurper            // Узурпатор
    }
} 