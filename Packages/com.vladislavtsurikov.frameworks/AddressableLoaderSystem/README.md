# AddressableLoaderSystem

`AddressableLoaderSystem` is a modular runtime framework for loading and unloading Unity Addressables resources by filters and rules.

This module is part of the broader Addressable Tools ecosystem:
- `AddressableLoaderSystem` - runtime loading framework.
- `AddressablesEditorTools` - editor utilities for managing AddressableAssetSettings.
- `AddressableGroupGenerator` - automatic group generation and asset structuring.

## Key Features

- Automatic resource loading with attribute-based filtering;
- Automatic unloading when loader context changes or loader is disposed;
- DI integration (Zenject, VContainer-style workflows);
- Automatic configuration loading and optional DI binding;
- Reflection-based loading of `AssetReference` fields from `ScriptableObject` and `GameObject`;
- Built-in handle tracking and asset release tools;
- Debug-friendly logging symbols for load/unload pipelines.

## How It Works

The system centers around `ResourceLoader` classes and filter attributes.

### Attributes

- `FilterAttribute` - base filter context;
- `SceneFilterAttribute` - scene-scoped context;
- `GlobalFilterAttribute` - shared/global context;
- `IgnoreResourceAutoload` - skips recursive auto-loading for a field.

Filters are resolved into labels through `AddressableLabelMapAsset` + `AddressableLabelResolver`.

### ResourceLoader Lifecycle

Each loader:
- gets registered through `ResourceLoaderManager` (or `StandaloneResourceLoaderRegistrar`);
- is selected by filter predicate in `ResourceLoaderManager.Load(...)`;
- loads assets in `LoadResourceLoader(...)`;
- unloads in `Unload(...)` and releases unused assets through `AddressableAssetTracker`.

### Zenject and DI

For DI workflows:
- `BindableResourceLoader` extends `ResourceLoader`;
- `LoadAndBind<T>()` loads and immediately binds into `DiContainer`;
- `BindableResourceLoaderRegistrar` helps register and initialize bindable loaders.

This allows direct `[Inject]` usage in runtime objects without manual boilerplate bindings.

### AssetReference Loading via Reflection

`AssetReferenceReflectionLoader`:
- recursively scans fields for `AssetReference`;
- supports nested objects, collections, and dictionaries;
- loads discovered references and tracks them by owner loader;
- prevents endless recursion via visited-object and max-depth checks.

## Logging and Debugging

Optional define symbols:
- `ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES` - enables Addressables implementation;
- `ADDRESSABLE_LOADER_SYSTEM_ZENJECT` - enables Zenject integration;
- `ADDRESSABLE_LOADER_LOGS` / `ADDRESSABLE_LOADER_LOGS_VERBOSE` - verbose runtime logs.

## Test Scene (Repository-level)

In the standalone Addressable Tools repository, a debug test scene can be used as integration validation instead of strict unit tests. Typical scenarios:
- dictionary-based references;
- nested configuration graphs;
- reuse of already loaded configs;
- cross-scene resource reuse and release;
- DI binding verification and step-by-step logs.

## Repository Structure (Addressable Tools)

```text
Assets/VladislavTsurikov/
|
|- AddressableLoaderSystem/       # Main framework for runtime resource loading
|- AddressablesEditorTools/       # Editor utilities for Addressables
`- AddressableGroupGenerator/     # Automated group and label assignment
```

## Dependencies

- Unity 2022.3
- Addressables
- Zenject (optional, for DI integration layer)

## License

MIT License
