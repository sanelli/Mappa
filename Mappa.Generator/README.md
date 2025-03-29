# Mappa.Generator
This source generator generates code for partial methods in partial classes tagged with the `[Mappa]` attribute defined in the [Mappa](https://www.nuget.org/packages/Mappa/) package.
Assuming you have a partial method like the following
```csharp
[Mappa]
public partial class Mapper
{
    public partial TTarget Map(TSource input);
}
```

where `TSource` is the source type of the mapping and `TTarget` is the target type of the mapping, the source generator works by applying the following set of strategies in the order they are defined (see [TypeMapIdentifierAlgorithm.cs](https://github.com/sanelli/Mappa/blob/main/Mappa.Generator/Algorithm/TypeMapIdentifierAlgorithm.cs)):
1. <u>Identity strategy</u>:
   - _When_:
      - `TSource` and `TTarget` are the same type (e.g. `TSource => int` and `TTarget => int`) OR,
      - `TSource` can be implicitly converted into `TTarget` (e.g. `TSource => int` and `TTarget => long`);
   - _What_:
      - the input value is simply assigned to the target;
2. <u>Nullable strategy</u>:
    - _When_:
        - `TSource` is the `Nullable<T>` value type (e.g. `int?`) OR,
        - `TSource` is a `nullable` reference type when `#nullable enable` (e.g. `string?`) OR,
        - `TSource` is a reference type when `#nullable disable` (e.g. `string`);
    - _What_:
        - if `TTarget` can be `null` the mapper will return null OR,
        - if `TTarget` cannot be `null` the mapper will throw a `NullReferenceException`;
3. <u>`enum` strategy</u>:
    - _When_:
        - `TSource` is an `enum` and `TTarget` is a different `enum`, an integral numeric type compatible with the `enum` or a string OR,
        - `TSource` is a different `enum`, an integral numeric type compatible with the `enum` and `TTarget` is an `enum`;
   - _What_:
       - a `switch` statement is introduced to quickly map `TSource` to `TTarget` using all the possible values of the `enum`,
4. <u>`string` strategy</u>:
    - _When_:
        - `TSource` is a `string` and `TTarget` is any of the following types `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, `Guid`, `Uri` OR,
        - `TTarget` is a `string`;
    - _What_:
       - `TSource` is a `string` and `TTarget` is any of the following types `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, `Guid` then their `TTarget.Parse` method will be used, possibly with the format and culture identified by the `MappaSettings` attribute, if any is provided on the class or on the method;
       - `TSource` is a `string` and `TTarget` is `Uri` then the `System.UriBuilder` will be used for the mapping
       - `TTarget` is a `string` and `TSource` is any of the following types `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, `Guid` then their `TSource.ToString()` method will be used, possibly with the format and culture identified by the `MappaSettings` attribute, if any is provided on the class or on the method;
       - `TTarget` is a `string` then the `TSource.ToString` method will be used
5. <u>Date & Time strategy</u>:
   - _When_:
     - `TSource` is a `DateTime` and `TTarget` is `long` or,`DateTime` or, `TimeOnly` OR,
     - `TSource` is a `DateTimeOffset` and `TTarget` is `long` or, `DateTime` or, `DateTime` or, `TimeOnly` OR,
     - `TSource` is a `DateOnly` and `TTarget` is `long` or, `DateTime` OR,
     - `TSource` is a `long` and `TTarget` is `DateTime` or, `DateTimeOffset` OR,
     - `TSource` is a `TimeSpan` and `TTarget` is `double` OR,
     - `TSource` is a `double` and `TTarget` is `TimeSpan` OR,
   - _What_:
     - The usual mapping conversions are used;
     - When mapping from `DateOnly` to `DateTime` or `DateTimeOffset` the `TimeOnly.Zero` is used;
     - When mapping to or from `long` the Unix time is used;
     - When a timezone is required UTC is implied;
   - _Notes_:
     - The mapping from `DateTime` to `DateTimeOffset` is handled by the identify strategy;
6. <u>Container strategy</u>:
   - _When_:
     - `TSource` and `TTarget` are both either dictionaries or collections,
     - For dictionaries mappings exist from source key type to the target key type and from the source value type to the target value type,
     - For collections a mapping exist from source element type to the target element type,
     - `TSource` dictionary types accepted:
          - any type implementing `IDictionary<TKey, TValue>`;
          - any type implementing `IReadOnlyDictionary<TKey, TValue>`;
          - any type implementing `IEnumerable<KeyValuePair<<TKey, TValue>>`;
     - `TTarget` dictionary types accepted:
         - any type implementing `IDictionary<TKey, TValue>` that has a constructor with zero arguments;
         - the following interfaces: `IDictionary<TKey, TValue>`, `IReadOnlyDictionary<TKey, TValue>`, `IImmutableDictionary<TKey, TValue>`;
         - the following classes: `ImmutableDictionary<TKey, TValue>`, `ImmutableSortedDictionary<TKey, TValue>`, `FrozenDictionary<TKey, TValue>`
     - `TSource` collection types accepted: any type implementing `IEnumerable<T>`, arrays, `Span<T>`, `ReadOnlySpan<T>`, `Memory<T>` and `ReadOnlyMemory<T>`;
     - `TTarget` collection types accepted:
         - any type implementing `ICollection<T>` or `ISet<T>` that has a constructor with zero arguments;
         - any type derived from `Stack<T>` or `Queue<T>` that has a constructor with zero arguments;
         - the following interfaces: `IEnumerable<T>`, `ICollection<T>`, `IReadOnlyCollection<T>`, `ISet<T>`, `IList<T>`, `IReadOnlyList<T>`, `IReadOnlySet<T>`, `IImmutableSet<T>`, `IImmutableList<T>`, `IImmutableQueue<T>`, `IImmutableStack<T>`;
         - the following classes: arrays, `List<T>`, `ReadOnlyCollection<T>`, `Span<T>`, `ReadOnlySpan<T>`, `Memory<T>`, `ReadOnlyMemory<T>`, `Stack<T>`, `Queue<T>`, `ReadOnlySet<T>`, `HashSet<T>`, `SortedSet<T>`, `ReadOnlyColletion<T>`, `FrozenSet<T>`, `ImmutableHashSet<T>`, `ImmutableSortedSet<T>`, `ImmutableArray<T>`, `ImmutableList<T>`, `ImmutableQueue<T>`, `ImmutableStack<T>`;
   - _What_:
       - A `for` loop or `foreach` loop is added to the code;
       - In the loop each element from the source collection is mapped in an element of the target collection and then added to the target collection;
   - _Notes_:
       - When possible for some types (e.g. `List<T>`) the usage of the constructor accepting capacity is preferred to reduce the number of allocations;
       - Explicit interface implementation is supported;
7. <u>Tuples strategy</u>:

Currently unsupported features are:
- Polymorphism;
- Generics;
- Format and culture when mapping numeric types to and from strings;
- Use of `Span<T>` or `ReadOnly<T>` for fast iterations over collections;
- `ValueType<T>` tuples.

Other relevant packages:
- [Mappa](https://www.nuget.org/packages/Mappa/): source generator that allows to automatically generate mapping between classes and value types;
- [Mappa Protobuf](https://www.nuget.org/packages/Mappa.Dependency.Protobuf/): methods to map `Google.Protobuf.WellKnownTypes` objects from [Google.Protobuf](https://www.nuget.org/packages/Google.Protobuf) package into common objects.
- [Mappa Protobuf dependency](https://www.nuget.org/packages/Mappa.Dependency.Protobuf.DependencyInjection/): utility methods to register the Protobuf mapper.
- [Mappa Bson](https://www.nuget.org/packages/Mappa.Dependency.Bson/): methods to map `MongoDB.Bson` objects from [MongoDB.Bson](https://www.nuget.org/packages/MongoDB.Bson) package into common objects.
- [Mappa Bson dependency](https://www.nuget.org/packages/Mappa.Dependency.Bson.DependencyInjection/): utility methods to register the Bson mapper.

You can find [samples](https://github.com/sanelli/Mappa/tree/main/Mappa.Samples) here.
Visit the Mappa [documentation](https://github.com/sanelli/Mappa/blob/main/Documentation/README.md) to learn more.