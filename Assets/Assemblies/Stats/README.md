# Stats Architecture

This document describes how the extensible stats architecture works in the project.

## Overview

The stats system is split into two layers:
- Definition layer (`Stats/Runtime`, assembly `VladislavTsurikov.Stats`) - what a stat is and how it is configured.
- Runtime layer (`EntityDataAction.Shared/Runtime/Stats`) - how stats are stored on entity instances and modified during gameplay.

This separation keeps stat definitions reusable, minimizes rebuild scope, and keeps runtime logic data-driven.

## Layer 1: Stat Definitions (Stats)

Path: `Packages/com.vladislavtsurikov.frameworks/Stats/Runtime`

Main types:
- `Stat` - ScriptableObject stat definition with:
  - string `Id` (unique runtime key);
  - `StatsComponentStack` (component-based stat configuration).
- `StatCollection` - list of stat definitions used as a source template for entities.
- `StatValueComponent` - default value and optional min/max clamp rules.
- `StatEffect` - list of `(Stat, Delta)` entries that can be applied to runtime stats.
- `StatModifier` - reusable list of `(Stat, Delta)` entries for modifier assets.

`StatsComponentStack` inherits `NodeStackOnlyDifferentTypes<ComponentData>`, so stat behavior can be extended with new components without changing core stat classes.

`ModifierStatEffect` stays in the main `VladislavTsurikov.ActionFlow` assembly even though it uses the `VladislavTsurikov.ActionFlow.Runtime.Stats` namespace. It is the bridge object between stat definitions and the ActionFlow modifier system, so keeping it outside the stats assembly avoids a circular assembly dependency.

## Layer 2: Runtime Stats (EntityDataAction.Shared)

Path: `Packages/com.vladislavtsurikov.frameworks/EntityDataAction.Shared/Runtime/Stats`

Main types:
- `RuntimeStat`
  - stores link to `Stat`;
  - stores current value in `ReactiveProperty<float>` for reactive subscriptions.
- `StatsEntityData`
  - stores `StatCollection`;
  - builds runtime dictionary `Dictionary<string, RuntimeStat>` by `stat.Id`;
  - applies clamp rules from `StatValueComponent` on all set/add operations;
  - exposes stat API (`SetStatValue`, `AddStatValue`, `GetStatValueById`, etc.).
- `ModifiersData`
  - stores active modifier effects in `ReactiveCollection<ModifierStatEffect>`.
- `ApplyModifierStatEffectAction`
  - listens to `ModifiersData.Effects` add/remove/reset events via UniRx;
  - rebuilds base stats from collection;
  - reapplies all active effects in deterministic order.

## Runtime Flow

1. Designer configures `Stat` assets and groups them into `StatCollection`.
2. Entity receives `StatsEntityData.Collection`.
3. `StatsEntityData.RebuildFromCollection()` creates runtime values from each stat's `StatValueComponent.BaseValue`.
4. Modifier changes are written into `ModifiersData.Effects`.
5. `ApplyModifierStatEffectAction` reacts to collection changes and recomputes final values.
6. Any subscriber to `RuntimeStat.Value` gets updates reactively.

## Why It Is Extensible

- New stat types are data assets, not hardcoded fields.
- Stat behavior is component-based (`StatsComponentStack`) instead of inheritance chains.
- Runtime operations work by stat `Id`, so new stats do not require API rewrites.
- Modifiers are composable assets (`ModifierStatEffect`) and can be added/removed at runtime.
- Reactive value storage (`ReactiveProperty<float>`) simplifies UI/gameplay sync.

## Practical Notes

- Ensure each `Stat.Id` is unique in a `StatCollection`, otherwise dictionary keys collide.
- Current modifier application strategy is "rebuild base + apply all modifiers"; this is simple and deterministic.
- Clamp logic is centralized in `StatValueComponent`, so all write paths use same min/max behavior.

## Related Files

- `Stats/Runtime/Stat.cs`
- `Stats/Runtime/StatCollection.cs`
- `Stats/Runtime/StatComponents/StatValueComponent.cs`
- `Stats/Runtime/StatEffect.cs`
- `EntityDataAction.Shared/Runtime/Stats/RuntimeStat.cs`
- `EntityDataAction.Shared/Runtime/Stats/ComponentData/StatsEntityData.cs`
- `EntityDataAction.Shared/Runtime/Stats/ComponentData/ModifiersData.cs`
- `EntityDataAction.Shared/Runtime/Stats/Actions/ApplyModifierStatEffectAction.cs`
