# План переписывания UI-архитектуры

## Контекст

После просмотра `C:\Users\Vlad\ShooterUpgradePrototype\Packages\com.vladislavtsurikov.frameworks\UISystem\Tests` стало понятно, что целевой принцип должен быть ближе к тестам `UISystem`, а не к усложненной схеме с отдельным `ViewModel`.

Главные ориентиры из тестов:

- `MissionsHUDButtonView + UIMissionsHUDButtonHandler`
- `MainMissionsWindowView + UIMissionsMainWindowHandler`
- `UIMissionToggleView + UIMissionTogglePresenter`
- `MissionWindowView + ChapterMissionsWindowHandler`

Там используется простой и понятный принцип:

- `View` хранит ссылки на свои UI-элементы;
- `Handler` работает с одной `View`;
- повторяющиеся части выносятся в отдельные `View + Handler`;
- навигация идет через `UINavigator`;
- отдельный `ViewModel` слой не вводится.

## Проверка текущего `UISystem`: поддержит ли он child presenter-ы для HUD

Да, текущая реализация `UISystem` это позволяет.

Что это подтверждает:

- `ButtonUIToolkitHandler` резолвит `GetUIComponent(...)` через `Parent?.GetType() ?? GetType()`
- `UIHandler.Show(...)` после показа родителя вызывает `InitializeChildren(...)`
- значит child handler без собственного loader может инициализироваться поверх уже загруженного parent layout

Практический вывод:

- `BattleHUDRootHandler` может только загрузить HUD layout;
- `PlayerHealthHUDHandler` может быть child presenter-ом над уже загруженным label;
- `OpenUpgradeHUDButtonHandler` может быть child presenter-ом над уже загруженной кнопкой.

Ограничение текушего runtime:

- для кнопки готовый базовый класс уже есть: `ButtonUIToolkitHandler`;
- для не-кнопочного presenter-а вроде `PlayerHealthHUDHandler` готового аналога нет;
- поэтому нужен маленький локальный базовый класс по образцу `ButtonUIToolkitHandler`.

То есть пакет переписывать не нужно. Текущий `UISystem` это уже поддерживает.

## Название локального базового handler-а

Наш локальный аналог `ButtonUIHandler` не должен называться через `Button`, потому что он будет использоваться не только для кнопок.

Целевое имя:

- `ParentBoundUIToolkitHandler`

Смысл названия:

- это handler, который резолвит bindable элементы в области parent handler-а;
- он подходит и для label presenter-ов, и для button presenter-ов, и для других маленьких feature presenter-ов поверх уже загруженного layout.

Что он должен делать:

- наследоваться от `ComponentBindingUIToolkitHandler`;
- переопределять `GetUIComponent(...)`;
- резолвить binding через `Parent?.GetType() ?? GetType()`.

## Главная проблема текущей реализации

Сейчас `Handler` знает внутренние raw-поля интерфейса:

- `Button`
- `Label`
- `VisualElement`
- backdrop
- modal panel
- rows container

И сам же:

- рендерит список;
- строит сегменты;
- управляет листом строк;
- делает прямой lookup handler-а для открытия окна.

Это плохая граница ответственности.

Presenter должен знать `View`, а не внутренние leaf-элементы `View`.

## Как это сделано в `UISystem/Tests`

## 1. Один `Handler` работает с одной `View`

Примеры:

- `UIMissionsHUDButtonHandler` работает с `MissionsHUDButtonView`
- `UIMissionsMainWindowHandler` работает с `MainMissionsWindowView`

Внутри handler не хранится куча разрозненных UI-полей. Он держит ссылку на конкретную `View`.

## 2. `View` тонкая и практичная

`View` в тестах:

- содержит serialized/UI references;
- дает `OnClicked` или другие события;
- дает простые методы вида:
  - `SetActive(...)`
  - `SetRedCircleAmount(...)`
  - `SetActiveRedCircle(...)`
- иногда отдает семантический контейнер:
  - `MissionSpawnRect`

То есть `View` не строит отдельный `ViewModel`, а просто дает presenter-у понятный API.

## 3. Повторяющиеся элементы оформляются отдельными presenters, если runtime это позволяет

Хороший пример:

- `UIMissionsMainWindowHandler` отвечает за главное окно;
- `ChapterMissionTogglePresenter`, `DailyMissionTogglePresenter`, `ProgressMissionTogglePresenter`
  отвечают каждый за свой toggle.

Для нашего окна улучшений это означает:

- не делать один перегруженный `UpgradeWindowHandler`;
- при первой возможности вынести строку характеристики в отдельный `UpgradeStatRowHandler`.

## 4. Parent handler может отдавать детям семантический контейнер

Пример:

- дочерние mission handlers используют `mainWindowHandler.View.MissionSpawnRect`

