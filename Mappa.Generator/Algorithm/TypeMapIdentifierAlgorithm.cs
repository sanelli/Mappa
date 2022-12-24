// <copyright file="TypeMapIdentifierAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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
        // XX. * -> object : IdentityStrategy
        // XX. T -> T or T -> T?: Identity strategy
        // XX. numeric -> implicit-convertible-numeric : Identity strategy.
        // XX. IDictionary<K,V> -> Dictionary<K,V> : DictionaryStrategy( Strategy(K, V), Startegy(TV, SV) ).
        // XX. enum -> string : EnumToString strategy.
        // XX. enum -> implicit-convertible-integral : EnumToIntegral strategy.
        // XX. string -> enum : StringToEnum strategy.
        // XX. integral -> enum : IntegralToEnum strategy.
        // XX. S[] -> T[] : ArrayStrategy ( Strategy(T, S) ).
        // XX. HashSet<S> -> HashSet<T> : HashSetStrategy( Strategy(S, T) ).
        // XX. List<S> -> List<T> : ListStrategy ( Strategy(T, S) ).
        // XX. IReadOnlyCollection<S> -> IReadOnlyCollection<T> : ReadOnlyCollectionStrategy ( Strategy(T, S) ).
        // XX. S -> T : ConstructorStrategy(S, T)
        throw new NotImplementedException();
    }
}