# Decision Log

## 2026-06-15 - Lightweight Unity dependency injection

Decision:
- Add a small in-repository DI container instead of introducing a third-party Unity DI framework.
- Support switchable dependencies through keyed service bindings.
- Store prefab references in a Unity `ScriptableObject` named `StorageInjection`.
- Expose managed ECS access through a `World` to container registry and `InjectedSystemBase`.

Reason:
- The project needs portfolio-friendly architecture that is easy to explain and review.
- A small DI layer is enough for scenario configuration, prefab lookup, bridge services, and test doubles.
- Keeping managed services outside ECS components preserves a clear data-oriented boundary.

Alternatives considered:
- Use a full Unity DI framework.
- Use only scene singletons.
- Store managed service references directly in ECS component data.

Impact:
- MonoBehaviours can resolve dependencies through `InjectionScope`.
- ECS `SystemBase` classes can resolve managed services when needed.
- Future simulation systems should keep high-frequency state in ECS data and use DI only for managed configuration and integration services.
