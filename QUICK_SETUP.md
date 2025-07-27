# Быстрая настройка проекта

## 🚀 Быстрый старт

### 1. Настройка игрока

1. **Создайте GameObject с тегом "Player"**
2. **Добавьте компоненты в следующем порядке:**
   ```
   - Rigidbody2D
   - Collider2D (CircleCollider2D или BoxCollider2D)
   - PlayerMovement
   - PlayerInputController
   - PlayerAttackSystem
   - PlayerCombat
   - PlayerHealthSystem
   - Animator
   ```

3. **Настройте Rigidbody2D:**
   - Gravity Scale: 0
   - Freeze Rotation: Z
   - Collision Detection: Continuous

4. **Настройте PlayerInput:**
   - Добавьте компонент PlayerInput
   - Укажите InputActions.inputactions
   - Установите Default Map: Player

### 2. Настройка врага

1. **Создайте GameObject**
2. **Добавьте компоненты:**
   ```
   - Rigidbody2D
   - Collider2D
   - UniversalHealthSystem
   - EnemyAttackSystem
   - EnemyAI
   - EnemyController
   - Animator
   ```

3. **Настройте EnemyController:**
   - Включите Auto Setup Components
   - Установите Enemy Name и Level

### 3. Настройка Input Actions

1. **Откройте Assets/InputActions.inputactions**
2. **Создайте действия:**
   - **Move** (Action Type: Value, Control Type: Vector2)
   - **Attack** (Action Type: Button)
   - **Dash** (Action Type: Button)
3. **Настройте привязки клавиш**
4. **Нажмите "Generate C# Class"**

### 4. Настройка анимаций

#### Для игрока:
```
Параметры:
- isMoving (bool)
- isAttacking (bool)
- isDashing (bool)
- moveX, moveY (float)
- lastMoveX, lastMoveY (float)

Триггеры:
- Attack
- TakeDamage
- Die
```

#### Для врага:
```
Параметры:
- isMoving (bool)
- isAttacking (bool)

Триггеры:
- Attack
- TakeDamage
- Die
```

## 🔧 Проверка работоспособности

### Тест движения
- Двигайте WASD/стрелки
- Игрок должен плавно двигаться
- Анимация движения должна работать

### Тест атаки
- Нажмите пробел/ЛКМ
- Игрок должен атаковать
- Анимация атаки должна работать

### Тест dash
- Нажмите Shift
- Игрок должен делать рывок
- Неуязвимость должна работать

### Тест врагов
- Враги должны преследовать игрока
- Атаки должны наносить урон
- Полоски здоровья должны отображаться

## 🐛 Отладка

### Включите отладочные логи:
```csharp
// В компонентах установите:
enableDebugLogs = true
showAttackGizmos = true
showMovementGizmos = true
```

### Проверьте консоль Unity:
- Должны появляться логи инициализации
- Логи движения и атак
- Предупреждения о недостающих компонентах

### Проверьте сцены:
- Откройте Assets/Scenes/CombatTestScene.unity
- Нажмите Play
- Протестируйте все механики

## 📝 Частые проблемы

### Игрок не двигается
- Проверьте PlayerInput компонент
- Убедитесь, что InputActions настроены
- Проверьте Rigidbody2D настройки

### Атаки не работают
- Проверьте LayerMask в атаке
- Убедитесь, что враги имеют IDamageable
- Проверьте аниматор

### Враги не атакуют
- Проверьте тег "Player" у игрока
- Убедитесь, что EnemyAI настроен
- Проверьте расстояния атаки

### Полоски здоровья не отображаются
- Проверьте EnemyHealthBar компонент
- Убедитесь, что UniversalHealthSystem работает
- Проверьте позиционирование полосок

## ✅ Готово!

После выполнения всех шагов у вас должна быть работающая боевая система с:
- Движением игрока
- Атаками игрока и врагов
- Системой здоровья
- Визуальными эффектами
- Искусственным интеллектом врагов 