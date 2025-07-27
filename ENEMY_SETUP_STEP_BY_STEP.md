# Пошаговое руководство по настройке врага

## 🎯 Цель
Создать рабочего врага, который:
- ✅ Двигается к игроку
- ✅ Атакует при приближении
- ✅ Получает урон и умирает
- ✅ Имеет полоску здоровья

## 🚀 Пошаговая настройка

### Шаг 1: Создание базового объекта

1. **В Hierarchy** → **Right Click** → **Create Empty**
2. **Переименуйте** в `TestEnemy`
3. **Установите Position** в `(0, 0, 0)`

### Шаг 2: Добавление визуального компонента

1. **Add Component** → **Sprite Renderer**
2. **Установите Sprite** (любой квадратный спрайт)
3. **Установите Color** в красный `(255, 0, 0)` для отличия от игрока

### Шаг 3: Добавление физики

1. **Add Component** → **Rigidbody 2D**
2. **Настройте параметры:**
   - **Body Type:** Dynamic
   - **Gravity Scale:** 0
   - **Freeze Rotation:** Z ✓
   - **Collision Detection:** Continuous

3. **Add Component** → **Box Collider 2D**
   - **Is Trigger:** ✗ (выключено)
   - **Size:** подгоните под спрайт

### Шаг 4: Добавление скриптов (В ПРАВИЛЬНОМ ПОРЯДКЕ!)

#### 4.1 EnemyController (главный)
1. **Add Component** → **Enemy Controller**
2. **Настройте параметры:**
   - **Enemy Name:** "TestEnemy"
   - **Enemy Level:** 1
   - **Auto Setup Components:** ✓
   - **Create Health Bar If Missing:** ✓
   - **Enable Debug Logs:** ✓

#### 4.2 SimpleHealthSystem
1. **Add Component** → **Simple Health System**
2. **Настройте параметры:**
   - **Max Health:** 100
   - **Invulnerability Duration:** 0.5
   - **Damage Flash Duration:** 0.1
   - **Damage Flash Count:** 3
   - **Death Delay:** 0.5
   - **Destroy On Death:** ✓
   - **Enable Debug Logs:** ✓

#### 4.3 SimpleAttackSystem
1. **Add Component** → **Simple Attack System**
2. **Настройте параметры:**
   - **Attack Damage:** 15
   - **Attack Range:** 1.5
   - **Attack Cooldown:** 1.0
   - **Attack Duration:** 0.3
   - **Enable Debug Logs:** ✓
   - **Show Attack Gizmos:** ✓

#### 4.4 EnemyAI
1. **Add Component** → **Enemy AI**
2. **Настройте параметры:**
   - **Move Speed:** 2
   - **Detection Range:** 5
   - **Attack Range:** 1.5
   - **Attack Cooldown:** 1
   - **Enable Debug Logs:** ✓
   - **Show Gizmos:** ✓

#### 4.5 Animator (опционально)
1. **Add Component** → **Animator**
2. **Controller:** оставьте пустым (создадим позже)
3. **Apply Root Motion:** ✗

### Шаг 5: Настройка тегов и слоев

1. **Убедитесь, что игрок имеет тег "Player"**
2. **Убедитесь, что враг имеет тег "Enemy"** (если нужен)

### Шаг 6: Проверка компонентов

После добавления всех компонентов в Inspector должно быть:
```
✅ Transform
✅ Sprite Renderer
✅ Rigidbody 2D
✅ Box Collider 2D
✅ Enemy Controller
✅ Simple Health System
✅ Simple Attack System
✅ Enemy AI
✅ Animator (опционально)
```

## 🧪 Тестирование

### 1. Запустите игру
1. **Нажмите Play** в Unity
2. **Проверьте консоль** - не должно быть ошибок

### 2. Проверьте поведение врага
1. **Подойдите к врагу** - он должен начать движение
2. **Подойдите вплотную** - он должен атаковать
3. **Атакуйте врага** - он должен получать урон
4. **Убейте врага** - он должен умереть и исчезнуть

### 3. Проверьте логи в консоли
Должны появиться сообщения:
```
EnemyAI: Enemy started moving
EnemyAI: Enemy started attacking
SimpleHealthSystem: Получен урон
SimpleHealthSystem: Враг умер
```

## 🔧 Возможные проблемы

### Проблема: Враг не двигается
**Решение:**
- Проверьте, что игрок имеет тег "Player"
- Проверьте, что Detection Range > 0
- Проверьте, что Move Speed > 0

### Проблема: Враг не атакует
**Решение:**
- Проверьте, что Attack Range > 0
- Проверьте, что Attack Cooldown > 0
- Проверьте, что SimpleAttackSystem добавлен

### Проблема: Враг не получает урон
**Решение:**
- Проверьте, что SimpleHealthSystem добавлен
- Проверьте, что Max Health > 0
- Проверьте, что Destroy On Death включен

### Проблема: Ошибки в консоли
**Решение:**
- Проверьте, что все компоненты добавлены
- Проверьте, что Enable Debug Logs включен
- Перезапустите Unity

## 📊 Рекомендуемые настройки

### Для простого тестового врага:
```
Move Speed: 2
Detection Range: 5
Attack Range: 1.5
Attack Cooldown: 1.0
Max Health: 100
Attack Damage: 15
```

### Для сильного врага:
```
Move Speed: 3
Detection Range: 7
Attack Range: 2.0
Attack Cooldown: 0.8
Max Health: 200
Attack Damage: 25
```

## ✅ Готово!

После выполнения всех шагов у вас должен быть полностью рабочий враг, который:
- Двигается к игроку
- Атакует при приближении
- Получает урон и умирает
- Имеет полоску здоровья
- Логирует все действия в консоль 