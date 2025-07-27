# Простое руководство по настройке

## ✅ Что сделано

Я удалил все проблемные скрипты и создал простые рабочие версии:

### Новые скрипты:
- `SimpleHealthSystem.cs` - универсальная система здоровья
- `SimpleAttackSystem.cs` - универсальная система атаки  
- `PlayerHealthSystem.cs` - система здоровья игрока с возрождением

### Обновленные скрипты:
- `EnemyAI.cs` - теперь использует SimpleHealthSystem и SimpleAttackSystem
- `EnemyController.cs` - обновлен для работы с новыми системами
- `EnemyHealthBar.cs` - обновлен для работы с SimpleHealthSystem
- `PlayerCombat.cs` - обновлен для работы с SimpleAttackSystem

## 🔧 Настройка в Unity

### Для Player:
1. **Выберите Player** в Hierarchy
2. **Удалите** все компоненты с ошибками "Missing (Script)"
3. **Добавьте компоненты** в правильном порядке:
   ```
   - Rigidbody2D
   - Collider2D  
   - PlayerMovement
   - SimpleAttackSystem
   - PlayerCombat
   - PlayerHealthSystem
   - PlayerInputController
   - Animator
   ```

### Для TestEnemy:
1. **Выберите TestEnemy** в Hierarchy
2. **Удалите** все компоненты с ошибками "Missing (Script)"
3. **Добавьте компоненты** в правильном порядке:
   ```
   - Rigidbody2D
   - Collider2D
   - SimpleHealthSystem
   - SimpleAttackSystem
   - EnemyAI
   - EnemyController
   - Animator
   ```

## ⚙️ Настройка параметров

### SimpleHealthSystem (для врагов):
- **Max Health**: 100
- **Invulnerability Duration**: 0.5
- **Damage Flash Duration**: 0.1
- **Damage Flash Count**: 3
- **Death Delay**: 0.5
- **Destroy On Death**: true

### SimpleAttackSystem (для врагов):
- **Attack Damage**: 15
- **Attack Range**: 1.5
- **Attack Cooldown**: 1.0
- **Attack Duration**: 0.3

### PlayerHealthSystem:
- **Max Health**: 100
- **Respawn Delay**: 2.0
- **Respawn At Start Position**: true

## 🎯 Проверка работы

После настройки:
1. **Нажмите Play** в Unity
2. **Проверьте Console** - не должно быть ошибок
3. **Проверьте движение** игрока (WASD)
4. **Проверьте атаку** игрока (левая кнопка мыши)
5. **Проверьте врагов** - они должны двигаться к игроку и атаковать

## 🚨 Если что-то не работает

### Проверьте:
1. **Input Actions** настроены правильно
2. **Player** имеет тег "Player"
3. **Все компоненты** добавлены в правильном порядке
4. **Console** не показывает ошибок

### Если есть ошибки:
1. **Закройте Unity**
2. **Откройте Unity заново**
3. **Подождите** пока Unity перекомпилирует скрипты

## ✅ Готово!

Теперь у вас простая, рабочая система без сложного наследования. Все компоненты независимы и легко настраиваются. 