Для нашего случая это значит:

- `UpgradeWindowHandler` может держать ссылку на `UpgradeWindowView`;
- row handlers могут спавниться в контейнер, который возвращает `UpgradeWindowView`.

## Целевой принцип для нашего UI

## Базовое правило

Для каждого meaningful UI-блока:

1. `View` владеет своей разметкой и внутренними ссылками.
2. `Handler` владеет orchestration-логикой.
3. Повторяющиеся части выносятся в отдельные `View + Handler`.

## Отдельный `ViewModel` слой не нужен

В этом кейсе `ViewModel` только утяжелит архитектуру.

Достаточно:

- простых методов на `View`;
- простых параметров в `Bind(...)`;
- отдельных child presenters для повторяющихся строк.

## Ограничение текущего `UIHandlerManager`

После дополнительной проверки runtime выяснилось следующее:

- `UIHandlerManager` строит дерево handler-ов по типам;
- на каждый тип handler-а создается только один экземпляр;
- повторяющиеся runtime-строки окна нельзя оформить как несколько экземпляров одного `UpgradeStatRowHandler` без расширения самого `UISystem`.

Практический вывод:

- для HUD текущий runtime подходит полностью;
- для окна улучшений мы можем сразу привести к правильной границе `Handler -> View`;
- повторяющиеся строки пока остаются частью `UpgradeWindowView` и orchestration внутри `UpgradeWindowHandler`;
- `UpgradeStatRowHandler` остается целевым следующим этапом после доработки `UISystem`.

## Целевая структура файлов на текущем этапе

- `Assets/Assemblies/ShooterUpgradePrototype/Runtime/UI/UISystem/Views/BattleHUDView.cs`
- `Assets/Assemblies/ShooterUpgradePrototype/Runtime/UI/UISystem/Views/PlayerHealthHUDView.cs`
- `Assets/Assemblies/ShooterUpgradePrototype/Runtime/UI/UISystem/Views/OpenUpgradeHUDButtonView.cs`
- `Assets/Assemblies/ShooterUpgradePrototype/Runtime/UI/UISystem/Views/UpgradeWindowView.cs`
- `Assets/Assemblies/ShooterUpgradePrototype/Runtime/UI/UISystem/Views/UpgradeStatRowView.cs`
- `Assets/Assemblies/ShooterUpgradePrototype/Runtime/UI/UISystem/Handlers/BattleHUDRootHandler.cs`
- `Assets/Assemblies/ShooterUpgradePrototype/Runtime/UI/UISystem/Handlers/PlayerHealthHUDHandler.cs`
- `Assets/Assemblies/ShooterUpgradePrototype/Runtime/UI/UISystem/Handlers/OpenUpgradeHUDButtonHandler.cs`
- `Assets/Assemblies/ShooterUpgradePrototype/Runtime/UI/UISystem/Handlers/UpgradeWindowHandler.cs`

## Контракты `View`

## 1. `BattleHUDView`

Базовый тип:

```csharp
public sealed class BattleHUDView : VisualElement, IBindableUIElement
```

Это root view HUD, а не feature view.

Что должно быть внутри `View`:

- root контейнер HUD;
- при необходимости семантические контейнеры для child presenter-ов.

Что должно выйти наружу:

```csharp
string BindingId => "BattleHUDView";
VisualElement Element => this;

VisualElement GetLeftContainer();
VisualElement GetRightContainer();
```

Сам root handler не должен знать внутренние `Button` и `Label`.

## 2. `PlayerHealthHUDView`

Базовый тип:

```csharp
public sealed class PlayerHealthHUDView : VisualElement, IBindableUIElement
```

Что должно быть внутри `View`:

- label здоровья.

Что должно выйти наружу:

```csharp
string BindingId => "PlayerHealthHUDView";
VisualElement Element => this;

void SetHealthText(string text);
```

## 3. `OpenUpgradeHUDButtonView`

Базовый тип:

```csharp
public sealed class OpenUpgradeHUDButtonView : VisualElement, IBindableUIElement
```

Что должно быть внутри `View`:

- кнопка `Upgrade`.

Что должно выйти наружу:

```csharp
string BindingId => "OpenUpgradeHUDButtonView";
VisualElement Element => this;

IObservable<Unit> OnClicked { get; }
```

## 4. `UpgradeWindowView`

Базовый тип:

```csharp
public sealed class UpgradeWindowView : VisualElement, IBindableUIElement
```

Что должно быть внутри `View`:

- кнопка `Apply`;
- кнопка `Close`;
- backdrop;
- label очков;
- container строк.

Что должно выйти наружу:

