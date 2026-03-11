# SceneManagerTool

`SceneManagerTool` is a modular scene orchestration framework for Unity. It provides editor-driven scene profiles and runtime load/unload pipelines with operation hooks.

## What It Solves

- Centralized scene flow configuration in one profile;
- Predictable scene loading and unloading order;
- Startup collections for build-like playmode bootstrapping;
- Hook points before/after load and unload;
- Optional UI callbacks (button-driven open, fade, progress bar, prefab spawn).

## Core Architecture

- `SceneManagerData` - singleton config entry (`EnableSceneManager`, active `Profile`);
- `Profile` - stores `BuildSceneCollectionStack` and active build configuration;
- `BuildSceneCollection` - defines what scene collections exist and which are startup;
- `SceneCollection` - runtime unit that loads/unloads one logical group of scenes;
- `SceneType` - internal scene loading strategy (`Single`, `Group`, custom variants);
- `SettingsComponent` - behavior blocks attached to collection/type settings stacks.

## Load Pipeline (SceneCollection)

`SceneCollection.Load(...)` runs this order:
1. Optional fade-in for current collection;
2. Unload current collection (if any);
3. Show progress bar (if configured);
4. Run `BeforeLoadOperationsSettings`;
5. Load active scene (if configured);
6. Load each `SceneType` in collection;
7. Wait until loading progress reaches 100%;
8. Run `AfterLoadOperationsSettings`;
9. Hide progress bar;
10. Optional fade-out from previous collection.

## Unload Pipeline (SceneCollection)

`SceneCollection.Unload(...)` runs:
1. `BeforeUnloadOperationsSettings`;
2. Unload each `SceneType`;
3. Unload active scene (if configured);
4. `AfterUnloadOperationsSettings`.

## SceneType Behavior

`SceneType` supports open/close rules through `SceneBehavior`:
- `SceneOpenBehavior.DoNotOpen` skips loading unless forced;
- `SceneCloseBehavior.KeepOpenAlways` keeps scene loaded on transitions unless forced.

Each `SceneType` can also run its own before/after operation settings.

## Editor Workflow

Main window: `Window/Vladislav Tsurikov/Scene Manager`.

Typical setup:
1. Create or assign `Profile`;
2. Enable Scene Manager;
3. Configure `BuildSceneCollection` and startup collections;
4. Configure per-collection/per-type settings and operation stacks;
5. Press `Play` in Scene Manager window.

The editor utility saves scene setup and enters playmode in build-like flow (`SceneManagerEditorUtility.EnterPlaymode()`).

## Runtime Entry and API

- Auto-start path: `RuntimeInitializeOnLoad` -> `RuntimeUtility.Start()` -> loads all startup scene collections.
- Public API:
  - `SceneManagerAPI.LoadSceneCollection(...)`
  - `SceneManagerAPI.UnloadSceneCollection(...)`
  - `SceneManagerAPI.LoadSceneType(...)`
  - `SceneManagerAPI.UnloadSceneComponent(...)`
- UI helper: `OpenSceneCollectionOnButtonClick` for `Button`-driven collection switch.

## Extensibility

- Add custom `BuildSceneCollection` strategies;
- Add custom `SceneType` implementations;
- Add custom operation steps through settings stacks (`ActionCollection`);
- Add runtime callbacks via `SceneOperation` (`OnLoad`, `OnUnload`).

## Package Structure

```text
SceneManagerTool/
|- Editor/      # window, drawers, setup helpers
|- Runtime/     # core runtime flow and API
|- Examples/    # sample usage
`- Resources/   # runtime/editor assets
```

## Dependencies

- Unity 2022.3+
- UniTask
- Odin Serializer
- ActionFlow / Nody / SceneUtility (from the same frameworks package)
