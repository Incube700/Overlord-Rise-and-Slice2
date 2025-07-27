# 🎬 Руководство по анимациям и эффектам

## 🎯 **Обзор системы**

Новая архитектура анимаций и эффектов включает:
- ✅ **PlayerAnimationController** — управление анимациями игрока
- ✅ **CameraEffects** — визуальные эффекты камеры
- ✅ **ParticleEffectsManager** — система частиц
- ✅ **PlayerProgression** — система прогрессии и лора

## 🎮 **PlayerAnimationController**

### **Назначение:**
Центральный контроллер всех анимаций игрока с событийной архитектурой.

### **Компоненты:**
```
- Animator (Unity компонент)
- PlayerMovement (движение)
- PlayerCombat (бой)
- SpriteRenderer (визуальные эффекты)
```

### **События:**
```csharp
OnAttackAnimationStarted    // Начало анимации атаки
OnAttackAnimationEnded      // Конец анимации атаки
OnDashAnimationStarted      // Начало анимации dash
OnDashAnimationEnded        // Конец анимации dash
OnDamageAnimationStarted    // Начало анимации получения урона
```

### **Настройка в Unity:**
1. **Добавьте компонент** к игроку
2. **Настройте Animator** с параметрами:
   - `isMoving` (Bool)
   - `isDashing` (Bool)
   - `isAttacking` (Bool)
   - `Attack` (Trigger)
   - `moveX`, `moveY` (Float)
   - `lastMoveX`, `lastMoveY` (Float)

### **Визуальные эффекты:**
- **Dash эффект** — изменение цвета на cyan
- **Неуязвимость** — мигание белым
- **Получение урона** — вспышка красным

## 🎥 **CameraEffects**

### **Назначение:**
Система визуальных эффектов камеры для улучшения геймплея.

### **Эффекты:**
- **Screen Shake** — тряска экрана при попаданиях
- **Camera Zoom** — зум при dash и попаданиях
- **Hit Effects** — комбинированные эффекты

### **Настройка:**
```csharp
// Добавьте к Main Camera
CameraEffects cameraEffects = camera.AddComponent<CameraEffects>();

// Настройте параметры
cameraEffects.SetShakeIntensity(0.1f);
cameraEffects.SetShakeDuration(0.2f);
cameraEffects.SetZoomLimits(3f, 8f);
```

### **Автоматические эффекты:**
- **Dash** — зум в 1.2x на 0.3 секунды
- **Попадание** — тряска + зум в 0.9x
- **Смерть врага** — сильная тряска

## ✨ **ParticleEffectsManager**

### **Назначение:**
Синглтон для управления всеми эффектами частиц с пулом объектов.

### **Типы эффектов:**
- **Hit** — эффект попадания
- **Dash** — эффект уклонения
- **Death** — эффект смерти
- **Damage** — эффект получения урона
- **Heal** — эффект лечения

### **Настройка:**
1. **Создайте GameObject** "ParticleEffectsManager"
2. **Добавьте компонент** ParticleEffectsManager
3. **Назначьте префабы** эффектов в инспекторе
4. **Настройте пул** (размер, расширение)

### **Использование:**
```csharp
// Создание эффекта
ParticleEffectsManager.Instance.CreateEffect("Hit", position);

// Создание эффекта с поворотом
ParticleEffectsManager.Instance.CreateDashEffect(position, direction);

// Создание эффекта смерти
ParticleEffectsManager.Instance.CreateDeathEffect(position);
```

### **Пул объектов:**
- **Начальный размер:** 10 объектов каждого типа
- **Максимальный размер:** 50 объектов
- **Автоматическое расширение:** да
- **Автоматическое уничтожение:** да

## 📈 **PlayerProgression**

### **Назначение:**
Система прогрессии игрока с душами, уровнями и лором.

### **Компоненты системы:**
- **Души** — валюта для прокачки
- **Уровни** — прогрессия игрока (1-50)
- **Способности** — разблокируемые умения
- **Лор** — история и атмосфера

### **Настройка:**
1. **Создайте GameObject** "PlayerProgression"
2. **Добавьте компонент** PlayerProgression
3. **Настройте способности** в инспекторе

### **Система душ:**
```csharp
// Получение душ
PlayerProgression.Instance.AddSouls(amount);

// Трата душ
PlayerProgression.Instance.SpendSouls(amount);

// Проверка баланса
int souls = PlayerProgression.Instance.GetSoulsCollected();
```

### **Система способностей:**
```csharp
// Разблокировка способности
PlayerProgression.Instance.UnlockAbility("Soul Sense");

// Проверка доступности
bool unlocked = PlayerProgression.Instance.IsAbilityUnlocked("Dash");
```

### **Система лора:**
```csharp
// Добавление лора
PlayerProgression.Instance.DiscoverLore("Новый лор...");

// Изменение уровня подземелья
PlayerProgression.Instance.SetDungeonLevel(5);
```

## 🎯 **Интеграция систем**

