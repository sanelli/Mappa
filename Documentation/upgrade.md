# Upgrade guide

## 10.0.0 → 10.1.0

### Breaking changes

- **`MappaSettings.ForceCaseInsensitivePropertyMap` renamed to `CaseInsensitivePropertyMap`**
  - Update attribute usage: `[MappaSettings(CaseInsensitivePropertyMap = BooleanSetting.Enable)]`
  - Update `.editorconfig`: `mappa.caseinsensitivepropertymap = enable`
  - Legacy editorconfig key `mappa.forcecaseinsensitivepropertymap` is still supported in 10.1.0