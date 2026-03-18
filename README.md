- Видео геймплея: https://youtu.be/0sVQsZtsW1Q
- Билд: https://drive.google.com/file/d/15z0wJ5YjUkUvmIg06pOqkqSFNOWz6S3l/view?usp=sharing

Видео было записано в редакторе, в билде шейдер врагов не работает для затухания после смерти, хоть шейдер точно попадает в билд, даже переписывал шейдер, пока не успел разобраться почему так происходит

Это README не полная по архитектуре и принятых решений, я дополню

## 1. Базовая архитектура папок и сборок

Проект организован вокруг `Assets/Assemblies`, где каждый доменный блок вынесен в отдельную assembly.

Типовая структура внутри assembly:
- `Content` — сцены, префабы, ScriptableObject-конфиги, UI-ассеты.
- `Runtime` — runtime-код, участвующий в игре.
- `Editor` — редакторские инструменты и расширения.

### 1.1 Пакет личных фреймворков

Проект опирается на мой UPM-пакет `Packages/com.vladislavtsurikov.frameworks`, который содержит базовые строительные блоки архитектуры.

### 1.2 Разделение на asmdef

Разделение на отдельные `asmdef` сделано, чтобы развести ответственность модулей и упростить дальнейшее развитие.

- `Assets/Assemblies/AutoStrike/AutoStrike.asmdef` (`AutoStrike`) — базовый gameplay-модуль с основной логикой игры. Здесь находятся игрок, враги, UI, сервисы, конфиги и интеграция доменной логики в runtime.
- `Assets/Assemblies/WaypointsSystem/WaypointsSystem.asmdef` (`WaypointsSystem`) — отдельный модуль waypoint-путей. Вынесен отдельно, потому что это самостоятельная система со своими runtime-данными, editor-инструментами и правилами работы с путями. Такой модуль можно развивать независимо от конкретной игры.
- `Assets/Assemblies/WaypointPathEntitySpawner/WaypointPathEntitySpawner.asmdef` (`WaypointPathEntitySpawner`) — модуль спавна сущностей на waypoint-путях. Он не зашит в доменную логику `AutoStrike`, а работает как отдельный слой между системой путей и конкретной игрой. Это позволяет переиспользовать сам подход спавна и отдельно расширять правила выбора пути, capacity и позиции спавна.
- `Assets/Assemblies/SceneManagerIntergration/ArmyClash.SceneManager.asmdef` (`ArmyClash.SceneManager`) — интеграционный bootstrap-модуль на базе `SceneManagerTool`. Содержит операции, которые выполняются при старте игры: загрузка resource loaders, биндинг конфигов и начальный спавн врагов после загрузки сцены.

## 2. Технологический стек и каркас

- **Zenject**
  Выбран для DI и архитектуры зависимостей: внутри класса не нужно вручную искать зависимости, они приходят через контейнер.
  Через `Zenject` в проекте создаются и связываются сервисы, спавнеры, bootstrap-операции и resource loaders.
  В текущей реализации через контейнер биндятся `KillCounterService`, `EnemyRegistryService`, `AutoRespawnService`, `WaypointPathEntitySpawner` и runtime-конфиги.

- **UniRx**
  Выбран как оптимизационный реактивный инструмент: логика выполняется по событию, а не через постоянные проверки условий в `Update`.
  Можно подписываться на реактивные поля и события жизненного цикла из разных частей кода и в нужный момент.
  В проекте `UniRx` используется для работы со счетчиком убийств, списком живых врагов, текущей целью, вводом и UI-синхронизацией.

- **EntityDataAction**
  Выбран как основной подход к построению gameplay-логики: код получается расширяемым и собирается из `ComponentData` и `Action`.
  Сущности в проекте строятся через `EntityMonoBehaviour`, `ComponentData` и `EntityMonoBehaviourAction`.
  Плюсы такого подхода в этом проекте: можно точечно отключать отдельные `Action` и быстро проверять влияние на поведение, добавлять debug-блоки вроде `OnDrawGizmos`, переиспользовать готовые data/action-элементы и не решать всё через большое наследование и дублирование.
  Такой подход ускоряет создание механик, но увеличивает количество файлов.

