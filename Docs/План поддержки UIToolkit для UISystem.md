# План полноценной поддержки UIToolkit в UISystem

Дата: 2026-03-11
Проект: ShooterUpgradePrototype

## 1. Краткий вывод по текущему состоянию

Сейчас `UISystem` можно использовать с `UIToolkit` только на уровне логики навигации и графа `UIHandler`, но не как полноценную визуальную систему.

Что уже можно переиспользовать без переписывания:
- дерево `UIHandler` и связи parent/child;
- жизненный цикл `Initialize -> Show -> Hide -> Destroy`;
- фильтрацию через `FilterAttribute` и сценовую композицию;
- навигацию через `UINavigator`;
- регистрацию хендлеров в Zenject.

Что жёстко завязано на `Unity UI` и мешает `UIToolkit`:
- визуальный слой опирается на `GameObject`, `Transform`, `MonoBehaviour` и prefab-spawn;
- биндинг компонентов работает через `GetComponentsInChildren<IBindableUIComponent>(true)`;
- `GetUIComponent<T>()` ограничен `where T : MonoBehaviour`;
- загрузчик ресурсов рассчитан именно на prefab-адреса;
- `ButtonUIHandler` и view-компоненты ожидают `UnityEngine.UI.Button`, `RectTransform`, `TextMeshProUGUI` и похожие классы.

Итог: ядро системы уже годится как platform-agnostic orchestration layer, но адаптер `UIToolkit` сейчас в пакете отсутствует.

## 2. Подтверждение по коду

### Платформенно-нейтральная часть
- `Runtime/Core/UIHandler.cs` управляет жизненным циклом и children, не зависит от `Canvas` или `VisualElement`.
- `Runtime/Core/UIHandlerManager.cs` строит дерево хендлеров по `NodeTreeAsset` и фильтрам, также не зависит от конкретной UI-технологии.

### Жёсткая привязка к Unity UI
- `Runtime/UnityUIIntegration/UnityUIHandler.cs` хранит `SpawnedParentPrefab : GameObject`, показывает/скрывает его через `SetActive`, а создаёт через `PrefabResourceLoader`.
- `Runtime/UnityUIIntegration/ComponentBindingUIHandler.cs` возвращает только `MonoBehaviour`-компоненты.
- `Runtime/UnityUIIntegration/BindableUIComponentSystem/UIComponentBinder.cs` ищет биндимые view через `GetComponentsInChildren` на `GameObject`.
- `Runtime/AddressableLoaderSystemIntegration/PrefabResourceLoader.cs` и `PrefabLoaderExtensions.cs` загружают и инстанцируют prefab.

### Что уже есть в проекте по UIToolkit
В проекте уже есть отдельный работающий паттерн для `UIToolkit` через `EntityDataAction`:
- `Packages/com.vladislavtsurikov.frameworks/EntityDataAction/Runtime/UIToolkitIntegration/UIToolkitEntityHost.cs`
- `Assets/Assemblies/AutoStrike/Runtime/UI/VisualElements/AutoStrikeBattleUIViewHost.cs`
- `Assets/Assemblies/AutoStrike/Runtime/UI/Entities/AutoStrikeBattleUIToolkitEntity.cs`

Это важно: нужные базовые знания и примеры по `UIDocument`/`VisualElement` в проекте уже есть, но они не интегрированы с `UISystem`.

## 3. Рекомендованная стратегия

Рекомендация: не пытаться «дотянуть» текущий `UnityUIIntegration` до `UIToolkit` условными `if` и перегрузками. Нужно выделить общий contract для визуального backend и сделать два независимых адаптера:
- `UnityUIIntegration` для prefab/canvas UI;
- `UIToolkitIntegration` для `UIDocument`/`VisualTreeAsset`/`VisualElement`.

Почему так:
- иначе generic API останется завязан на `MonoBehaviour` и `GameObject`;
- смешанный адаптер быстро превратится в слой `if (unityUi) else if (uiToolkit)`;
- тестировать и расширять два отдельных backend-а проще, чем поддерживать один гибридный.

## 4. Целевая архитектура

### 4.1. Оставить без изменений
Эти части должны остаться основой системы:
- `UIHandler`
- `UIHandlerManager`
- `NodeTreeAsset`
- `FilterAttribute` / `ParentUIHandlerAttribute`
- `UINavigator`
- логика активных детей (`ChildActivityTracker`, `SingleActiveUIChildSwitcher`)

### 4.2. Вынести новый общий runtime-контракт
Нужно добавить абстракции уровня представления, не завязанные на `GameObject`:
- `IUIViewHandle`
  - `object NativeRoot { get; }`
  - `void SetVisible(bool visible)`
  - `UniTask DisposeAsync(bool unload, CancellationToken ct)`
- `IUIComponentResolver`
  - `T Resolve<T>(string bindingId, int index = 0)`
  - `bool TryResolve<T>(string bindingId, out T value, int index = 0)`