### **Автоматические связи:**
1. **PlayerMovement** → **PlayerAnimationController** → анимации движения
2. **PlayerCombat** → **PlayerAnimationController** → анимации атаки
3. **PlayerMovement** → **CameraEffects** → эффекты dash
4. **PlayerCombat** → **ParticleEffectsManager** → эффекты попадания
5. **EnemyHealth** → **PlayerProgression** → получение душ

### **События:**
```csharp
// Анимации
playerAnimationController.OnAttackAnimationStarted += OnAttackStarted;
playerAnimationController.OnDashAnimationStarted += OnDashStarted;

// Эффекты камеры
cameraEffects.OnScreenShakeStarted += OnShakeStarted;
cameraEffects.OnCameraZoomStarted += OnZoomStarted;

// Прогрессия
playerProgression.OnSoulsChanged += OnSoulsChanged;
playerProgression.OnLevelUp += OnLevelUp;
playerProgression.OnLoreDiscovered += OnLoreDiscovered;
```

## 🔧 **Настройка в Unity**

### **Шаг 1: Настройка игрока**
1. **Выберите Player** в Hierarchy
2. **Добавьте компоненты:**
   ```
   - PlayerAnimationController
   - PlayerInputController
   - PlayerMovement
   - PlayerCombat
   - PlayerInput
   - Rigidbody2D
   - BoxCollider2D
   - SpriteRenderer
   - Animator
   ```

### **Шаг 2: Настройка камеры**
1. **Выберите Main Camera**
2. **Добавьте компонент** CameraEffects
3. **Настройте параметры** в инспекторе

### **Шаг 3: Настройка эффектов**
1. **Создайте GameObject** "ParticleEffectsManager"
2. **Добавьте компонент** ParticleEffectsManager
3. **Создайте префабы** эффектов частиц
4. **Назначьте префабы** в инспекторе

### **Шаг 4: Настройка прогрессии**
1. **Создайте GameObject** "PlayerProgression"
2. **Добавьте компонент** PlayerProgression
3. **Настройте способности** и лор

## 🎨 **Создание эффектов частиц**

### **Hit Effect (Эффект попадания):**
```
Particle System Settings:
- Duration: 0.5
- Start Lifetime: 0.3
- Start Speed: 3
- Start Size: 0.1
- Shape: Circle
- Emission Rate: 20
- Color: Red/Orange gradient
```

### **Dash Effect (Эффект уклонения):**
```
Particle System Settings:
- Duration: 0.3
- Start Lifetime: 0.2
- Start Speed: 5
- Start Size: 0.05
- Shape: Circle
- Emission Rate: 50
- Color: Cyan/Blue gradient
```

### **Death Effect (Эффект смерти):**
```
Particle System Settings:
- Duration: 2.0
- Start Lifetime: 1.0
- Start Speed: 2
- Start Size: 0.2
- Shape: Sphere
- Emission Rate: 100
- Color: Dark red/Black gradient
```

## 🚀 **Тестирование**

### **Тест анимаций:**
1. **Движение** — проверьте плавные переходы
2. **Атака** — проверьте триггер анимации
3. **Dash** — проверьте эффект и анимацию
4. **Получение урона** — проверьте мигание

### **Тест эффектов камеры:**
1. **Screen Shake** — атакуйте врага
2. **Camera Zoom** — используйте dash
3. **Hit Effects** — проверьте комбинированные эффекты

### **Тест частиц:**
1. **Hit Effect** — атакуйте врага
2. **Dash Effect** — используйте dash
3. **Death Effect** — убейте врага
4. **Пул объектов** — проверьте переиспользование

### **Тест прогрессии:**
1. **Получение душ** — убейте врага
2. **Повышение уровня** — наберите достаточно душ
3. **Разблокировка способностей** — достигните нужного уровня
4. **Открытие лора** — измените уровень подземелья

## 📊 **Производительность**

### **Оптимизация анимаций:**
- ✅ **События** вместо прямых вызовов
- ✅ **Пул объектов** для частиц
- ✅ **Автоматическое уничтожение** эффектов
- ✅ **Ограничение количества** активных эффектов

### **Мониторинг:**
```csharp
// Статистика пула частиц
string stats = ParticleEffectsManager.Instance.GetPoolStatistics();

// Текущее состояние анимации
string state = playerAnimationController.GetCurrentAnimationState();

// Статистика прогрессии
int level = PlayerProgression.Instance.GetCurrentLevel();
int souls = PlayerProgression.Instance.GetSoulsCollected();
```

## 🎯 **Готово к использованию!**

Система анимаций и эффектов полностью интегрирована и готова к использованию:

- ✅ **Модульная архитектура** с событиями
- ✅ **Автоматическая интеграция** с существующими системами
- ✅ **Оптимизированная производительность** с пулом объектов
- ✅ **Расширяемая система** для новых эффектов
- ✅ **Полная документация** и примеры

**🎬 Наслаждайтесь красивыми анимациями и эффектами!** ✨🎮 