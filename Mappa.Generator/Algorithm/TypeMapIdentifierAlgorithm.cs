// <copyright file="TypeMapIdentifierAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Algorithm to identify a suitable map strategy from <see cref="SourceType"/>
/// to <see cref="TargetType"/>.
/// </summary>
internal class TypeMapIdentifierAlgorithm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeMapIdentifierAlgorithm"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="source">The name of the source mapping.</param>
    public TypeMapIdentifierAlgorithm(
        MappaMapAlgorithmContext context,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        string source)
    {
        this.Context = context;
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.Source = source;
    }

    /// <summary>
    /// Gets the context.
    /// </summary>
    internal MappaMapAlgorithmContext Context { get; }

    /// <summary>
    /// Gets the target type.
    /// </summary>
    internal ITypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the source type.
    /// </summary>
    internal ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the mapping source.
    /// </summary>
    internal string Source { get; }

    /// <summary>
    /// Compute a suitable strategy from type <see cref="SourceType"/> to
    /// <see cref="TargetType"/>.
    /// </summary>
    /// <returns>The strategy computed.</returns>
    internal virtual IMapStrategy GetStrategy()
    {
        var nullableEnabled = this.Context.IsNullableEnabled();
        var notNullableEnabled = !nullableEnabled;

        // 01. Map to the very same type.
        // (non-nullable): T -> T
        // (nullable): T -> T
        // (nullable): T? -> T?
        // TODO: (nullable) T -> T?
        // TODO: (non-nullable) T -> T? (where T is non reference type)
        if ((notNullableEnabled && SymbolEqualityComparer.Default.Equals(this.TargetType, this.SourceType))
            || (nullableEnabled && SymbolEqualityComparer.IncludeNullability.Equals(this.TargetType, this.SourceType))
            || (nullableEnabled && SymbolEqualityComparer.Default.Equals(this.TargetType, this.SourceType) &&
                this.TargetType.NullableAnnotation == NullableAnnotation.Annotated))
        {
            // TODO: Introduce the ability to perform a deep copy instead of shallow copy.
            return new IdentityMapStrategy(
                MappaAlgorithmRule.MapToSameType,
                this.TargetType,
                this.SourceType,
                this.Source);
        }

        // 02. Map to object
        // (non-nullable): * -> object
        // (nullable): * -> object?
        // TODO: (nullable) * -> object (When T is not NOT nullable annotated)
        if ((notNullableEnabled && this.TargetType.IsObject())
            || (nullableEnabled && this.TargetType.IsObject() &&
                this.TargetType.NullableAnnotation == NullableAnnotation.Annotated))
        {
            return new IdentityMapStrategy(
                MappaAlgorithmRule.MapToObject,
                this.TargetType,
                this.SourceType,
                this.Source);
        }

        // XX. Map nullable to nullable
        //    TODO: (nullable & non-nullable) S? -> T? : NullableStrategy( Strategy(T, S) )
        // XX. numeric -> implicit-convertible-numeric : Identity strategy.
        // XX. IDictionary<SK,SV> -> Dictionary<TK,TV> : DictionaryStrategy( Strategy(TK, SK), Strategy(TV, SV) ).
        // XX. enum -> string : EnumToString strategy.
        // XX. enum -> implicit-convertible-integral : EnumToIntegral strategy.
        // XX. string -> enum : StringToEnum strategy.
        // XX. integral -> enum : IntegralToEnum strategy.
        // XX. S[] -> T[] : ArrayStrategy ( Strategy(T, S) ).
        // XX. HashSet<S> -> HashSet<T> : HashSetStrategy( Strategy(T, S) ).
        // XX. List<S> -> List<T> : ListStrategy ( Strategy(T, S) ).
        // XX. IReadOnlyCollection<S> -> IReadOnlyCollection<T> : ReadOnlyCollectionStrategy ( Strategy(T, S) ).
        // XX. IEnumerable<S> -> IEnumerable<T> : EnumerableStrategy ( Strategy(T, T) )
        // XX. S -> T : ConstructorStrategy(S, T)
        // XX. Report error
        throw new NotImplementedException();
    }
}