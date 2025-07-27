using UnityEngine;
using System.Collections;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Система визуальных эффектов для атаки
    /// </summary>
    public class AttackVisualEffects : MonoBehaviour
    {
        [Header("Настройки анимации")]
        [SerializeField] private Animator animator;
        [SerializeField] private string attackTriggerName = "Attack";
        [SerializeField] private string isAttackingBoolName = "isAttacking";
        
        [Header("Настройки спрайта")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color attackColor = Color.red;
        [SerializeField] private float attackFlashDuration = 0.1f;
        [SerializeField] private int attackFlashCount = 3;
        
        [Header("Настройки частиц")]
        [SerializeField] private ParticleSystem attackParticles;
        [SerializeField] private ParticleSystem hitParticles;
        
        [Header("Настройки звука")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip attackSound;
        [SerializeField] private AudioClip hitSound;
        
        [Header("Настройки отладки")]
        [SerializeField] private bool enableDebugLogs = true;
        
        // Компоненты
        private SimpleAttackSystem attackSystem;
        private Color originalColor;
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void InitializeComponents()
        {
            if (animator == null)
                animator = GetComponent<Animator>();
                
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
                
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
                
            attackSystem = GetComponent<SimpleAttackSystem>();
            
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
            
            if (enableDebugLogs)
            {
                Debug.Log("AttackVisualEffects: Компоненты инициализированы");
            }
        }
        
        private void Start()
        {
            // Подписываемся на события атаки
            if (attackSystem != null)
            {
                attackSystem.OnAttackStarted += OnAttackStarted;
                attackSystem.OnAttackEnded += OnAttackEnded;
                attackSystem.OnEnemyHit += OnEnemyHit;
            }
        }
        
        private void OnDestroy()
        {
            // Отписываемся от событий
            if (attackSystem != null)
            {
                attackSystem.OnAttackStarted -= OnAttackStarted;
                attackSystem.OnAttackEnded -= OnAttackEnded;
                attackSystem.OnEnemyHit -= OnEnemyHit;
            }
        }
        
        /// <summary>
        /// Вызывается при начале атаки
        /// </summary>
        private void OnAttackStarted()
        {
            if (enableDebugLogs)
            {
                Debug.Log("AttackVisualEffects: Начало атаки");
            }
            
            // Запускаем анимацию
            StartAttackAnimation();
            
            // Запускаем эффект вспышки
            StartCoroutine(AttackFlashEffect());
            
            // Запускаем частицы атаки
            PlayAttackParticles();
            
            // Воспроизводим звук атаки
            PlayAttackSound();
        }
        
        /// <summary>
        /// Вызывается при завершении атаки
        /// </summary>
        private void OnAttackEnded()
        {
            if (enableDebugLogs)
            {
                Debug.Log("AttackVisualEffects: Завершение атаки");
            }
            
            // Останавливаем анимацию
            StopAttackAnimation();
        }
        
        /// <summary>
        /// Вызывается при попадании по врагу
        /// </summary>
        /// <param name="enemy">Поврежденный враг</param>
        private void OnEnemyHit(IDamageable enemy)
        {
            if (enableDebugLogs)
            {
                Debug.Log("AttackVisualEffects: Попадание по врагу!");
            }
            
            // Запускаем эффект попадания
            StartCoroutine(HitEffect());
            
            // Воспроизводим звук попадания
            PlayHitSound();
        }
        
        /// <summary>
        /// Запускает анимацию атаки
        /// </summary>
        private void StartAttackAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger(attackTriggerName);
                animator.SetBool(isAttackingBoolName, true);
            }
        }
        
        /// <summary>
        /// Останавливает анимацию атаки
        /// </summary>
        private void StopAttackAnimation()
        {
            if (animator != null)
            {
                animator.SetBool(isAttackingBoolName, false);
            }
        }
        
        /// <summary>
        /// Эффект вспышки при атаке
        /// </summary>
        private IEnumerator AttackFlashEffect()
        {
            if (spriteRenderer == null) yield break;
            
            for (int i = 0; i < attackFlashCount; i++)
            {
                spriteRenderer.color = attackColor;
                yield return new WaitForSeconds(attackFlashDuration);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(attackFlashDuration);
            }
        }
        
        /// <summary>
        /// Эффект при попадании
        /// </summary>
        private IEnumerator HitEffect()
        {
            if (spriteRenderer == null) yield break;
            
            // Кратковременная вспышка при попадании
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = originalColor;
        }
        
        /// <summary>
        /// Запускает частицы атаки
        /// </summary>
        private void PlayAttackParticles()
        {
            if (attackParticles != null)
            {
                attackParticles.Play();
            }
        }
        
        /// <summary>
        /// Воспроизводит звук атаки
        /// </summary>
        private void PlayAttackSound()
        {
            if (audioSource != null && attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }
        }
        
        /// <summary>
        /// Воспроизводит звук попадания
        /// </summary>
        private void PlayHitSound()
        {
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
        
        /// <summary>
        /// Устанавливает новый цвет атаки
        /// </summary>
        /// <param name="newColor">Новый цвет</param>
        public void SetAttackColor(Color newColor)
        {
            attackColor = newColor;
        }
        
        /// <summary>
        /// Устанавливает новую длительность вспышки
        /// </summary>
        /// <param name="newDuration">Новая длительность</param>
        public void SetFlashDuration(float newDuration)
        {
            attackFlashDuration = newDuration;
        }
        
        /// <summary>
        /// Устанавливает новое количество вспышек
        /// </summary>
        /// <param name="newCount">Новое количество</param>
        public void SetFlashCount(int newCount)
        {
            attackFlashCount = newCount;
        }
    }
} 