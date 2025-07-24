# 🚀 БЫСТРАЯ НАСТРОЙКА ИГРЫ (5 минут)

## ✅ **Готово автоматически:**
- ✅ Все скрипты в `Assets/Scripts/`
- ✅ Базовая сцена `Assets/Scenes/MainScene.unity`
- ✅ Структура папок Unity

## 🎮 **Откройте проект в Unity:**

### **1. Откройте Unity Hub**
### **2. Add Project From Disk**
### **3. Выберите эту папку:** 
```
/Users/serg/Desktop/Projeсts/OverlordRiseAndSlice/Overlord-Rise-and-Slice2
```

### **4. Откройте проект**
- Unity должен автоматически импортировать все скрипты
- Откроется сцена MainScene

---

## 🔧 **БЫСТРАЯ НАСТРОЙКА В UNITY (следуйте точно):**

### **Шаг 1: Создайте временные тайлы (30 сек)**

1. **Project** → правый клик → **Create** → **2D** → **Sprites** → **Square**
2. Создайте 8 квадратов, назовите:
   - `Floor_1`, `Floor_2`, `Floor_3` (цвета: серый, тёмно-серый, коричневый)
   - `Wall_1`, `Wall_2` (цвета: чёрный, тёмно-красный)
   - `Decor_1`, `Decor_2`, `Decor_3` (цвета: зелёный, жёлтый, фиолетовый)

3. **Создайте Tile ресурсы:**
   - Правый клик → **Create** → **2D** → **Tile** (8 раз)
   - Назовите: `Floor_Tile_1`, `Floor_Tile_2`, и т.д.
   - Перетащите спрайты в поле **Sprite** каждого тайла

### **Шаг 2: Настройте сцену (2 минуты)**

#### **2.1. Настройте камеру:**
1. Выберите **Main Camera**
2. **Add Component** → **CameraFollow**

#### **2.2. Создайте Tilemap:**
1. **Hierarchy** → правый клик → **2D Object** → **Grid**
2. **Правый клик на Grid** → **2D Object** → **Tilemap** (3 раза)
3. Переименуйте:
   - `Tilemap` → `Tilemap_Floor`
   - `Tilemap (1)` → `Tilemap_Walls`
   - `Tilemap (2)` → `Tilemap_Decor`

#### **2.3. Настройте стены:**
1. Выберите **Tilemap_Walls**
2. **Add Component** → **TilemapCollider2D**
3. **Add Component** → **CompositeCollider2D**
4. **Add Component** → **Rigidbody2D**
5. **TilemapCollider2D:** ✅ Used By Composite
6. **Rigidbody2D:** Body Type = Static
7. **Tag** → создайте "Wall" и назначьте

#### **2.4. Создайте игрока:**
1. **Hierarchy** → правый клик → **2D Object** → **Sprite**
2. Переименуйте в **Player**
3. **Add Component:**
   - **Rigidbody2D** (Gravity Scale: 0, Freeze Rotation Z: ✅)
   - **BoxCollider2D**
   - **PlayerMovement**
   - **PlayerAttack**
   - **HealthSystem**
4. **Tag** → создайте "Player" и назначьте

#### **2.5. Создайте GameManager:**
1. **Hierarchy** → правый клик → **Create Empty**
2. Переименуйте в **GameManager**
3. **Add Component:**
   - **LevelGenerator**
   - **TileDatabase**

### **Шаг 3: Свяжите компоненты (1 минута)**

#### **В LevelGenerator:**
- **Floor Tilemap:** перетащите `Tilemap_Floor`
- **Wall Tilemap:** перетащите `Tilemap_Walls`
- **Decor Tilemap:** перетащите `Tilemap_Decor`
- **Tile Database:** перетащите компонент `TileDatabase`

#### **В TileDatabase:**
- **Floor Tile 1-3:** перетащите Floor тайлы
- **Wall Tile 1-2:** перетащите Wall тайлы
- **Decor Tile 1-3:** перетащите Decor тайлы

#### **В CameraFollow:**
- **Target:** перетащите **Player**

### **Шаг 4: ЗАПУСК! ▶️**

**Нажмите Play!**

**Должно работать:**
- ✅ Генерируется уровень 20x20
- ✅ WASD управляет игроком
- ✅ Камера следует за игроком
- ✅ Коллизии со стенами

---

## 🚨 **Если не работает:**

### **Скрипты не видны:**
- Проверьте Console на ошибки
- Убедитесь, что все .cs файлы в Assets/Scripts/

### **Уровень не генерируется:**
- Проверьте, что все тайлы назначены в TileDatabase
- Убедитесь, что все Tilemap назначены в LevelGenerator

### **Игрок не двигается:**
- Rigidbody2D: Gravity Scale = 0
- PlayerMovement компонент добавлен

### **Нет коллизий:**
- Тег "Wall" на Tilemap_Walls
- Used By Composite включен

---

## 🎯 **ГОТОВО!**

После настройки у вас будет полностью рабочая игра!

**Приятной игры! 🎮** 