```csharp
string BindingId => "UpgradeWindowView";
VisualElement Element => this;

IObservable<Unit> OnApplyClicked { get; }
IObservable<Unit> OnCloseClicked { get; }
IObservable<Unit> OnBackdropClicked { get; }

void SetAvailablePointsText(string text);
void SetApplyEnabled(bool enabled);
VisualElement GetRowsContainer();
```

Важно:

- список как контейнер принадлежит `View`;
- raw ссылка на container не хранится в handler как leaf-поле;
- handler получает контейнер только через семантический API `View`.

## 5. `UpgradeStatRowView`

Базовый тип:

```csharp
public sealed class UpgradeStatRowView : VisualElement, IBindableUIElement
```

Что должно быть внутри `View`:

- label названия стата;
- label pending delta;
- container сегментов;
- кнопка `+`.

Что должно выйти наружу:

```csharp
string BindingId => "UpgradeStatRowView";
VisualElement Element => this;

IObservable<Unit> OnUpgradeClicked { get; }

void SetTitle(string text);
void SetPendingDelta(string text, bool visible);
void SetUpgradeEnabled(bool enabled);
void SetLevel(int currentLevel, int maxLevel);
```

Сегменты уровня рисует сама строка.

## Контракты `Handler`

## 1. `BattleHUDRootHandler`

Это прямой аналог `HUDScene1Handler`.

Должен делать только:

- загрузить layout HUD;
- при необходимости держать ссылку на `BattleHUDView`;
- не содержать feature-логики.

То есть в нем не должно быть:

- логики HP;
- логики кнопки `Upgrade`;
- подписок на gameplay state.

## 2. `PlayerHealthHUDHandler`

Это child presenter поверх уже загруженного HUD.

Должен делать только:

- получить `PlayerHealthHUDView`;
- подписаться на `PlayerUpgradeService.CurrentHealth/MaxHealth`;
- обновлять текст `HP: current / max`.

Не должен:

- загружать layout;
- знать про кнопку `Upgrade`;
- открывать окна.

## 3. `OpenUpgradeHUDButtonHandler`

Это child presenter поверх уже загруженного HUD.

Должен делать только:

- получить `OpenUpgradeHUDButtonView`;
- подписаться на `view.OnClicked`;
- открыть окно через `UINavigator`.

Целевой вызов:

```csharp
await UINavigator.Show<UpgradeWindowHandler, UIToolkitScreens>(cancellationToken);
```

Не должен:

- загружать HUD;
- знать про HP label;
- делать прямой `FindHandler`.

## 4. `UpgradeWindowHandler`

Должен делать только:

- получить `UpgradeWindowView`;
- создать `draft state`;
- обновлять очки и доступность `Apply`;
- создавать или обновлять `UpgradeStatRowView`;
- применять или отменять `draft`;
- скрывать окно через `UINavigator`.

Не должен:

- сам искать элементы строки;
- сам строить сегменты;
- сам подписывать кнопку `+` у каждой строки;
- сам владеть list rendering как leaf-логикой.

## 5. `UpgradeStatRowHandler` как следующий этап

Это остается целевым presenter-ом для одной строки характеристики, но не для текущего refactoring pass.

Его имеет смысл вводить только после доработки `UISystem`, чтобы framework умел создавать несколько экземпляров одного handler-а для списка.

## Использование `UXML`

Root-ноды `UXML` должны стать кастомными `View`, а не анонимными контейнерами.

Целевой вариант:

- `BattleHUD.uxml` root -> `BattleHUDView`
- внутри него отдельные bindable view:
  - `PlayerHealthHUDView`
  - `OpenUpgradeHUDButtonView`
- `UpgradeWindow.uxml` root -> `UpgradeWindowView`
- `UpgradeStatRow.uxml` root -> `UpgradeStatRowView`

Тогда binder будет резолвить сам `View`, а не множество leaf-элементов.

## Как работает биндинг в `UnityUI Integration` и в нашем `UIToolkit`

## `UnityUI`

В `UnityUI Integration` binder:

- берет все `IBindableUIComponent` через `GetComponentsInChildren<IBindableUIComponent>(true)`;
- для каждого компонента строит id:
  - `HandlerType:BindingId#index`;
- биндит сам instance компонента в контейнер.

Именно поэтому `MissionsHUDButtonView` и другие `MonoBehaviour View` корректно резолвятся через `GetUIComponent<...>(...)`.

## `UIToolkit`

В `UIToolkit Integration` binder:

- обходит root `VisualElement` и все дочерние элементы;
- если элемент реализует `IBindableUIElement`, берет `BindingId`;
- если не реализует, использует `element.name`;
- строит тот же принцип id:
  - `HandlerType:BindingId#index`;
- биндит сам instance `VisualElement` в контейнер.

Практический вывод:

