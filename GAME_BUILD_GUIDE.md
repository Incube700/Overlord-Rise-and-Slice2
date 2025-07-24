# 🎮 Overlord: Rise and Slice — Полное руководство по сборке

## 📋 **Обзор проекта**

**Overlord: Rise and Slice** — это 2D Top-Down Action Slasher с процедурной генерацией уровней, созданный на Unity. Игрок управляет восставшим Overlord'ом, исследующим мрачные подземелья замка.

---

## 🚀 **Быстрый старт (5 минут)**

### **1. Подготовка проекта:**
```bash
git clone https://github.com/Incube700/Overlord-Rise-and-Slice2.git
cd Overlord-Rise-and-Slice2
```

### **2. Откройте Unity:**
- Unity 2022.3 LTS или новее
- Откройте проект через Unity Hub

### **3. Проверьте скрипты:**
Все скрипты должны быть в `Assets/Scripts/`:
- ✅ `PlayerMovement.cs`
- ✅ `PlayerAttack.cs`
- ✅ `HealthSystem.cs`
- ✅ `CameraFollow.cs`
- ✅ `LevelGenerator.cs`
- ✅ `TileDatabase.cs`
- ✅ `EnemyAI.cs`
- ✅ `EnemyAttack.cs`

---

## 🏗️ **Полная сборка игры**

### **Шаг 1: Создание тайлов**

#### **1.1. Создайте временные тайлы:**
1. **Project Window** → **Create** → **2D** → **Sprites** → **Square**
2. Создайте 8 квадратов и назовите:
   - `Floor_1`, `Floor_2`, `Floor_3`
   - `Wall_1`, `Wall_2`
   - `Decor_1`, `Decor_2`, `Decor_3`

