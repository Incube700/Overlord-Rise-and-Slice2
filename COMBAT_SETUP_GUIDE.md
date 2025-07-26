# 🎮 Руководство по настройке боевой системы

## ✅ Что было реализовано

### 🔁 **PlayerMovement.cs** - Расширенная система движения:
- ✅ **Dash система** - Left Shift для уклонения
- ✅ **Настраиваемые параметры** - скорость, длительность, перезарядка
- ✅ **Неуязвимость** во время dash
- ✅ **Интеграция с Animator** - параметры для анимаций
- ✅ **События** для других систем (OnDashStateChanged, OnInvulnerabilityChanged)

### ⚔️ **PlayerMeleeAttack.cs** - Система ближней атаки:
- ✅ **Mouse0** для атаки с анимацией
- ✅ **isAttacking** проверка и кулдаун
- ✅ **Интеграция с движением** - направление атаки по движению
- ✅ **Визуальные эффекты** - вспышка при атаке
- ✅ **События атаки** для других систем

### 🎭 **Player_Animator.controller** - Система анимаций:
- ✅ **4 состояния**: Idle, Walk, Dash, Attack
- ✅ **Параметры**: isMoving, isDashing, isAttacking, Attack (trigger)
- ✅ **Переходы** между состояниями
- ✅ **Готов к подключению** анимационных клипов

### 🧪 **CombatTestScene.unity** - Тестовая сцена:
- ✅ **Игрок** с полной настройкой компонентов
- ✅ **Тестовый враг** с EnemyHealth и EnemyAI
- ✅ **Камера** с CameraFollow
- ✅ **Готово к тестированию**

---

## 🚀 Быстрая настройка в Unity

### 1. **Откройте проект в Unity 2022.3 LTS**

### 2. **Настройте слои (Layers)**
1. Откройте **Edit → Project Settings → Tags and Layers**
2. Добавьте новый слой: `Layer 8 = Enemy`
3. Назначьте врагам слой **Enemy**

### 3. **Настройте теги (Tags)**
1. Убедитесь что есть теги:
   - `Player` (для игрока)
   - `Enemy` (для врагов)
   - `Wall` (для стен)

### 4. **Откройте тестовую сцену**
1. Перейдите в `Assets/Scenes/CombatTestScene.unity`
2. Нажмите **Play**

---

## 🎮 Управление

### ⌨️ **Клавиши управления:**
- **WASD** / **Стрелки** - движение
- **Left Shift** - dash (уклонение)
- **Mouse0** - атака мечом

### 🎯 **Что должно работать:**
1. **Движение игрока** - плавное движение в 8 направлениях
2. **Dash** - быстрое перемещение с неуязвимостью
3. **Атака** - удар с визуальной вспышкой
4. **Преследование врага** - враг идёт к игроку
5. **Урон врагу** - враг получает урон и умирает
6. **Логи в консоли** - подробная информация о действиях

---

## 🔧 Настройка компонентов

### 👤 **Player GameObject:**

#### **PlayerMovement** настройки:
```
Move Speed: 5
Dash Speed: 15
Dash Duration: 0.2
Dash Cooldown: 1
Dash Makes Invulnerable: ✓
Enable Debug Logs: ✓
```

#### **PlayerMeleeAttack** настройки:
```
Attack Damage: 25
Attack Range: 1.2
Attack Cooldown: 0.4
Attack Duration: 0.3
Attack Offset: (1, 0)
Enemy Layer: Enemy (Layer 8)
Attack Color: Red
Flash Duration: 0.1
Enable Debug Logs: ✓
Show Attack Gizmos: ✓
```

#### **Rigidbody2D** настройки:
```
Body Type: Dynamic
Gravity Scale: 0
Freeze Rotation: Z ✓
```

#### **BoxCollider2D** настройки:
```
Is Trigger: ✗
Size: (0.13, 0.13)
```

#### **Animator** настройки:
```
Controller: Player_Animator
Apply Root Motion: ✗
```

### 👹 **Enemy GameObject:**

