# ⚡ Быстрая проверка атаки

## 🎯 5-минутная проверка

### 1. Проверьте компоненты игрока
```
✅ Player Input (с InputActions)
✅ Player Input Controller
✅ Player Combat
✅ Simple Attack System
```

### 2. Проверьте настройки Player Input
```
Actions: InputActions ✓
Default Map: Player ✓
Behavior: Send Messages ✓
```

### 3. Проверьте ссылки в Player Input Controller
```
Player Movement: ✓
Player Combat: ✓
Player Health System: ✓
Enable Debug Logs: ✓
```

### 4. Запустите игру и протестируйте
```
Space → Должна быть атака
Left Click → Должна быть атака
```

### 5. Проверьте логи в консоли
```
PlayerInputController: Атака!
SimpleAttackSystem: Атака началась
```

## 🔧 Если что-то не работает

### Проблема: Нет реакции на кнопки
**Быстрое решение:**
1. **Перезапустите Unity**
2. **Проверьте, что Player Input активен**
3. **Убедитесь, что InputActions назначен**

### Проблема: Нет логирования
**Быстрое решение:**
1. **Включите Enable Debug Logs во всех компонентах**
2. **Проверьте консоль на ошибки**
3. **Убедитесь, что все ссылки настроены**

### Проблема: Атака не наносит урон
**Быстрое решение:**
1. **Проверьте, что у врага есть Simple Health System**
2. **Убедитесь, что Attack Range > 0**
3. **Проверьте, что враг в зоне атаки**

## ✅ Готово!

Если все шаги выполнены, атака должна работать! 🎮 