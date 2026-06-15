# Biosphere Balance Lab Architecture

## Unity dependency injection

The Unity project uses a small in-repository dependency injection layer under
`unity/Assets/Scripts/Infrastructure/DependencyInjection`.

The container supports keyed bindings so one service interface can have multiple
implementations. Runtime code can switch the active binding with
`Switch<TService>(key)`, which is useful for scenario presets, simulation modes,
or test doubles.

Prefab references are stored in a `StorageInjection` ScriptableObject. This keeps
scene and prefab references inside Unity assets while still allowing systems and
MonoBehaviours to resolve them through `IPrefabStorage`.

ECS integration is intentionally managed-only. `InjectionScope` registers the
container against `World.DefaultGameObjectInjectionWorld`, and managed ECS
systems can inherit from `InjectedSystemBase` to resolve configuration, bridge
services, or prefab storage. Core simulation state should remain in ECS
components so Burst/job-friendly systems stay data-oriented.
