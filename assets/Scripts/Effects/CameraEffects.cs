using UnityEngine;
using System.Collections;

namespace OverlordRiseAndSlice
{
    /// <summary>
    /// Система визуальных эффектов камеры
    /// Screen shake, zoom, и другие эффекты для улучшения геймплея
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraEffects : MonoBehaviour
    {
        [Header("Screen Shake")]
        [SerializeField] private float shakeIntensity = 0.1f;
        [SerializeField] private float shakeDuration = 0.2f;
        [SerializeField] private float shakeFrequency = 10f;
        
        [Header("Camera Zoom")]
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minZoom = 3f;
        [SerializeField] private float maxZoom = 8f;
        [SerializeField] private float defaultZoom = 5f;
        
        [Header("Hit Effects")]
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitEffectDuration = 0.1f;
        [SerializeField] private float hitEffectIntensity = 0.3f;
        
        [Header("Dash Effects")]
        [SerializeField] private float dashZoomMultiplier = 1.2f;
        [SerializeField] private float dashZoomDuration = 0.3f;
        
        [Header("Отладка")]
        [SerializeField] private bool enableDebugLogs = true;
        
        // Компоненты
        private Camera cameraComponent;
        private Vector3 originalPosition;
        private float originalOrthographicSize;
        
        // Состояние эффектов
        private bool isShaking = false;
        private bool isZooming = false;
        private Coroutine currentShakeCoroutine;
        private Coroutine currentZoomCoroutine;
        
        // События для других систем
        public System.Action OnScreenShakeStarted;
        public System.Action OnScreenShakeEnded;
        public System.Action OnCameraZoomStarted;
        public System.Action OnCameraZoomEnded;
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Start()
        {
            SubscribeToEvents();
            SetupCamera();
        }
        
        private void InitializeComponents()
        {
            cameraComponent = GetComponent<Camera>();
            
            if (cameraComponent == null)
            {
                Debug.LogError("CameraEffects: Отсутствует компонент Camera!");
                return;
            }
            
            originalPosition = transform.position;
            originalOrthographicSize = cameraComponent.orthographicSize;
            
            if (enableDebugLogs)
            {
                Debug.Log("CameraEffects: Компоненты инициализированы");
            }
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
        }
        
        private void SetupCamera()
        {
            if (cameraComponent == null) return;
            
            // Устанавливаем начальный зум
            cameraComponent.orthographicSize = defaultZoom;
            
            if (enableDebugLogs)
            {
                Debug.Log("CameraEffects: Камера настроена");
            }
        }
        
        #region Screen Shake
        
        /// <summary>
        /// Запускает эффект тряски экрана
        /// </summary>
        /// <param name="intensity">Интенсивность тряски</param>
        /// <param name="duration">Длительность тряски</param>
        public void ShakeScreen(float intensity = -1f, float duration = -1f)
        {
            if (isShaking)
            {
                StopShake();
            }
            
            float shakeIntensityValue = intensity > 0 ? intensity : this.shakeIntensity;
            float shakeDurationValue = duration > 0 ? duration : this.shakeDuration;
            
            currentShakeCoroutine = StartCoroutine(ShakeCoroutine(shakeIntensityValue, shakeDurationValue));
            
            OnScreenShakeStarted?.Invoke();
            
            if (enableDebugLogs)
            {
                Debug.Log($"CameraEffects: Начата тряска экрана (интенсивность: {shakeIntensityValue}, длительность: {shakeDurationValue})");
            }
        }
        
        private IEnumerator ShakeCoroutine(float intensity, float duration)
        {
            isShaking = true;
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                // Создаем случайное смещение
                Vector3 shakeOffset = new Vector3(
                    Random.Range(-intensity, intensity),
                    Random.Range(-intensity, intensity),
                    0f
                );
                
                transform.position = startPosition + shakeOffset;
                
                elapsedTime += Time.deltaTime;
                yield return new WaitForSeconds(1f / shakeFrequency);
            }
            
            // Возвращаем камеру в исходное положение
            transform.position = startPosition;
            isShaking = false;
            
            OnScreenShakeEnded?.Invoke();
            
            if (enableDebugLogs)
            {
                Debug.Log("CameraEffects: Тряска экрана завершена");
            }
        }
        
        /// <summary>
        /// Останавливает текущую тряску экрана
        /// </summary>
        public void StopShake()
        {
            if (currentShakeCoroutine != null)
            {
                StopCoroutine(currentShakeCoroutine);
                currentShakeCoroutine = null;
            }
            
            isShaking = false;
            transform.position = originalPosition;
            
            if (enableDebugLogs)
            {
                Debug.Log("CameraEffects: Тряска экрана остановлена");
            }
        }
        
        #endregion
        
        #region Camera Zoom
        