#### **EnemyHealth** настройки:
```
Max Health: 50
Invulnerability Duration: 0.2
Enable Debug Logs: ✓
```

#### **EnemyAI** настройки:
```
Detection Range: 5
Attack Range: 1.5
Move Speed: 2
Enable Debug Logs: ✓
Show Gizmos: ✓
```

---

## 🎭 Добавление анимаций

### 📁 **Структура анимаций:**
```
Assets/Animations/Player/
├── Player_Animator.controller ✅
├── Player_Idle.anim (создать)
├── Player_Walk.anim (создать)
├── Player_Dash.anim (создать)
└── Player_Attack.anim (создать)
```

### 🎬 **Создание анимационных клипов:**

1. **Выделите Player** в сцене
2. Откройте **Window → Animation → Animation**
3. Нажмите **Create** и создайте:
   - `Player_Idle.anim`
   - `Player_Walk.anim`
   - `Player_Dash.anim`
   - `Player_Attack.anim`

4. **Добавьте ключевые кадры** для каждой анимации:
   - Меняйте `Sprite Renderer → Sprite`
   - Добавляйте `Transform → Scale` для эффектов
   - Настройте `Loop Time` в инспекторе анимации

### 🔗 **Подключение к Animator Controller:**

1. Откройте **Player_Animator.controller**
2. **Назначьте анимации** состояниям:
   - **Idle State** → `Player_Idle.anim`
   - **Walk State** → `Player_Walk.anim`
   - **Dash State** → `Player_Dash.anim`
   - **Attack State** → `Player_Attack.anim`

---

## 🧪 Тестирование

### ✅ **Проверочный список:**

1. **Запустите сцену** - нет ошибок в консоли
2. **Движение WASD** - игрок перемещается плавно
3. **Dash (Shift)** - быстрое перемещение, логи в консоли
4. **Атака (Mouse0)** - красная вспышка, логи урона
5. **Враг преследует** - красный квадрат движется к игроку
6. **Урон врагу** - логи получения урона в консоли
7. **Смерть врага** - враг исчезает при 0 HP
8. **Гизмо атаки** - красный круг вокруг игрока в Scene View

### 🐛 **Возможные проблемы:**

#### **Dash не работает:**
- Проверьте **PlayerMovement** на игроке
- Убедитесь что **Rigidbody2D.freezeRotation = true**

#### **Атака не наносит урон:**
- Проверьте **Enemy Layer** в PlayerMeleeAttack
- Убедитесь что враг имеет **Layer 8 (Enemy)**
- Проверьте наличие **EnemyHealth** на враге

#### **Анимации не работают:**
- Проверьте **Animator Controller** на игроке
- Убедитесь что **Apply Root Motion = false**

#### **Враг не преследует:**
- Проверьте **EnemyAI** на враге
- Убедитесь что игрок имеет **тег Player**

---

## 🎯 Следующие шаги

### 🚀 **Готово к расширению:**
1. **Добавить спрайты** для анимаций
2. **Создать префабы** Player и Enemy
3. **Добавить звуковые эффекты**
4. **Интегрировать с системой душ**
5. **Добавить больше типов врагов**

### 📊 **Архитектура готова для:**
- Системы прокачки
- Различных видов оружия
- Способностей и заклинаний
- Системы здоровья игрока
- UI интерфейса

---

## 💡 Советы по разработке

### 🔧 **Отладка:**
- Включите **Debug Logs** во всех компонентах
- Используйте **Gizmos** для визуализации зон
- Проверяйте **консоль Unity** на ошибки

### 🎮 **Геймплей:**
- Настройте **Attack Range** для комфортного боя
- Подберите **Dash Cooldown** для баланса
- Отрегулируйте **Move Speed** врагов

### 🎨 **Визуалы:**
- Добавьте **Trail Renderer** для dash эффекта
- Используйте **Particle Systems** для атак
- Настройте **Sprite Sorting Layers**

---

**🎮 Готово к первому полноценному бою с уклонением, ударом и врагом!** 