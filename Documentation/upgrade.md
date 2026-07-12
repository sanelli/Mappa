# Upgrade guide

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