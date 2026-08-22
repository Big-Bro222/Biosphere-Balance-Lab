# Decision Log

## 2026-08-22 - Unity 6000.5 project upgrade

Decision:
- Update the Unity project baseline to Unity `6000.5.9f1`.
- Keep `com.unity.feature.ecs` at `1.0.0` while accepting the Unity 6000.5 lock-file updates to Entities, Entities Graphics, and Unity Physics `6.5.0`.
- Update the package snapshot to URP `17.5.0`, Input System `1.20.0`, Unity Test Framework `1.7.0`, and UGUI `2.5.0`.

Reason:
- The repository should document the Unity version that actually opens the project.
- Package versions should be tracked from `unity/Packages/manifest.json` and `unity/Packages/packages-lock.json` so future Unity, ECS, WebGPU, and CI work starts from an accurate baseline.

Impact:
- Future Unity work should use Unity `6000.5.9f1` unless another project upgrade is recorded.
- Documentation that describes the current stack should stay synchronized with `unity/ProjectSettings/ProjectVersion.txt` and the package lock file.

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
