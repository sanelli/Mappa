# Mappa
Mappa implements mapping via source generators.

## How does it work?
TODO

## Examples
TODO

## Algorithm
TODO

### Strategies
This is the ordered list of strategies applied
1. **Identity strategy**: the source is assigned directly to the target. This happens in the following scenario:
   1. The source and the target are the same type
   2. The target is of type `object`
   3. An implicit conversion exists from source to target
2. **Enum strategy**: either the source or the target is an `enum`
   1. The source is an `enum` and the target is a `string`
   2. The source is an `enum` and the target is an integral type (`int`, `short`, ...) and an implicit conversion exists from the underlying integral type of the source `enum`
   3. The source is a `string` and the target is an `enum`
   4. The source is an integral type (`int`, `short`, ...) and the target is an `anum` and an implicit conversion exists to the underlying integral type of the target `enum`
3. **String strategy**: either the source of the target type is a a `string`
   1. The source is a `string` and the target is any numeric type: the target `TargetType.Parse(string)` static method will be invoked (e.g. `int.Parse("3")`)
   2. The source is a `string` and the target is `DateTime`: the target `DateTime.Parse(string)` static method will be invoked
   3. The source is a `string` and the target is `DateOnly`: the target `DateOnly.Parse(string)` static method will be invoked (*TODO*)
   4. The source is a `string` and the target is `TimeOnly`: the target `TimeOnly.Parse(string)` static method will be invoked (*TODO*)
   5. The source is a `string` and the target is `Guid`: the target `Guid.Parse(string)` static method will be invoked (*TODO*)
   6. The source is a `string` and the target is `TimeSpan`: the target `TimeSpan.Parse(string)` static method will be invoked (*TODO*)
   7. The source is a `string` and the target is `URI`: the target `URI.Parse(string)` static method will be invoked (*TODO*)
   8. The source is a `string` and the target type has a static `Parse` method accepting `string` as input an returning the target type itself (*TODO*)
   9. The target is a `string`: The method `ToString()` will be invoked

## Attributes to control the mapping
- `Mappa`: Enable the generation of mapping for a class.

## Performances
TODO