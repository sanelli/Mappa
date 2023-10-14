# Mappa.Generator
This source generator generates code for partial classes tagged with the `[Mappa]` attribute.
It also allow to reuse code via the `[MappaDependecy]` attribute.

## Supported native mappings
- **Constructor**
   - _Mapping constructor_: `TTarget(TSource source){ ... }`
   - _Empty constructor with set/init properties_: `TTarget(){ ... }`
   - _Constructor with parameters_: ⚠️ WORK IN PROGRESS ⚠️ 
- **Containers**
   - `TSource[]`/`List<TSource>`/`IList<TSource>` to `TTarget[]`
   - `IEnumerable<TSource>`/`ICollection<TSource>`/`IReadOnlyCollection<Tsource>` to `TTarget[]`
   - `TSource[]`/`List<TSource>`/`IList<TSource>` to `List<TSource>`/`IList<TSource>`/ `IEnumerable<TSource>`/`ICollection<TSource>`/`IReadOnlyCollection<Tsource>`
   - `IEnumerable<TSource>`/`ICollection<TSource>`/`IReadOnlyCollection<Tsource>` to `List<TSource>`/`IList<TSource>`/ `IEnumerable<TSource>`/`ICollection<TSource>`/`IReadOnlyCollection<Tsource>`
   - `IDictionary<TKSource, TVSource>`/`Dictionary<TKSource, TVSource>` to `IDictionary<TKTarget, TVTarget>`/`Dictionary<TKTarget, TVTarget>`
- **`enum` mappings**
   - `enum` to `string`
   - `enum` to `byte/short/int/long`
   - `string` to `enum`
   - `byte/short/int/long` to `enum`
   - `enum` to `enum`
- **`string` mapping**
   - `string` to `INumber`
   - `string` to `DateTime`
   - `object` to `string` (by invoking `ToString()`)
- **Tuples**
- **Nullable struct and reference types**
- **Identity mapping**
- **Classes and records**
- **Custom mapping methods**
- **Reference to other classes with generated or custom mappers**