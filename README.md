# Overlord Rise and Slice 2

Современная 2D игра в жанре action-RPG с системой боя и исследования подземелий.

## 🎮 Особенности

- **Современная архитектура**: Использование новой Input System и модульной структуры
- **Универсальные системы**: Переиспользуемые компоненты для игрока и врагов
- **Боевая система**: Гибкая система атак с поддержкой различных типов урона
- **Система здоровья**: Универсальная система с поддержкой неуязвимости и эффектов
- **Искусственный интеллект**: Умные враги с патрулированием и преследованием
- **Визуальные эффекты**: Анимации, частицы и эффекты повреждений

## 📁 Структура проекта

### Scripts/
```
Scripts/
├── Combat/                    # Боевые системы
│   ├── IDamageable.cs        # Интерфейс для объектов с уроном
│   ├── UniversalHealthSystem.cs  # Универсальная система здоровья
│   ├── UniversalAttackSystem.cs  # Универсальная система атаки
│   ├── PlayerAttackSystem.cs     # Специализированная атака игрока
│   ├── EnemyAttackSystem.cs      # Специализированная атака врагов
│   └── PlayerCombat.cs           # Координатор боя игрока
├── Player/                   # Системы игрока
│   ├── PlayerMovement.cs     # Движение игрока
│   ├── PlayerInputController.cs  # Контроллер ввода
│   └── PlayerHealthSystem.cs     # Здоровье игрока
├── Enemies/                  # Системы врагов
│   ├── EnemyController.cs    # Основной контроллер врага
│   └── EnemyAI.cs           # Искусственный интеллект
├── UI/                      # Пользовательский интерфейс
│   └── EnemyHealthBar.cs    # Полоска здоровья врагов
├── Animation/               # Анимации
│   └── PlayerAnimationController.cs
├── Effects/                 # Визуальные эффекты
│   ├── CameraEffects.cs     # Эффекты камеры
│   └── ParticleEffectsManager.cs
├── InteractiveObjects/      # Интерактивные объекты
│   ├── BaseInteractiveObject.cs
│   └── AncientStatue.cs
├── Progression/            # Система прогрессии
│   └── PlayerProgression.cs
└── Core/                   # Основные системы
    ├── CameraFollow.cs      # Следование камеры
    ├── LevelGenerator.cs    # Генератор уровней
    ├── DungeonHierarchy.cs  # Иерархия подземелий
    ├── LevelTier.cs        # Уровни сложности
    └── TileDatabase.cs     # База данных тайлов
```

## 🛠️ Настройка

### Требования
- Unity 2022.3 LTS или новее
- Input System Package
- 2D Animation Package

### Установка
1. Клонируйте репозиторий
2. Откройте проект в Unity
3. Убедитесь, что все пакеты установлены
4. Откройте сцену `Assets/Scenes/MainScene.unity`

## 🎯 Использование

### Настройка игрока
1. Создайте GameObject с тегом "Player"
2. Добавьте компоненты:
   - `PlayerMovement`
   - `PlayerInputController`
   - `PlayerCombat`
   - `PlayerAttackSystem`
   - `PlayerHealthSystem`
   - `Rigidbody2D`
   - `Collider2D`
   - `Animator`

### Настройка врага
1. Создайте GameObject
2. Добавьте компоненты:
   - `EnemyController`
   - `UniversalHealthSystem`
   - `EnemyAttackSystem`
   - `EnemyAI`
   - `Rigidbody2D`
   - `Collider2D`
   - `Animator`

### Настройка Input Actions
1. Откройте `Assets/InputActions.inputactions`
2. Настройте действия:
   - `Move` - движение (Vector2)
   - `Attack` - атака (Button)
   - `Dash` - рывок (Button)

## 🔧 Архитектура

### Универсальные системы
- **UniversalHealthSystem**: Базовая система здоровья с поддержкой IDamageable
- **UniversalAttackSystem**: Базовая система атак с проверкой попаданий

### Специализированные системы
- **PlayerAttackSystem**: Атака игрока с учетом движения
- **EnemyAttackSystem**: Атака врагов с преследованием игрока
- **PlayerHealthSystem**: Здоровье игрока с респавном

### Интерфейсы
- **IDamageable**: Интерфейс для объектов, получающих урон

## 🎨 Анимации

### Параметры аниматора игрока
- `isMoving` (bool) - движение
- `isAttacking` (bool) - атака
- `isDashing` (bool) - рывок
- `moveX`, `moveY` (float) - направление движения
- `lastMoveX`, `lastMoveY` (float) - последнее направление
- `Attack` (trigger) - атака
- `TakeDamage` (trigger) - получение урона
- `Die` (trigger) - смерть

### Параметры аниматора врага
- `isMoving` (bool) - движение
- `isAttacking` (bool) - атака
- `Attack` (trigger) - атака
- `TakeDamage` (trigger) - получение урона
- `Die` (trigger) - смерть

## 🐛 Отладка

Включите отладочные логи в компонентах:
- `enableDebugLogs = true`
- `showAttackGizmos = true`
- `showMovementGizmos = true`

## 📝 Лицензия

MIT License - см. файл LICENSE для деталей.

## 🤝 Вклад в проект

1. Форкните репозиторий
2. Создайте ветку для новой функции
3. Внесите изменения
4. Создайте Pull Request

## 📞 Поддержка

Если у вас есть вопросы или проблемы, создайте Issue в репозитории.