- `IUIViewSpawner<TViewHandle>` или `IUIViewLoader<TViewHandle>`
  - создаёт представление для конкретного backend-а

Смысл: `UIHandler` и high-level handler-классы должны работать не с `GameObject`, а с абстрактным view handle.

### 4.3. Перестроить базовые хендлеры
Вместо текущей цепочки:
- `DiContainerUIHandler`
- `ComponentBindingUIHandler`
- `ChildSpawningUIHandler`
- `UnityUIHandler`

сделать:
- `DiContainerUIHandler` или аналог DI-базиса
- `ViewBindingUIHandler<TViewHandle>`
- `SpawnedUIHandler<TViewHandle>`
- конкретные адаптеры:
  - `UnityUIHandler`
  - `UIToolkitUIHandler`

`ViewBindingUIHandler<TViewHandle>` должен опираться на интерфейс resolver-а, а не на `MonoBehaviour`.

### 4.4. Новый binder для UIToolkit
Нужен отдельный binder, например:
- `UIToolkitElementBinder`
- `IBindableVisualElement` либо metadata-based binding через `name`/`viewDataKey`

Рекомендую не создавать wrapper-компонент на каждый `VisualElement`, а использовать lookup по имени и typed-queries:
- `root.Q<Button>("CloseButton")`
- `root.Q<Label>("KillsLabel")`
- `root.Q<VisualElement>("WindowRoot")`

Причина: в `UIToolkit` binding естественнее строить на `VisualElement` tree, а не на `MonoBehaviour`-обёртках.

### 4.5. Новый loader/spawner для UIToolkit
Нужен отдельный ресурсный слой, например:
- `UIToolkitDocumentLoader` для готового prefab с `UIDocument`;
- или `UIToolkitLayoutLoader` для `VisualTreeAsset + PanelSettings + host UIDocument`.

Рекомендованный путь для тестового задания:
- использовать `UIDocument` как host в сцене;
- грузить `VisualTreeAsset` и клонировать его в `rootVisualElement`;
- для top-level экранов держать отдельные document/root containers;
- для child screen использовать вставку subtree в заранее определённый parent `VisualElement`.

Такой подход ближе к `UIToolkit` и проще в управлении, чем спавн prefab-объектов ради каждого окна.

## 5. Минимальный набор новых классов

### Runtime/UIToolkitIntegration
- `UIToolkitUIHandler`
- `UIToolkitButtonUIHandler` или отказаться от отдельного button-handler класса
- `UIToolkitViewHandle`
- `UIToolkitElementResolver`
- `UIToolkitBindingId`
- `UIToolkitLayoutLoader`
- `UIToolkitSpawnOperation`
- `IUIToolkitBindableElement` если останется binding через контракты

### Runtime/UIToolkitIntegration/Views
- `UIToolkitViewContext`
- `UIToolkitViewContainer`
- `UIToolkitParentLocator`

### Runtime/UIToolkitIntegration/Utility
- query-утилиты для `VisualElement`
- конвертеры событий `Clickable` / `RegisterCallback<ClickEvent>` в `IObservable<Unit>` или R3 stream
- утилиты показа/скрытия через `style.display`, `visible`, css-class toggling

## 6. Предлагаемые API-решения

### Вариант A. Унифицированный API через generic resolver
Пример идеи:

```csharp
public abstract class ViewBindingUIHandler<TViewRoot> : DiContainerUIHandler
{
    protected IUIComponentResolver Resolver { get; private set; }
    protected TViewRoot ViewRoot { get; private set; }

    protected T Resolve<T>(string bindingId, int index = 0) =>
        Resolver.Resolve<T>(bindingId, index);
}
```

Плюсы:
- один паттерн для Unity UI и UIToolkit;
- handler-логика становится переносимой.

Минусы:
- потребуется рефактор текущих `GetUIComponent<T>()`.

### Вариант B. Отдельный API для UIToolkit
Пример идеи:

```csharp
public abstract class UIToolkitUIHandler : DiContainerUIHandler
{
    protected VisualElement Root { get; private set; }

    protected TElement Query<TElement>(string name)
        where TElement : VisualElement;
}
```

Плюсы:
- внедряется быстрее;
- совпадает с native-стилем `UIToolkit`.

Минусы:
- `Unity UI` и `UIToolkit` будут жить в разных API.

Рекомендация: для тестового задания и первой рабочей версии выбрать вариант B, а унификацию сделать вторым этапом. Это быстрее и даст рабочий результат с меньшим риском.

## 7. Поэтапный план внедрения

### Этап 1. Подготовка архитектуры
1. Зафиксировать, что `Runtime/Core` остаётся без визуальных зависимостей.
2. Добавить папку `Runtime/UIToolkitIntegration`.
3. Выделить контракт для показа/скрытия и освобождения view.
4. Убрать из нового общего слоя ограничения `MonoBehaviour`.

Результат этапа:
- ядро продолжает работать как раньше;
- появляется точка входа для нового backend-а.

