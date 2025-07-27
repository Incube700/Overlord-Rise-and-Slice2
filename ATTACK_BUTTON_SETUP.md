# ⚔️ Подключение кнопки атаки

## ✅ Текущее состояние

**Кнопка атаки уже настроена в Input System!**

### Что уже работает:
- ✅ **InputActions** - кнопка атаки настроена
- ✅ **PlayerInputController** - обработка атаки
- ✅ **PlayerCombat** - выполнение атаки
- ✅ **SimpleAttackSystem** - система атаки

## 🎮 Доступные кнопки атаки

### Клавиатура:
- **Space** - основная кнопка атаки

### Мышь:
- **Left Click** - атака мышью

### Геймпад:
- **X/Square** - атака геймпадом

## 🔧 Проверка подключения

### 1. Проверьте Player Input компонент

1. **Выберите Player в Hierarchy**
2. **Найдите компонент Player Input**
3. **Убедитесь, что настроено:**
   ```
   Actions: InputActions
   Default Map: Player
   Behavior: Send Messages
   ```

### 2. Проверьте Player Input Controller

1. **Найдите компонент Player Input Controller**
2. **Убедитесь, что ссылки настроены:**
   ```
   Player Movement: ✓
   Player Combat: ✓
   Player Health System: ✓
   Enable Debug Logs: ✓
   ```

### 3. Проверьте Player Combat

1. **Найдите компонент Player Combat**
2. **Убедитесь, что есть:**
   ```
   Enable Debug Logs: ✓
   ```

## 🧪 Тестирование атаки

### 1. Запустите игру
```
Play → Проверьте консоль
```

### 2. Протестируйте атаку
```
Space → Атака
Left Click → Атака
```

### 3. Проверьте логи в консоли
```
PlayerInputController: Атака!
SimpleAttackSystem: Атака началась
SimpleAttackSystem: Атака завершена
```

## 🔧 Если атака не работает

### Проблема: Нет реакции на Space
**Решение:**
1. Убедитесь, что **Player Input** компонент добавлен
2. Убедитесь, что **InputActions** назначен в Player Input
3. Убедитесь, что **Player Input Controller** добавлен

### Проблема: Нет реакции на мышь
**Решение:**
1. Проверьте, что мышь подключена
2. Убедитесь, что в InputActions настроен Left Button
3. Проверьте, что нет конфликтов с UI

### Проблема: Атака не выполняется
**Решение:**
1. Убедитесь, что **Simple Attack System** добавлен к игроку
2. Проверьте, что **Player Combat** ссылается на Simple Attack System
3. Убедитесь, что **Enable Debug Logs** включен

### Проблема: Нет логирования
**Решение:**
1. Включите **Enable Debug Logs** во всех компонентах
2. Перезапустите Unity
3. Проверьте консоль на ошибки

## 📊 Настройки атаки

### Simple Attack System:
```
Attack Damage: 15
Attack Range: 1.5
Attack Cooldown: 1.0
Attack Duration: 0.3
Enable Debug Logs: ✓
Show Attack Gizmos: ✓
```

### Player Combat:
```
Enable Debug Logs: ✓
```

### Player Input Controller:
```
Enable Debug Logs: ✓
Player Combat: ссылка на Player Combat
```

## 🎯 Как работает атака

### 1. Нажатие кнопки
```
Space/Left Click → Player Input → Player Input Controller
```

### 2. Обработка ввода
```
PlayerInputController.OnAttackPerformed() → PlayerCombat.PerformAttack()
```

### 3. Выполнение атаки
```
PlayerCombat → SimpleAttackSystem → Обнаружение врагов → Урон
```

### 4. Логирование
```
PlayerInputController: Атака!
SimpleAttackSystem: Атака началась
SimpleAttackSystem: Враг получил урон
SimpleAttackSystem: Атака завершена
```

## ✅ Готово!

После проверки у вас должно работать:
- ✅ Атака по Space
- ✅ Атака по Left Click
- ✅ Атака геймпадом
- ✅ Логирование в консоли
- ✅ Урон врагам
- ✅ Визуальные эффекты атаки

**Протестируйте все способы атаки!** ⚔️ 