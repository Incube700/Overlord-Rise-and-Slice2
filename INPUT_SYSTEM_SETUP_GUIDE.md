# 🎮 Настройка Input System для игрока

## ✅ Текущее состояние

**Input System уже настроен и готов к работе!**

### Что уже есть:
- ✅ **InputActions.inputactions** - все действия настроены
- ✅ **PlayerInputController.cs** - обработка ввода
- ✅ **PlayerMovement.cs** - движение персонажа
- ✅ **com.unity.inputsystem** - пакет установлен

## 🚀 Быстрая настройка игрока

### Шаг 1: Добавьте компоненты к игроку

1. **Выберите игрока в Hierarchy**
2. **Добавьте компоненты в этом порядке:**

```
Add Component → Player Input Controller
Add Component → Player Movement
Add Component → Player Combat
Add Component → Player Health System
```

### Шаг 2: Настройте Player Input

1. **Найдите компонент Player Input Controller**
2. **Настройте ссылки:**
   - **Player Movement:** перетащите компонент Player Movement
   - **Player Combat:** перетащите компонент Player Combat  
   - **Player Health System:** перетащите компонент Player Health System
   - **Enable Debug Logs:** ✓

### Шаг 3: Настройте Player Input (важно!)

1. **Add Component → Player Input**
2. **В Player Input настройте:**
   - **Actions:** перетащите `InputActions` из Assets
   - **Default Map:** Player
   - **Behavior:** Send Messages

## 🎯 Доступные способы управления

### Клавиатура:
- **WASD** - движение
- **Стрелки** - движение  
- **Space** - атака
- **Left Shift** - рывок

### Мышь:
- **Left Click** - атака

### Геймпад:
- **Left Stick** - движение
- **X/Square** - атака
- **Circle/B** - рывок

## 🧪 Тестирование

### 1. Запустите игру
```
Play → Проверьте консоль на ошибки
```

### 2. Проверьте управление
```
WASD → Движение
Space → Атака
Shift → Рывок
```

### 3. Проверьте логи
```
PlayerInputController: Input Actions настроены
PlayerMovement: Компоненты инициализированы
```

## 🔧 Возможные проблемы

### Проблема: Персонаж не двигается
**Решение:**
1. Убедитесь, что есть **Player Input** компонент
2. Убедитесь, что **InputActions** назначен в Player Input
3. Убедитесь, что **Player Input Controller** добавлен

### Проблема: "InputActions not found"
**Решение:**
1. Найдите `InputActions.inputactions` в Assets
2. Перетащите его в поле Actions в Player Input
3. Убедитесь, что Default Map = "Player"

### Проблема: Нет реакции на кнопки
**Решение:**
1. Перезапустите Unity
2. Проверьте, что Player Input активен
3. Убедитесь, что Behavior = "Send Messages"

## 📊 Рекомендуемые настройки

### Player Movement:
```
Move Speed: 5
Dash Speed: 15
Dash Duration: 0.2
Dash Cooldown: 1.0
```

### Player Input Controller:
```
✅ Enable Debug Logs: ВКЛЮЧЕНО
✅ Все ссылки на компоненты настроены
```

### Player Input:
```
Actions: InputActions
Default Map: Player
Behavior: Send Messages
```

## ✅ Готово!

После настройки у вас будет:
- ✅ Движение по WASD и стрелкам
- ✅ Атака пробелом или мышью
- ✅ Рывок Shift'ом
- ✅ Поддержка геймпада
- ✅ Нормализация диагонального движения

**Не забудьте протестировать все способы управления!** 🎮 