        /// <summary>
        /// Запускает эффект зума камеры
        /// </summary>
        /// <param name="targetZoom">Целевой зум</param>
        /// <param name="duration">Длительность зума</param>
        public void ZoomCamera(float targetZoom, float duration = -1f)
        {
            if (isZooming)
            {
                StopZoom();
            }
            
            float zoomDuration = duration > 0 ? duration : 0.5f;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            
            currentZoomCoroutine = StartCoroutine(ZoomCoroutine(targetZoom, zoomDuration));
            
            OnCameraZoomStarted?.Invoke();
            
            if (enableDebugLogs)
            {
                Debug.Log($"CameraEffects: Начат зум камеры (цель: {targetZoom}, длительность: {zoomDuration})");
            }
        }
        
        private IEnumerator ZoomCoroutine(float targetZoom, float duration)
        {
            isZooming = true;
            float startZoom = cameraComponent.orthographicSize;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                float progress = elapsedTime / duration;
                float currentZoom = Mathf.Lerp(startZoom, targetZoom, progress);
                
                cameraComponent.orthographicSize = currentZoom;
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            cameraComponent.orthographicSize = targetZoom;
            isZooming = false;
            
            OnCameraZoomEnded?.Invoke();
            
            if (enableDebugLogs)
            {
                Debug.Log("CameraEffects: Зум камеры завершен");
            }
        }
        
        /// <summary>
        /// Останавливает текущий зум камеры
        /// </summary>
        public void StopZoom()
        {
            if (currentZoomCoroutine != null)
            {
                StopCoroutine(currentZoomCoroutine);
                currentZoomCoroutine = null;
            }
            
            isZooming = false;
            
            if (enableDebugLogs)
            {
                Debug.Log("CameraEffects: Зум камеры остановлен");
            }
        }
        
        /// <summary>
        /// Возвращает камеру к исходному зуму
        /// </summary>
        public void ResetZoom()
        {
            ZoomCamera(originalOrthographicSize, 0.5f);
        }
        
        #endregion
        
        #region Hit Effects
        
        /// <summary>
        /// Запускает эффект попадания
        /// </summary>
        /// <param name="intensity">Интенсивность эффекта</param>
        public void TriggerHitEffect(float intensity = -1f)
        {
            float hitIntensity = intensity > 0 ? intensity : hitEffectIntensity;
            
            // Комбинируем тряску и зум для эффекта попадания
            ShakeScreen(hitIntensity, hitEffectDuration);
            ZoomCamera(cameraComponent.orthographicSize * 0.9f, hitEffectDuration);
            
            if (enableDebugLogs)
            {
                Debug.Log($"CameraEffects: Эффект попадания (интенсивность: {hitIntensity})");
            }
        }
        
        #endregion
        
        #region Event Handlers
        
        private void OnPlayerDashStateChanged(bool isDashing)
        {
            if (isDashing)
            {
                // Эффект зума при dash
                float dashZoom = cameraComponent.orthographicSize * dashZoomMultiplier;
                ZoomCamera(dashZoom, dashZoomDuration);
                
                if (enableDebugLogs)
                {
                    Debug.Log("CameraEffects: Dash эффект камеры");
                }
            }
            else
            {
                // Возвращаем к нормальному зуму
                ResetZoom();
            }
        }
        
        private void OnPlayerHitEnemy(IDamageable enemy)
        {
            // Эффект попадания
            TriggerHitEffect();
        }
        
        #endregion
        
        #region Публичные методы для других систем
        
        /// <summary>
        /// Проверяет, активна ли тряска экрана
        /// </summary>
        /// <returns>true если экран трясется</returns>
        public bool IsShaking()
        {
            return isShaking;
        }
        
        /// <summary>
        /// Проверяет, активен ли зум камеры
        /// </summary>
        /// <returns>true если камера зумится</returns>
        public bool IsZooming()
        {
            return isZooming;
        }
        
        /// <summary>
        /// Получает текущий зум камеры
        /// </summary>
        /// <returns>Текущий orthographic size</returns>
        public float GetCurrentZoom()
        {
            return cameraComponent != null ? cameraComponent.orthographicSize : defaultZoom;
        }
        
        /// <summary>
        /// Устанавливает интенсивность тряски
        /// </summary>
        /// <param name="intensity">Новая интенсивность</param>
        public void SetShakeIntensity(float intensity)
        {
            shakeIntensity = intensity;
        }
        
        /// <summary>
        /// Устанавливает длительность тряски
        /// </summary>
        /// <param name="duration">Новая длительность</param>
        public void SetShakeDuration(float duration)
        {
            shakeDuration = duration;
        }
        
        /// <summary>
        /// Устанавливает границы зума
        /// </summary>
        /// <param name="min">Минимальный зум</param>
        /// <param name="max">Максимальный зум</param>
        public void SetZoomLimits(float min, float max)
        {
            minZoom = min;
            maxZoom = max;
        }
        
        #endregion
        
        private void OnDestroy()
        {
            // Останавливаем все корутины
            if (currentShakeCoroutine != null)
            {
                StopCoroutine(currentShakeCoroutine);
            }
            
            if (currentZoomCoroutine != null)
            {
                StopCoroutine(currentZoomCoroutine);
            }
        }
    }
} 