// <copyright file="TypeMapIdentifierAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.TypeMapStrategy;

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
    public TypeMapIdentifierAlgorithm(MappaClassGeneratorContext context, ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        this.Context = context;
        this.TargetType = targetType;
        this.SourceType = sourceType;
    }

    /// <summary>
    /// Gets the context.
    /// </summary>
    internal MappaClassGeneratorContext Context { get; }

    /// <summary>
    /// Gets the target type.
    /// </summary>
    internal ITypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the source type.
    /// </summary>
    internal ITypeSymbol SourceType { get; }

    /// <summary>
    /// Compute a suitable strategy from type <see cref="SourceType"/> tp
    /// <see cref="TargetType"/>.
    /// </summary>
    /// <returns>The strategy computed.</returns>
    internal virtual ITypeMapStrategy GetStrategy()
    {
        // 01. * -> object : Identity strategy
        // 02. T -> T : Identity strategy
        // 05. numeric -> implicit-convertible-numeric : Identity strategy.
        // 03. IDictionary<TK,TV> -> Dictionary<SK,SV> : DictionaryStrategy( Strategy(TK, SK), Startegy(TV, SV) ).
        // 04. enum -> string : EnumToString strategy.
        // 05. enum -> implicit-convertible-integral : EnumToIntegral strategy.
        // 06. string -> enum : StringToEnum strategy.
        // 07. integral -> enum : IntegralToEnum strategy.
        // 08. S[] -> T[] : ArrayStrategy ( Strategy(T, S) ).
        // 09. HashSet<S> -> HashSet<T> : HashSetStrategy( Strategy(S, T) ).
        // 10. List<S> -> List<T> : ListStrategy ( Strategy(T, S) ).
        // 11. IReadOnlyCollection<S> -> IReadOnlyCollection<T> : ReadOnlyCollectionStrategy ( Strategy(T, S) ).
        // 12. S -> T : ConstructorStrategy(S, T)
        throw new NotImplementedException();
    }
}