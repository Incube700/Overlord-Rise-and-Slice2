using UnityEngine;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Быстрая настройка визуализации атаки
    /// </summary>
    public class QuickAttackVisualSetup : MonoBehaviour
    {
        [Header("Автоматическая настройка")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private bool createParticleSystems = true;
        [SerializeField] private bool createAudioSource = true;
        
        [Header("Настройки частиц")]
        [SerializeField] private Color attackParticleColor = Color.red;
        [SerializeField] private Color hitParticleColor = Color.yellow;
        
        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupAttackVisuals();
            }
        }
        
        /// <summary>
        /// Настраивает визуальные эффекты атаки
        /// </summary>
        [ContextMenu("Setup Attack Visuals")]
        public void SetupAttackVisuals()
        {
            Debug.Log("QuickAttackVisualSetup: Настройка визуальных эффектов атаки...");
            
            // Добавляем AttackVisualEffects
            AddAttackVisualEffects();
            
            // Создаем частицы
            if (createParticleSystems)
            {
                CreateAttackParticles();
                CreateHitParticles();
            }
            
            // Создаем AudioSource
            if (createAudioSource)
            {
                CreateAudioSource();
            }
            
            // Добавляем LineRenderer
            AddLineRenderer();
            
            Debug.Log("QuickAttackVisualSetup: Настройка завершена!");
        }
        
        /// <summary>
        /// Добавляет компонент AttackVisualEffects
        /// </summary>
        private void AddAttackVisualEffects()
        {
            if (GetComponent<AttackVisualEffects>() == null)
            {
                gameObject.AddComponent<AttackVisualEffects>();
                Debug.Log("QuickAttackVisualSetup: Добавлен AttackVisualEffects");
            }
        }
        
        /// <summary>
        /// Создает частицы атаки
        /// </summary>
        private void CreateAttackParticles()
        {
            GameObject attackParticles = new GameObject("Attack Particles");
            attackParticles.transform.SetParent(transform);
            attackParticles.transform.localPosition = Vector3.zero;
            
            ParticleSystem particles = attackParticles.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.duration = 0.5f;
            main.startLifetime = 0.3f;
            main.startSpeed = 5f;
            main.startSize = 0.1f;
            main.startColor = attackParticleColor;
            main.loop = false;
            
            var emission = particles.emission;
            emission.rateOverTime = 20;
            
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.5f;
            
            // Назначаем в AttackVisualEffects
            AttackVisualEffects visualEffects = GetComponent<AttackVisualEffects>();
            if (visualEffects != null)
            {
                // Используем reflection для установки приватного поля
                var field = typeof(AttackVisualEffects).GetField("attackParticles", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(visualEffects, particles);
                }
            }
            
            Debug.Log("QuickAttackVisualSetup: Созданы частицы атаки");
        }
        
        /// <summary>
        /// Создает частицы попадания
        /// </summary>
        private void CreateHitParticles()
        {
            GameObject hitParticles = new GameObject("Hit Particles");
            hitParticles.transform.SetParent(transform);
            hitParticles.transform.localPosition = Vector3.zero;
            
            ParticleSystem particles = hitParticles.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.duration = 0.3f;
            main.startLifetime = 0.2f;
            main.startSpeed = 3f;
            main.startSize = 0.05f;
            main.startColor = hitParticleColor;
            main.loop = false;
            
            var emission = particles.emission;
            emission.rateOverTime = 30;
            
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.2f;
            
            // Назначаем в AttackVisualEffects
            AttackVisualEffects visualEffects = GetComponent<AttackVisualEffects>();
            if (visualEffects != null)
            {
                var field = typeof(AttackVisualEffects).GetField("hitParticles", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(visualEffects, particles);
                }
            }
            
            Debug.Log("QuickAttackVisualSetup: Созданы частицы попадания");
        }
        
        /// <summary>
        /// Создает AudioSource
        /// </summary>
        private void CreateAudioSource()
        {
            if (GetComponent<AudioSource>() == null)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.volume = 0.5f;
                
                Debug.Log("QuickAttackVisualSetup: Добавлен AudioSource");
            }
        }
        
        /// <summary>
        /// Добавляет LineRenderer
        /// </summary>
        private void AddLineRenderer()
        {
            if (GetComponent<LineRenderer>() == null)
            {
                LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.enabled = false;
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.05f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.yellow;
                
                Debug.Log("QuickAttackVisualSetup: Добавлен LineRenderer");
            }
        }
        
        /// <summary>
        /// Очищает все визуальные эффекты
        /// </summary>
        [ContextMenu("Clear Attack Visuals")]
        public void ClearAttackVisuals()
        {
            // Удаляем AttackVisualEffects
            AttackVisualEffects visualEffects = GetComponent<AttackVisualEffects>();
            if (visualEffects != null)
            {
                DestroyImmediate(visualEffects);
            }
            
            // Удаляем частицы
            Transform attackParticles = transform.Find("Attack Particles");
            if (attackParticles != null)
            {
                DestroyImmediate(attackParticles.gameObject);
            }
            
            Transform hitParticles = transform.Find("Hit Particles");
            if (hitParticles != null)
            {
                DestroyImmediate(hitParticles.gameObject);
            }
            
            // Удаляем LineRenderer
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                DestroyImmediate(lineRenderer);
            }
            
            Debug.Log("QuickAttackVisualSetup: Визуальные эффекты очищены");
        }
    }
} 