#### **1.2. Настройте цвета:**
- **Floor:** Серый (#808080), Тёмно-серый (#606060), Коричневый (#8B4513)
- **Walls:** Чёрный (#000000), Тёмно-красный (#800000)
- **Decor:** Зелёный (#228B22), Жёлтый (#FFD700), Фиолетовый (#800080)

#### **1.3. Создайте Tile ресурсы:**
1. **Create** → **2D** → **Tile** (8 раз)
2. Назовите тайлы соответственно спрайтам
3. Перетащите спрайты в поле **Sprite** каждого тайла

### **Шаг 2: Настройка сцены**

#### **2.1. Создайте новую сцену:**
1. **File** → **New Scene** → **2D Template**
2. **File** → **Save As** → `Assets/Scenes/MainScene.unity`

#### **2.2. Настройте камеру:**
1. Выберите **Main Camera**
2. **Add Component** → **CameraFollow**
3. Настройки:
   - **Follow Speed:** 0.125
   - **Offset:** (0, 0, -10)
   - **Background:** Чёрный (#000000)

#### **2.3. Создайте Tilemap систему:**
1. **Right Click** → **2D Object** → **Grid**
2. **Right Click на Grid** → **2D Object** → **Tilemap** (3 раза)
3. Переименуйте:
   - `Tilemap` → `Tilemap_Floor`
   - `Tilemap (1)` → `Tilemap_Walls`
   - `Tilemap (2)` → `Tilemap_Decor`

#### **2.4. Настройте коллизии стен:**
1. Выберите **Tilemap_Walls**
2. **Add Component:**
   - **TilemapCollider2D**
   - **CompositeCollider2D**
   - **Rigidbody2D**
3. Настройки:
   - **TilemapCollider2D:** ✅ Used By Composite
   - **Rigidbody2D:** Body Type = Static
   - **Tag:** "Wall"

### **Шаг 3: Создание игрока**

#### **3.1. Создайте объект игрока:**
1. **Right Click** → **2D Object** → **Sprite**
2. Переименуйте в **Player**
3. **Add Component:**
   - **Rigidbody2D**
   - **BoxCollider2D**
   - **PlayerMovement**
   - **PlayerAttack**
   - **HealthSystem**

#### **3.2. Настройте компоненты:**
- **Rigidbody2D:**
  - Gravity Scale: 0
  - Freeze Rotation: ✅ Z
- **Tag:** "Player"
- **Sprite:** можно использовать встроенный Unity квадрат

### **Шаг 4: Создание GameManager**

#### **4.1. Создайте менеджер:**
1. **Right Click** → **Create Empty**
2. Переименуйте в **GameManager**
3. **Add Component:**
   - **LevelGenerator**
   - **TileDatabase**

#### **4.2. Настройте LevelGenerator:**
1. **Floor Tilemap:** перетащите `Tilemap_Floor`
2. **Wall Tilemap:** перетащите `Tilemap_Walls`
3. **Decor Tilemap:** перетащите `Tilemap_Decor`
4. **Tile Database:** перетащите компонент `TileDatabase`

#### **4.3. Настройте TileDatabase:**
1. Перетащите все созданные тайлы в соответствующие поля:
   - **Floor Tile 1-3:** тайлы пола
   - **Wall Tile 1-2:** тайлы стен
   - **Decor Tile 1-3:** декоративные тайлы

### **Шаг 5: Финальная настройка**

#### **5.1. Связывание систем:**
1. **Main Camera** → **CameraFollow** → **Target:** перетащите **Player**
2. Проверьте все ссылки в **LevelGenerator**

#### **5.2. Создайте теги:**
1. **Tags & Layers** → **Tags**
2. Добавьте теги:
   - "Player"
   - "Wall"
   - "Enemy"

---

## 🎯 **Тестирование игры**

### **Запустите сцену (Play Mode):**

#### **✅ Должно работать:**
1. **Генерация уровня:** 20x20 комната с разными тайлами
2. **Движение игрока:** WASD управление
3. **Коллизии:** игрок не проходит сквозь стены
4. **Камера:** плавно следует за игроком
5. **Декор:** случайно размещён по уровню

#### **🔧 Если что-то не работает:**
1. **Проверьте консоль** на ошибки
2. **Убедитесь в назначении тегов** Player и Wall
3. **Проверьте все ссылки** в компонентах
4. **Перезапустите сцену**

---

## 🎮 **Добавление врагов (опционально)**

### **Создание врага:**
1. **Right Click** → **2D Object** → **Sprite**
2. Переименуйте в **Enemy**
3. **Add Component:**
   - **Rigidbody2D** (Gravity Scale: 0)
   - **CircleCollider2D**
   - **EnemyAI**
   - **EnemyAttack**
   - **HealthSystem**
4. **Tag:** "Enemy"
5. Поместите врага в центр комнаты

### **Настройка EnemyAI:**
- **Move Speed:** 2
- **Detection Range:** 5
- **Attack Range:** 1.5
- **Patrol Range:** 3

---

## 📦 **Структура готового проекта**

```
Overlord-Rise-and-Slice2/
├── Assets/
│   ├── Scripts/              # Все C# скрипты
│   │   ├── PlayerMovement.cs
│   │   ├── PlayerAttack.cs
│   │   ├── HealthSystem.cs
│   │   ├── CameraFollow.cs
│   │   ├── LevelGenerator.cs
│   │   ├── TileDatabase.cs
│   │   ├── EnemyAI.cs
│   │   └── EnemyAttack.cs
│   ├── Scenes/
│   │   └── MainScene.unity    # Основная сцена
│   ├── Tiles/                 # Tile ресурсы
│   ├── Sprites/               # Спрайты
│   └── Prefabs/               # Префабы
├── docs/
│   └── design.md             # Документация
├── Assets/
│   ├── SCENE_SETUP_GUIDE.md  # Руководство по сцене
│   └── Tiles/README_Tiles.md # Руководство по тайлам
├── README.md                 # Основная документация
└── GAME_BUILD_GUIDE.md      # Это руководство
```

---

## 🚀 **Следующие шаги развития**

### **Immediate (Sprint 3):**
- [ ] Анимации персонажа
- [ ] Звуковые эффекты
- [ ] UI система (здоровье, счёт)
- [ ] Эффекты частиц

### **Short-term (Sprint 4):**
- [ ] Больше типов врагов
- [ ] Система прокачки
- [ ] Разные типы комнат
- [ ] Сбор предметов

### **Long-term (Sprint 5+):**
- [ ] Процедурная генерация замка
- [ ] Боссы
- [ ] Система сохранения
- [ ] Мультиплеер

---

## 🎯 **Результат**

После выполнения всех шагов у вас будет:

✅ **Полностью рабочая игра** с управлением, генерацией уровней и камерой  
✅ **Модульная архитектура** для лёгкого расширения  
✅ **Документированный код** с комментариями на русском  
✅ **Готовая база** для добавления новых механик  
✅ **Система врагов** с ИИ и боевой системой  

**Игра готова к дальнейшему развитию! 🎮**

---

## 📞 **Поддержка**

Если возникли проблемы:
1. Проверьте консоль Unity на ошибки
2. Убедитесь, что все файлы на месте
3. Перечитайте `Assets/SCENE_SETUP_GUIDE.md`
4. Проверьте версию Unity (2022.3 LTS+)

**Удачи в разработке! 🎯** 