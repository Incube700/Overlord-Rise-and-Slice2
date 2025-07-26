using UnityEngine;
using System.Collections;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Древняя статуя - интерактивный объект, который восстанавливает силы игрока
    /// и предоставляет временные бонусы. Лор и эффекты зависят от яруса подземелья.
    /// </summary>
    public class AncientStatue : BaseInteractiveObject
    {
        [Header("Настройки статуи")]
        [SerializeField] private StatueType statueType = StatueType.OverlordMemory;
        [SerializeField] private bool glowsWhenNear = true;
        [SerializeField] private float glowIntensity = 2f;
        [SerializeField] private Color glowColor = Color.blue;

        [Header("Эффекты восстановления силы")]
        [SerializeField] private int healthRestoreAmount = 10;
        [SerializeField] private float powerBoostDuration = 30f;
        [SerializeField] private float powerBoostMultiplier = 1.2f;

        [Header("Анимация")]
        [SerializeField] private float activationAnimationDuration = 2f;
        [SerializeField] private AnimationCurve activationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // Компоненты для эффектов
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private bool isActivated = false;
        private Color originalColor;

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
            // Настраиваем спрайт для эффектов
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
                if (glowsWhenNear)
                {
                    spriteRenderer.color = Color.Lerp(originalColor, glowColor, 0.3f);
                }
            }

            // Получаем аниматор
            animator = GetComponent<Animator>();
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
            objectName = "Потрёпанная статуя";
            loreDescription = "Статуя павшего героя, некогда пытавшегося свергнуть Overlord. " +
                            "Его лицо искажено болью и отчаянием, а доспехи покрыты ржавчиной. " +
                            "Местные жители приносят к ней скромные дары, надеясь на защиту...";

            glowColor = Color.yellow;
            healthRestoreAmount = 8;
        }

        /// <summary>
        /// Лор для средних уровней (40-69)
        /// </summary>
        void SetupMidTierLore()
        {
            objectName = "Статуя древнего стража";
            loreDescription = "Могучий страж, созданный самим Overlord для защиты своих владений. " +
                            "Его каменные глаза следят за каждым движением, а аура власти всё ещё пульсирует вокруг него. " +
                            "Только достойные могут получить его благословение...";

            glowColor = Color.cyan;
            healthRestoreAmount = 12;
        }

        /// <summary>
        /// Лор для высоких уровней (20-39)
        /// </summary>
        void SetupHighTierLore()
        {
            objectName = "Статуя узурпатора";
            loreDescription = "Памятник тому, кто осмелился бросить вызов самому Overlord. " +
                            "Его поза выражает непокорность, а меч всё ещё занесён для удара. " +
                            "Аура бунта и неповиновения исходит от этого монумента...";

            glowColor = Color.magenta;
            healthRestoreAmount = 15;
        }

        /// <summary>
        /// Лор для самых верхних уровней (1-19)
        /// </summary>
        void SetupTopTierLore()
        {
            objectName = "Величественная статуя Overlord";
            loreDescription = "Сам Overlord, запечатлённый в камне в момент своего триумфа. " +
                            "Его взгляд пронзает время и пространство, а аура абсолютной власти окутывает всё вокруг. " +
                            "Только истинный наследник может прикоснуться к этой статуе...";

            glowColor = Color.red;
            healthRestoreAmount = 20;
        }

        /// <summary>
        /// Выполняет взаимодействие со статуей
        /// </summary>
        protected override void ExecuteInteraction()
        {
            if (isActivated) return;

            isActivated = true;
            StartCoroutine(ActivateStatue());
        }

        /// <summary>
        /// Корутина активации статуи
        /// </summary>
        IEnumerator ActivateStatue()
        {
            if (enableDebugLogs)
            {
                Debug.Log($"AncientStatue: Активация статуи {objectName}");
            }

            // Запускаем анимацию активации
            if (animator != null)
            {
                animator.SetTrigger("Activate");
            }

            // Эффект свечения
            if (spriteRenderer != null)
            {
                float elapsedTime = 0f;
                while (elapsedTime < activationAnimationDuration)
                {
                    float progress = elapsedTime / activationAnimationDuration;
                    float curveValue = activationCurve.Evaluate(progress);

                    spriteRenderer.color = Color.Lerp(originalColor, glowColor, curveValue);
                    spriteRenderer.transform.localScale = Vector3.one * (1f + curveValue * 0.2f);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Возвращаем к нормальному состоянию
                spriteRenderer.color = originalColor;
                spriteRenderer.transform.localScale = Vector3.one;
            }

            // Даём награды
            GiveStatueRewards();

            // Сбрасываем состояние
            isActivated = false;

            if (enableDebugLogs)
            {
                Debug.Log("AncientStatue: Активация завершена");
            }
        }

        /// <summary>
        /// Даёт награды от статуи
        /// </summary>
        void GiveStatueRewards()
        {
            // Восстанавливаем здоровье
            RestorePlayerHealth();

            // Даём временный бонус силы
            GivePowerBoost();

            // Даём награду в зависимости от яруса
            GiveTierBasedReward();

            if (enableDebugLogs)
            {
                Debug.Log($"AncientStatue: Выданы награды от статуи {objectName}");
            }
        }

        /// <summary>
        /// Восстанавливает здоровье игрока
        /// </summary>
        void RestorePlayerHealth()
        {
            // Здесь можно добавить логику восстановления здоровья игрока
            // Например, найти PlayerHealth компонент и вызвать RestoreHealth

            if (enableDebugLogs)
            {
                Debug.Log($"AncientStatue: Восстановлено {healthRestoreAmount} здоровья");
            }
        }

        /// <summary>
        /// Даёт временный бонус силы
        /// </summary>
        void GivePowerBoost()
        {
            // Здесь можно добавить логику временного бонуса
            // Например, найти PlayerMovement и увеличить скорость

            if (enableDebugLogs)
            {
                Debug.Log($"AncientStatue: Дан бонус силы на {powerBoostDuration} секунд");
            }

            StartCoroutine(PowerBoostCoroutine());
        }

        /// <summary>
        /// Корутина временного бонуса силы
        /// </summary>
        IEnumerator PowerBoostCoroutine()
        {
            // Здесь можно добавить логику временного бонуса
            // Например, увеличить скорость движения игрока

            yield return new WaitForSeconds(powerBoostDuration);

            if (enableDebugLogs)
            {
                Debug.Log("AncientStatue: Бонус силы закончился");
            }
        }

        /// <summary>
        /// Показывает подсказку для взаимодействия
        /// </summary>
        protected override void ShowInteractionPrompt()
        {
            if (isActivated) return;

            base.ShowInteractionPrompt();

            // Добавляем эффект пульсации при приближении
            if (glowsWhenNear && spriteRenderer != null)
            {
                StartCoroutine(PulseGlow());
            }
        }

        /// <summary>
        /// Корутина пульсации свечения
        /// </summary>
        IEnumerator PulseGlow()
        {
            while (isPlayerNearby && !isActivated)
            {
                float pulse = Mathf.Sin(Time.time * 2f) * 0.3f + 0.7f;
                spriteRenderer.color = Color.Lerp(originalColor, glowColor, pulse * 0.5f);

                yield return null;
            }

            // Возвращаем к нормальному состоянию
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }

        /// <summary>
        /// Получает описание типа статуи
        /// </summary>
        public string GetStatueTypeDescription()
        {
            switch (statueType)
            {
                case StatueType.OverlordMemory:
                    return "Воспоминание о великом Overlord";
                case StatueType.FallenHero:
                    return "Павший герой, пытавшийся свергнуть тирана";
                case StatueType.AncientGuardian:
                    return "Древний страж, созданный Overlord";
                case StatueType.Usurper:
                    return "Узурпатор, бросивший вызов власти";
                default:
                    return "Неизвестная статуя";
            }
        }
    }

    /// <summary>
    /// Типы статуй
    /// </summary>
    public enum StatueType
    {
        OverlordMemory,    // Воспоминание об Overlord
        FallenHero,        // Павший герой
        AncientGuardian,   // Древний страж
        Usurper            // Узурпатор
    }
} 