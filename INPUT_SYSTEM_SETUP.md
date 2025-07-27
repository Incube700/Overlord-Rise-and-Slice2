# Настройка Unity Input System

## 🎮 **Пошаговая инструкция по настройке новой системы управления**

### **Шаг 1: Подготовка проекта**

1. **Откройте Unity** и загрузите проект
2. **Убедитесь, что Input System Package установлен**:
   - Window → Package Manager
   - Найдите "Input System"
   - Установите если не установлен

### **Шаг 2: Настройка Input Actions**

1. **Откройте InputActions.inputactions**:
   - В Project Window найдите `Assets/InputActions.inputactions`
   - Дважды кликните для открытия

2. **Сгенерируйте C# класс**:
   - В открывшемся окне нажмите **"Generate C# Class"**
   - Выберите папку `Assets/Scripts/`
   - Нажмите **"Apply"**

3. **Проверьте создание файла**:
   - В `Assets/Scripts/` должен появиться `InputActions.cs`

### **Шаг 3: Создание игрока**

1. **Создайте GameObject "Player"**:
   - Правый клик в Hierarchy → Create Empty
   - Переименуйте в "Player"
   - Установите Tag: "Player"

2. **Добавьте компоненты**:
   ```
   - PlayerInputController
   - PlayerMovement
   - PlayerCombat
   - PlayerInput (Unity компонент)
   - Rigidbody2D
   - BoxCollider2D
   - SpriteRenderer
   - Animator (если есть анимации)
   ```

3. **Настройте PlayerInput**:
   - Выберите Player в Hierarchy
   - В компоненте PlayerInput:
     - **Actions**: выберите `InputActions`
     - **Default Map**: выберите "Player"
     - **Behavior**: Send Messages

### **Шаг 4: Настройка Rigidbody2D**

1. **Выберите Player** в Hierarchy
2. **Настройте Rigidbody2D**:
   - **Body Type**: Dynamic
   - **Gravity Scale**: 0
   - **Freeze Rotation**: Z (поставьте галочку)
   - **Collision Detection**: Continuous

### **Шаг 5: Создание врага**

1. **Создайте GameObject "Enemy"**:
   - Правый клик в Hierarchy → Create Empty
   - Переименуйте в "Enemy"
   - Установите Tag: "Enemy"

2. **Добавьте компоненты**:
   ```
   - EnemyHealth (реализует IDamageable)
   - EnemyAI
   - Rigidbody2D
   - BoxCollider2D
   - SpriteRenderer
   - Animator (если есть анимации)
   ```

3. **Настройте EnemyHealth**:
   - **Max Health**: 30
   - **Invulnerability Duration**: 0.5
   - **Damage Color**: Red
   - **Death Delay**: 0.5

### **Шаг 6: Настройка камеры**

1. **Выберите Main Camera** в Hierarchy
2. **Добавьте скрипт CameraFollow** (если есть)
3. **Настройте камеру**:
   - **Projection**: Orthographic
   - **Size**: 5 (или по вашему усмотрению)

### **Шаг 7: Создание уровня**

1. **Создайте Grid**:
   - Правый клик в Hierarchy → 2D Object → Tilemap
   - Переименуйте в "Grid"

2. **Создайте Tilemap слои**:
   - **Floor** (для пола)
   - **Walls** (для стен)
   - **Decor** (для декораций)

3. **Настройте коллизии**:
   - Выберите Walls Tilemap
   - Добавьте TilemapCollider2D
   - Добавьте Rigidbody2D (Static)

### **Шаг 8: Тестирование**

1. **Нажмите Play** в Unity
2. **Проверьте управление**:
   - **WASD** — движение
   - **ЛКМ/Пробел** — атака
   - **Shift** — dash

3. **Проверьте бой**:
   - Подойдите к врагу
   - Атакуйте — должен получить урон
   - Враг должен преследовать игрока

## 🔧 **Устранение проблем**

### **Проблема: "InputActions not found"**
**Решение:**
1. Убедитесь, что InputActions.inputactions импортирован
2. Проверьте, что C# класс сгенерирован
3. Перезапустите Unity

### **Проблема: "PlayerInput component missing"**
**Решение:**
1. Добавьте PlayerInput компонент к игроку
2. Укажите InputActions в поле Actions
3. Выберите "Player" в Default Map

### **Проблема: "IDamageable not found"**
**Решение:**
1. Убедитесь, что EnemyHealth реализует IDamageable
2. Проверьте, что IDamageable.cs находится в проекте
3. Перекомпилируйте проект

### **Проблема: "PlayerMovement not found"**
**Решение:**
1. Убедитесь, что PlayerMovement.cs в папке Scripts/Player/
2. Проверьте, что нет ошибок компиляции
3. Перезапустите Unity

## 🎯 **Проверка работоспособности**

### **Логи в консоли должны показывать:**
```
PlayerInputController: Компоненты инициализированы
PlayerInputController: Input Actions настроены
PlayerMovement: Компоненты инициализированы
PlayerMovement: Rigidbody2D настроен
PlayerCombat: Компоненты инициализированы
EnemyHealth: Инициализировано здоровье 30/30
```

### **Тестовые действия:**
1. **Движение** — игрок должен двигаться плавно
2. **Атака** — должны быть логи атаки в консоли
3. **Dash** — игрок должен быстро перемещаться
4. **Урон врагу** — враг должен получать урон и мигать
5. **Смерть врага** — враг должен умереть и исчезнуть

## 🚀 **Готово!**

После выполнения всех шагов у вас должна быть полностью рабочая система управления и боя с современной архитектурой Unity Input System.

**🎮 Наслаждайтесь игрой!** ⚔️✨ 