- наши `View` действительно будут биндиться;
- но только если они являются реальными элементами в visual tree;
- wrapper-объект рядом с деревом не подойдет;
- `BindingId` должен быть задан на самом `VisualElement`, который живет в `UXML` или создается в дереве.

То есть для нас корректный путь такой:

- `PlayerHealthHUDView : VisualElement, IBindableUIElement`
- `OpenUpgradeHUDButtonView : VisualElement, IBindableUIElement`
- `UpgradeWindowView : VisualElement, IBindableUIElement`
- `UpgradeStatRowView : VisualElement, IBindableUIElement`

И все они должны существовать как реальные элементы в загруженном layout.

## Правило навигации

Навигация только через `UINavigator`.

Разрешено:

```csharp
await UINavigator.Show<UpgradeWindowHandler, UIToolkitScreens>(cancellationToken);
await UINavigator.Hide<UpgradeWindowHandler, UIToolkitScreens>(cancellationToken);
```

Не разрешено:

- `UIHandlerUtility.FindHandler(...)` внутри feature presenter-ов;
- ручной `Show()` после прямого lookup.

## Пошаговый план переписывания

## Шаг 1. Заморозить текущий direct-field подход

Не продолжать улучшать текущие handler-ы, которые знают внутренние поля разметки.

## Шаг 2. Ввести `BattleHUDView`

Сделать root `View` HUD без feature-логики.

Это должен быть контейнер для child presenter-ов, а не presenter-класс в disguise.

## Шаг 3. Ввести `PlayerHealthHUDView` и `OpenUpgradeHUDButtonView`

Разделить HUD на две независимые части по скриншоту:

- левый блок HP;
- правый блок `Upgrade`.

Именно под них сделать два отдельных child presenter-а.

## Шаг 4. Ввести `ParentBoundUIToolkitHandler`

Добавить локальный базовый handler для маленьких presenter-ов над уже загруженным родительским layout.

Он должен заменить идею переиспользования `ButtonUIToolkitHandler` для всего подряд.

## Шаг 5. Ввести `UpgradeWindowView`

Сделать окно, которое:

- само знает `Apply`, `Close`, `Backdrop`, `RowsContainer`;
- наружу отдает только нужные события и методы;
- возвращает контейнер строк через `GetRowsContainer()`.

## Шаг 6. Ввести `UpgradeStatRowView`

Сделать отдельную `View` для одной строки:

- название;
- pending;
- сегменты;
- кнопка `+`.

## Шаг 7. Ввести `BattleHUDRootHandler`, `PlayerHealthHUDHandler`, `OpenUpgradeHUDButtonHandler`

Сделать HUD по принципу:

- `BattleHUDRootHandler` только грузит layout;
- `PlayerHealthHUDHandler` работает только с HP;
- `OpenUpgradeHUDButtonHandler` работает только с кнопкой.

Это прямое повторение идеи:

- `HUDScene1Handler`
- `UIMissionsHUDButtonHandler`

## Шаг 8. Переписать `UpgradeWindowHandler`

Свести его к orchestration:

- `draft/apply/cancel`;
- работа только через `UpgradeWindowView` и `UpgradeStatRowView`;
- `UINavigator.Hide`.

## Шаг 9. Подготовить окно к будущему `UpgradeStatRowHandler`

Сейчас нужно:

- убрать raw leaf-поля из `UpgradeWindowHandler`;
- оставить генерацию строк во `View`;
- пометить TODO на переход к отдельным row presenter-ам после расширения `UISystem`.

## Шаг 10. Удалить старый подход

После перехода удалить:

- raw поля UI из handler-ов;
- ручной рендер строк в окне;
- прямой lookup handler-а;
- ручные callback registration flags для leaf-элементов.

## Критерии готовности

Переписывание считается завершенным, когда:

- `BattleHUDRootHandler` только загружает HUD;
- `PlayerHealthHUDHandler` отвечает только за HP;
- `OpenUpgradeHUDButtonHandler` отвечает только за кнопку `Upgrade`;
- `UpgradeWindowHandler` работает только через `UpgradeWindowView`;
- строка характеристики выделена как минимум в `UpgradeStatRowView`;
- контейнер списка принадлежит `UpgradeWindowView`;
- сегменты рисуются в `UpgradeStatRowView`;
- навигация идет через `UINavigator`;
- `Handler` больше не хранит внутренние leaf-поля разметки;
- архитектура по духу повторяет принцип `UISystem/Tests`;
- в коде есть явный TODO на будущий `UpgradeStatRowHandler`, когда runtime начнет поддерживать повторяющиеся handler-ы.

## Что этот rewrite не меняет

Этот план не меняет:

- gameplay;
- `PlayerUpgradeService`;
- bootstrap;
- save system;
- progression logic.

Это только план переписывания UI-архитектуры.
