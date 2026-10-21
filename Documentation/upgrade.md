# Upgrade guide

## 10.2.0 → 10.3.0

### Breaking changes

- **`Mappa.Dependency.Bson` / `Mappa.Dependency.Protobuf` packages removed**
  - The NuGet packages `Mappa.Dependency.Bson`, `Mappa.Dependency.Protobuf`, and their `.DependencyInjection` companions are no longer published.
  - Reference [MongoDB.Bson](https://www.nuget.org/packages/MongoDB.Bson/) and/or [Google.Protobuf](https://www.nuget.org/packages/Google.Protobuf/) directly, then enable `BsonWellKnownTypes` / `ProtobufWellKnownTypes` on `[MappaSettings]` (or via `.editorconfig`).
  - Remove `[MappaDependency]` fields that pointed at `MappaBsonMapper` / `MappaProtobufMapper`, and stop calling `RegisterMappaBson` / `RegisterMappaProtobuf`.
  - Well-known mappings are **opt-in** (default off). See [tutorial — Bson and Protobuf well-known types](./tutorial.md#bson-and-protobuf-well-known-types) and [algorithm §4b / §4c](./mappa-generator-algorithm.md#4b-bson-well-known-types-strategy).

- **Deterministic selection among multiple collection/dictionary interface implementations**
  - When a type implements more than one closed version of the same open interface family (for example both `IEnumerable<int>` and `IEnumerable<string>`), Mappa no longer uses an unspecified first-match walk of `AllInterfaces` for element / key / value typing.
  - Selection order is: matching `[MappaSourcePreferredInterfaceImplementation]` / `[MappaTargetPreferredInterfaceImplementation]` attribute (when usable) → unique usable **implicit** implementation → unique usable **explicit** implementation → fall-through.
  - Generated `foreach` casts, `Add` / indexer access, and nested get-only collection property population follow the resolved closed interface.
  - Mappings that previously depended on incidental first-match order may change which element or key/value types (and which interface cast) are used. Add an explicit preference attribute when you need a specific closed interface.
  - See [Preferred interface implementations](./mappa-attributes.md#preferred-interface-implementations), [algorithm — container strategy](./mappa-generator-algorithm.md#7-container-strategy), and diagnostics **MP00079–MP00085**.

## 10.1.0 → 10.2.0

### Breaking changes

- **Relaxed nullability matching for existing map methods (nested mappings)**
  - When `#nullable enable` is active, nested mappings may now reuse an existing map method on the mapper, a dependency, or a matching polymorphic method even when nullability annotations differ slightly, as long as the underlying types match.
  - Exact nullability matches are still preferred. A relaxed match is used only when no exact match exists.
  - Supported relaxations:
    - Nested mapping needs `TTarget?` → may invoke a method returning `TTarget`.
    - Nested mapping needs `TSource` → may invoke a method accepting `TSource?`.
  - Previously, a nullability mismatch caused the generator to skip the existing method and generate inline mapping (or select another strategy) instead. Review nested mappings that relied on the old behaviour.

## 10.0.0 → 10.1.0

### Breaking changes

- **`MappaSettings.ForceCaseInsensitivePropertyMap` renamed to `CaseInsensitivePropertyMap`**
  - Update attribute usage: `[MappaSettings(CaseInsensitivePropertyMap = BooleanSetting.Enable)]`
  - Update `.editorconfig`: `mappa.caseinsensitivepropertymap = enable`