- **UI Toolkit**
  Используется для UI.
  UI включен в ту же архитектурную модель через `UIToolkitEntity`, `UIToolkitViewData` и `UIToolkitAction`, поэтому UI не превращается в отдельный procedural-слой.

- **AddressableLoaderSystem**
  Выбран для загрузки/биндинга конфигов: конфиги поднимаются через Addressables до старта боя, автоматически биндятся в Zenject и корректно выгружаются, это мощный инструмент для автоматической загрузку и выгрузки, также позволяет множество конфигов загружать батчами паралельно. Хоть в проекте через него загружается один конфиг, я решил его добавить, как основу архитектуры проекта

- **SceneManagerTool**
  Выбран как bootstrap-точка входа в игру: через `Play` выполняется управляемый конвейер старта с операциями до и после загрузки сцены. Это уже готовая bootstrap реализация, она очень расширяемая и аналогов по вохможностям не видел

## 3. Точка запуска и bootstrap-поток

Старт игры в редакторе выполняется через `SceneManagerTool`, через кнопку `Play` в окне `Scene Manager`.

Окно открывается через путь:
- `Window -> Vladislav Tsurikov -> Scene Manager`

Этот подход был выбран не только как способ запуска сцены, но и как расширяемая точка инициализации игры. `SceneManagerTool` достаточно мощный для более крупных проектов: он даёт более удобный способ работать со сценами, управлять bootstrap-процессом и постепенно наращивать пайплайн инициализации без превращения старта игры в набор случайных вызовов из разных мест.

При таком запуске используется конвейер операций `SceneManager`:

`BeforeLoad`: выполняется `LoadResourceLoadersOperation` — загружаются нужные конфиги и resource loaders до загрузки целевой сцены.

`AfterLoad`: выполняется `SpawnEnemiesOperation` — вызывается стартовый спавн врагов до `MaxMobCount`.

Скриншот bootstrap-инструмента:

<p align="center">
  <a href="Docs/Images/scene-manager-tool.png">
    <img src="Docs/Images/scene-manager-tool.png" alt="SceneManagerTool" width="500">
  </a>
</p>

## 4. Архитектура данных и поведения

### 4.1 Entity-уровень

В проекте есть две основные gameplay-сущности:
- `PlayerEntity`
- `EnemyEntity`

Обе сущности не содержат "большой контроллер". Они создают набор `ComponentData` и `Action`, из которых собирается итоговое поведение.

### 4.2 Игрок

`PlayerEntity` создаёт данные:
- `StatsEntityData`
- `TargetData`
- `AttackDistanceData`
- `PlayerInputData`

И набор action:
- `ReadInputAction`
- `MoveByInputAction`
- `UpdateAutoAttackTargetAction`
- `RotateByInputAction`
- `RotateToTargetAction`
- `AttackTargetAction`

Роли этих action:
- `ReadInputAction` читает Input System и записывает результат в `PlayerInputData`;
- `MoveByInputAction` перемещает `Rigidbody` на базе направления ввода и стата `SPEED`;
- `RotateByInputAction` разворачивает персонажа по направлению движения;
- `UpdateAutoAttackTargetAction` управляет выбором ближайшей цели в радиусе;
- `RotateToTargetAction` разворачивает игрока к текущей цели;
- `AttackTargetAction` наносит урон на основе `ATK` и `ATKSPD`.

Скриншот `PlayerEntity` в инспекторе:

<p align="center">
  <a href="Docs/Images/player-entity-inspector.png">
    <img src="Docs/Images/player-entity-inspector.png" alt="PlayerEntity Inspector" width="400">
  </a>
</p>

### 4.3 Противники

`EnemyEntity` создаёт данные:
- `StatsEntityData`
- `WaypointPathData`
- `WaypointPathDirectionData`

И action:
- `PatrolMoveAction`
- `PatrolRotateAction`
- `DeathAction`

