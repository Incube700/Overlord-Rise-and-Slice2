# 🎭 Настройка Animator Controller в Unity

## ❌ Проблема
Файл `Player_Animator.controller` был удалён из-за ошибки парсинга YAML.

## ✅ Решение - Создание через Unity Editor

### 1. **Создание Animator Controller:**
1. В Project Window перейдите в `Assets/Animations/Player/`
2. **Правый клик** → **Create** → **Animator Controller**
3. Назовите его `Player_Animator`

### 2. **Настройка параметров:**
1. Откройте созданный `Player_Animator.controller`
2. В окне **Parameters** добавьте:
   - `isMoving` (Bool)
   - `isDashing` (Bool) 
   - `isAttacking` (Bool)
   - `Attack` (Trigger)
   - `moveX` (Float)
   - `moveY` (Float)
   - `lastMoveX` (Float)
   - `lastMoveY` (Float)

### 3. **Создание состояний:**
1. В окне **Layers** создайте состояния:
   - **Idle** (по умолчанию)
   - **Walk**
   - **Dash**
   - **Attack**

### 4. **Настройка переходов:**
1. **Idle → Walk:** условие `isMoving = true`
2. **Walk → Idle:** условие `isMoving = false`
3. **Any State → Dash:** условие `isDashing = true`
4. **Dash → Idle:** условие `isDashing = false`
5. **Any State → Attack:** условие `Attack` (trigger)
6. **Attack → Idle:** условие `isAttacking = false` + Exit Time

### 5. **Настройка переходов:**
- **Transition Duration:** 0.1 для обычных переходов
- **Transition Duration:** 0.05 для dash и attack
- **Exit Time:** 0.9 для Attack → Idle

### 6. **Подключение к игроку:**
1. Выделите **Player** в сцене
2. В **Inspector** найдите **Animator** компонент
3. Перетащите `Player_Animator.controller` в поле **Controller**

## 🎬 Создание анимаций (опционально)

### 1. **Создание анимационных клипов:**
1. Выделите **Player** в сцене
2. **Window** → **Animation** → **Animation**
3. Создайте клипы:
   - `Player_Idle.anim`
   - `Player_Walk.anim`
   - `Player_Dash.anim`
   - `Player_Attack.anim`

### 2. **Настройка анимаций:**
1. Откройте каждый клип
2. Добавьте ключевые кадры для:
   - **Sprite Renderer → Sprite** (смена спрайтов)
   - **Transform → Scale** (эффекты)
3. Настройте **Loop Time** в инспекторе

### 3. **Подключение к Animator Controller:**
1. Откройте `Player_Animator.controller`
2. Для каждого состояния назначьте соответствующий клип:
   - **Idle State** → `Player_Idle.anim`
   - **Walk State** → `Player_Walk.anim`
   - **Dash State** → `Player_Dash.anim`
   - **Attack State** → `Player_Attack.anim`

## 🧪 Тестирование

### ✅ **Проверка работы:**
1. Запустите сцену
2. Двигайтесь **WASD** - должна быть анимация Walk
3. Нажмите **Shift** - должна быть анимация Dash
4. Нажмите **Mouse0** - должна быть анимация Attack
5. Стоя на месте - должна быть анимация Idle

## 🔧 Отладка

### 🐛 **Возможные проблемы:**
- **Анимации не работают:** проверьте подключение Controller к Animator
- **Переходы не работают:** проверьте условия в Transitions
- **Параметры не обновляются:** проверьте код в PlayerMovement и PlayerMeleeAttack

## 📝 Примечания

- **Без анимаций** система всё равно работает, но без визуальных эффектов
- **Код уже готов** к работе с анимациями
- **Параметры обновляются** автоматически из скриптов
- **Можно добавить анимации позже** без изменения кода 