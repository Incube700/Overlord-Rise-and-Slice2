# 🔧 Исправление проблемы с атакой

## 🚨 Проблема: Атака не работает

### Возможные причины:

1. **PlayerInput не настроен**
2. **InputActions не назначен**
3. **Ссылки между компонентами не настроены**
4. **Enable Debug Logs выключен**

## ✅ Пошаговое исправление

### Шаг 1: Проверьте Player Input компонент

1. **Выберите Player в Hierarchy**
2. **Найдите компонент Player Input**
3. **Проверьте настройки:**
   ```
   Actions: InputActions (должно быть назначено)
   Default Map: Player
   Behavior: Send Messages
   ```

### Шаг 2: Настройте Player Input Controller

1. **Найдите компонент Player Input Controller**
2. **Настройте ссылки:**
   ```
   Player Movement: перетащите Player Movement
   Player Combat: перетащите Player Combat
   Player Health System: перетащите Player Health System
   Enable Debug Logs: ✓ (ВКЛЮЧЕНО)
   ```

### Шаг 3: Проверьте Enable Debug Logs

**Включите во всех компонентах:**
```
Player Input Controller: Enable Debug Logs ✓
Player Combat: Enable Debug Logs ✓
Simple Attack System: Enable Debug Logs ✓
```

### Шаг 4: Перезапустите Unity

1. **Остановите игру** (если запущена)
2. **Сохраните сцену**
3. **Перезапустите Unity**
4. **Запустите игру заново**

## 🧪 Тестирование

### 1. Проверьте консоль при запуске
```
PlayerInputController: Компоненты инициализированы
PlayerInputController: Input Actions настроены
```

### 2. Протестируйте атаку
```
Space → Должно появиться: "PlayerInputController: Атака!"
```

### 3. Если нет логов
**Проверьте:**
- Enable Debug Logs включен
- Player Input активен
- InputActions назначен

## 🔧 Если атака все еще не работает

### Проблема: "PlayerInput или actions не найдены"
**Решение:**
1. Убедитесь, что Player Input компонент добавлен
2. Убедитесь, что InputActions назначен в Player Input
3. Перезапустите Unity

### Проблема: "Не удалось найти действия в InputActions"
**Решение:**
1. Проверьте, что InputActions.inputactions существует
2. Убедитесь, что в InputActions есть действия "Move", "Attack", "Dash"
3. Перезапустите Unity

### Проблема: "PlayerCombat не найден"
**Решение:**
1. Убедитесь, что Player Combat компонент добавлен
2. Настройте ссылку в Player Input Controller
3. Перезапустите Unity

### Проблема: Нет реакции на кнопки
**Решение:**
1. Проверьте, что Player Input активен
2. Убедитесь, что Behavior = "Send Messages"
3. Проверьте, что Default Map = "Player"

## 📊 Правильные настройки

### Player Input:
```
Actions: InputActions
Default Map: Player
Behavior: Send Messages
```

### Player Input Controller:
```
Player Movement: [ссылка на Player Movement]
Player Combat: [ссылка на Player Combat]
Player Health System: [ссылка на Player Health System]
Enable Debug Logs: ✓
```

### Player Combat:
```
Enable Debug Logs: ✓
```

### Simple Attack System:
```
Attack Damage: 15
Attack Range: 1.5
Attack Cooldown: 1.0
Enable Debug Logs: ✓
Show Attack Gizmos: ✓
```

## ✅ Ожидаемый результат

После исправления:
- ✅ При нажатии Space появляется лог "PlayerInputController: Атака!"
- ✅ При нажатии Left Click появляется лог "PlayerInputController: Атака!"
- ✅ Враги получают урон при атаке
- ✅ Атака имеет кулдаун
- ✅ Атака отображается визуально

**Если проблема остается, проверьте консоль на ошибки!** 🔍 