Роли:
- `PatrolMoveAction` двигает врага вдоль assigned waypoint-пути;
- `PatrolRotateAction` синхронизирует направление поворота с движением по сегменту пути;
- `DeathAction` слушает `HP`, удаляет врага из реестра, увеличивает счетчик убийств и уничтожает GameObject.

Скриншот `EnemyEntity` в инспекторе:

<p align="center">
  <a href="Docs/Images/enemy-entity-inspector.png">
    <img src="Docs/Images/enemy-entity-inspector.png" alt="EnemyEntity Inspector" width="400">
  </a>
</p>

### 4.4 Почему выбран такой подход

Это мой первый опыт использования этого фреймворка именно для gameplay-логики, хотел расширить свои знания в таком подходе программирования, для ТЗ, в плане расширяемости, он очень годится. До этого у меня уже был опыт с `DOTS`, а сам этот подход раньше применялся для сложного UI на `Unity UI Canvas`, где он помогал переиспользовать блоки кода в другом UI и ускорял разработку.

По сути это ECS-подход. В классическом ECS поведение обычно уходит в `System`, который нельзя настраивать в Inspector. Здесь эту роль выполняет `Action`: его можно конфигурировать прямо в Inspector и быстрее собирать логику без лишних промежуточных данных и отдельных конфигов. За счёт этого структура остаётся проще и не разрастается лишними файлами.

Дополнительно система удобна тем, что `ComponentData` может становиться "грязным": если `Action` или сторонний код меняет данные, автоматически вызывается зависимый `Action`. Это упрощает реактивное поведение, хорошо работает для UI и позволяет тестировать часть логики в `EditMode` без запуска `PlayMode`. Ещё один практический плюс — у каждого `Action` можно отдельно включать `OnDrawGizmos`, что удобно для отладки и настройки конкретной логики.

## 5. Архитектура статов и конфигов

Статы в проекте не писались с нуля под это ТЗ, а собраны на базе уже существующей системы статов из отдельного `asmdef` `Packages/com.vladislavtsurikov.frameworks/Stats/VladislavTsurikov.Stats.asmdef`.
Отдельная документация по этой системе: [Stats Architecture](Packages/com.vladislavtsurikov.frameworks/Stats/README.md).

Для текущего проекта такая система статов, возможно, выглядит избыточной, потому что здесь статы пока используются достаточно просто, как числа в конфигах. Но я всё равно решил опираться не на примитивные поля в `MonoBehaviour`, а на уже готовую и достаточно мощную stat-систему. Это задел на будущее: в неё можно добавлять модификаторы, бафы, эффекты на статы, а также без переписывания runtime-логики делать разные значения здоровья, скорости или силы атаки у разных сущностей. То есть сейчас статы используются достаточно просто, но архитектура уже готова к более сложным сценариям.

Основные конфиги проекта:
- `Assets/Assemblies/AutoStrike/Content/Configs/Stats/Player/ATK.asset` — базовый стат урона игрока.
- `Assets/Assemblies/AutoStrike/Content/Configs/Stats/Player/ATKSPD.asset` — базовый стат скорости атаки игрока.
- `Assets/Assemblies/AutoStrike/Content/Configs/Stats/Player/SPEED.asset` — базовый стат скорости движения игрока.
- `Assets/Assemblies/AutoStrike/Content/Configs/Stats/Enemy/HP.asset` — базовый стат здоровья врага.
- `Assets/Assemblies/AutoStrike/Content/Configs/Stats/Enemy/SPEED.asset` — базовый стат скорости патруля врага.
- `Assets/Assemblies/AutoStrike/Content/Configs/StatsCollections/PlayerEntityCollection.asset` — набор статов, который используется как stat-конфиг игрока.
- `Assets/Assemblies/AutoStrike/Content/Configs/StatsCollections/EnemyEntityCollection.asset` — набор статов, который используется как stat-конфиг врага.
- `Assets/Assemblies/AutoStrike/Content/Configs/EnemySpawnConfig.asset` — отдельный конфиг респавна врагов: задержка респавна и максимальное количество врагов на сцене.