### Этап 2. Первый рабочий backend для top-level экранов
1. Сделать `UIToolkitUIHandler` для работы с `UIDocument` или `VisualTreeAsset`.
2. Реализовать show/hide через `DisplayStyle.None/Flex` или attach/detach subtree.
3. Поддержать top-level экран без вложенных children.
4. Поднять простой экран тестового задания: счётчик убийств.

Результат этапа:
- один UI-экран можно показать/скрыть через текущий `UINavigator`.

### Этап 3. Binding и события
1. Реализовать query API для `Button`, `Label`, `ProgressBar`, `VisualElement`.
2. Сделать утилиты подписки на `ClickEvent` и cleanup в `CompositeDisposable`.
3. Добавить `TryQuery` и понятные ошибки на отсутствующие элементы.
4. Поддержать повторяющиеся элементы через имя + индекс или через список query-результатов.

Результат этапа:
- хендлеры начинают управлять `UIToolkit`-элементами почти так же, как сейчас управляют `MonoBehaviour` view.

### Этап 4. Parent/child композиция
1. Добавить способ указать контейнер для потомка внутри родителя.
2. Решить mapping `ParentUIHandler -> target VisualElement`.
3. Сделать вставку child-layout в конкретный container.
4. Проверить `AllowMultipleActiveChildren = false` на `UIToolkit`-экранах.

Результат этапа:
- можно строить окна, HUD-элементы и вложенные панели через существующее дерево `UIHandler`.

### Этап 5. Ресурсная загрузка
1. Решить, что именно грузится адресаблами:
   - `VisualTreeAsset`;
   - prefab с `UIDocument`;
   - или набор `VisualTreeAsset + StyleSheet + PanelSettings`.
2. Сделать новый loader под выбранный формат.
3. Обеспечить корректный unload без утечек ссылок на `VisualElement`.
4. Проверить повторное открытие/закрытие экранов.

Результат этапа:
- `UIToolkit`-экраны начинают работать на тех же сценовых фильтрах и ресурсных правилах, что и текущий UI.

### Этап 6. Совместимость двух backend-ов
1. Разрешить проекту иметь одновременно `UnityUIHandler` и `UIToolkitUIHandler`.
2. Проверить, что `UIHandlerManager` одинаково регистрирует оба типа.
3. Добавить smoke-тесты на coexistence в одной сцене.
4. Зафиксировать ограничения: например, child одного backend-а нельзя вставлять в container другого backend-а без bridge-слоя.

Результат этапа:
- можно мигрировать UI по частям, а не одним большим переписыванием.

## 8. Что делать в рамках именно тестового задания

Для тестового задания не нужен сразу весь универсальный framework. Рациональный объём такой:

1. Оставить текущий `UISystem` core как есть.
2. Сделать минимальный `UIToolkitUIHandler` для top-level panel.
3. Сделать один `UIDocument` + `UXML` для HUD.
4. Через handler обновлять `Label` со счётчиком убийств.
5. Если нужен popup или screen, реализовать ещё один top-level screen без вложенных child-контейнеров.

Этого достаточно, чтобы:
- показать умение работать с `UIToolkit`;
- не раздувать объём тестового задания;
- не переписывать весь пакет преждевременно.

## 9. Основные риски

### Риск 1. Слишком ранняя унификация API
Если сразу делать полностью общий API для `MonoBehaviour` и `VisualElement`, можно утонуть в абстракциях и не получить рабочий UI.

Митигирование:
- сначала рабочий `UIToolkit` backend с отдельным API;
- потом выносить общее только из реально дублирующегося кода.

### Риск 2. Попытка хранить VisualElement в Zenject так же, как MonoBehaviour
Это можно сделать, но при агрессивном bind/unbind легко получить хрупкие зависимости и проблемы жизненного цикла.

Митигирование:
- держать `VisualElement`-resolution локально в handler через resolver/query слой;
- в контейнер класть только сами handler/service объекты.

### Риск 3. Неправильная модель child-вставки
Если child-экран не знает, в какой container себя монтировать, дерево `UIHandler` останется логическим, но не визуальным.

Митигирование:
- ввести явный parent-slot contract;
- для каждого вложенного handler описывать target container name.

## 10. Практическая рекомендация по следующему шагу

Следующий разумный шаг в коде:
1. создать `Runtime/UIToolkitIntegration`;
2. реализовать `UIToolkitUIHandler` без child-вставки, только для top-level `UIDocument`/`VisualTreeAsset`;
3. поднять через него HUD тестового задания с label счётчика убийств;
4. только после этого добавлять child composition и адресабельную загрузку layout-ов.

## 11. Оценка текущей применимости UISystem к UIToolkit

Если отвечать коротко: 
- как система навигации и жизненного цикла `UISystem` уже подходит;
- как система представлений под `UIToolkit` она сейчас не готова;
- для полноценной поддержки нужен отдельный runtime-backend, а не косметическая адаптация нескольких классов.

Это хорошая база для миграции, но не готовое решение из коробки.
