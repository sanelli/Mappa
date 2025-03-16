// <copyright file="ContainerMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for container related strategies.
/// </summary>
internal sealed class ContainerMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;
    private readonly CancellationToken cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public ContainerMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        this.context = context;
        this.cancellationToken = cancellationToken;
        this.compilation = compilation;
    }

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // 01. S[]/List<T> -> T[] : ArrayOrListToArrayMapStrategy ( IMapStrategy(T, S) ).
        if (this.CanMapArrayOrListToArray(out var arrayOrListToArrayElementStrategy))
        {
            // TODO [#23] Add Support for input implementing IList<>.
            // TODO [#24] Add support for faster iteration using Span<>.
            mapStrategy = new ArrayOrListToArrayMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                arrayOrListToArrayElementStrategy);
        }

        // 02. Collection<S>/Enumerable<S> -> T[] : CollectionOrEnumerableToArray( IMapStrategy(T, S) )
        else if (this.CanMapCollectionOrEnumerableToArray(out var collectionOrEnumerableToArrayElementStrategy))
        {
            // TODO [#25] Add Support for input implementing IEnumerable<>, ICollection<>, IReadOnlyCollection<>.
            mapStrategy = new EnumerableOrCollectionToArrayMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                collectionOrEnumerableToArrayElementStrategy);
        }

        // 03. S[]/List<S> -> Collection<T>/IEnumerable<T> : ArrayOrListToCollectionMapStrategy ( IMapStrategy(T, S) ).
        else if (this.CanMapArrayOrListToCollectionOrEnumerable(out var arrayOrListElementStrategy))
        {
            // TODO [#26] Add Support for input implementing IList<>.
            // TODO [#27] Add Support for output implementing IList<>, IEnumerable<>, IReadOnlyCollection<>, ICollection<>.
            // TODO [#28] Check if it is possible using Span<> here as well.
            // TODO [#29] Allow to prefer returning array over lists.
            mapStrategy = new ArrayOrListToCollectionMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                arrayOrListElementStrategy);
        }

        // 04. IEnumerable<S>/Collection<S> -> Collection<T>/IEnumerable<T> : EnumerableOrCollectionToCollectionMapStrategy ( IMapStrategy(T, S) ).
        else if (this.CanMapCollectionOrEnumerableToCollectionOrEnumerable(out var collectionOrEnumerableElementStrategy))
        {
            // TODO [#30] Add Support for input implementing IEnumerable<>, ICollection<>, IReadOnlyCollection<>.
            // TODO [#31] Add Support for output implementing IList<>, IEnumerable<>, IReadOnlyCollection<>, ICollection<>.
            // TODO [#32] Check if it is possible using Span<> here as well.
            // TODO [#33] Allow to prefer returning array over lists.
            mapStrategy = new EnumerableOrCollectionToCollectionMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                collectionOrEnumerableElementStrategy);
        }

        // 05. (I)Dictionary<SK,SV> -> (I)Dictionary<TK,TV> : DictionaryStrategy( IMapStrategy(TK, SK), IMapStrategy(TV, SV) ).
        else if (this.CanMapDictionaryToDictionary(out var dictionaryKeyStrategy, out var dictionaryValueStrategy))
        {
            // TODO [#34] Allow the user to specify if they want to use .Add or the indexer.
            mapStrategy = new DictionaryToDictionaryMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                dictionaryKeyStrategy,
                dictionaryValueStrategy);
        }

        return mapStrategy is not NoMapStrategy;
    }

    private bool CanMapArrayOrListToArray(out MapStrategy elementStrategy)
    {
        // Source can be S[], IList<S>, List<S>
        var acceptSource = this.context.SourceType.IsArray();
        acceptSource = acceptSource || this.context.SourceType.IsIList();
        acceptSource = acceptSource || this.context.SourceType.IsList(this.compilation);

        var isTargetArray = this.context.TargetType.IsArray();

        elementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        return acceptSource && isTargetArray && this.context.TryGetElementStrategy(this.compilation, out elementStrategy, this.cancellationToken);
    }

    private bool CanMapCollectionOrEnumerableToArray(out MapStrategy elementStrategy)
    {
        // Source can be IEnumerable<S>, ICollection<S> or IReadOnlyCollection<S>
        var acceptSource = this.context.SourceType.IsIEnumerable();
        acceptSource = acceptSource || this.context.SourceType.IsICollection();
        acceptSource = acceptSource || this.context.SourceType.IsIReadOnlyCollection();

        var isTargetArray = this.context.TargetType.IsArray();

        elementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        return acceptSource && isTargetArray && this.context.TryGetElementStrategy(this.compilation, out elementStrategy, this.cancellationToken);
    }

    private bool CanMapArrayOrListToCollectionOrEnumerable(out MapStrategy elementStrategy)
    {
        // Source can be S[], IList<S>, List<S>
        var acceptSource = this.context.SourceType.IsArray();
        acceptSource = acceptSource || this.context.SourceType.IsIList();
        acceptSource = acceptSource || this.context.SourceType.IsList(this.compilation);

        // Target can be IList<T>, List<T>, ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>
        var acceptTarget = this.context.TargetType.IsIList();
        acceptTarget = acceptTarget || this.context.TargetType.IsList(this.compilation);
        acceptTarget = acceptTarget || this.context.TargetType.IsICollection();
        acceptTarget = acceptTarget || this.context.TargetType.IsIReadOnlyCollection();
        acceptTarget = acceptTarget || this.context.TargetType.IsIEnumerable();

        // Return result of check.
        elementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        return acceptSource && acceptTarget && this.context.TryGetElementStrategy(this.compilation, out elementStrategy, this.cancellationToken);
    }

    private bool CanMapCollectionOrEnumerableToCollectionOrEnumerable(out MapStrategy elementStrategy)
    {
        // Source can be S[], IList<S>, List<S>
        var acceptSource = this.context.SourceType.IsIEnumerable();
        acceptSource = acceptSource || this.context.SourceType.IsICollection();
        acceptSource = acceptSource || this.context.SourceType.IsIReadOnlyCollection();

        // Target can be IList<T>, List<T>, ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>
        var acceptTarget = this.context.TargetType.IsIList();
        acceptTarget = acceptTarget || this.context.TargetType.IsList(this.compilation);
        acceptTarget = acceptTarget || this.context.TargetType.IsICollection();
        acceptTarget = acceptTarget || this.context.TargetType.IsIReadOnlyCollection();
        acceptTarget = acceptTarget || this.context.TargetType.IsIEnumerable();

        // Return result of check.
        elementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        return acceptSource && acceptTarget && this.context.TryGetElementStrategy(this.compilation, out elementStrategy, this.cancellationToken);
    }

    private bool CanMapDictionaryToDictionary(out MapStrategy keyStrategy, out MapStrategy valueStrategy)
    {
        var isSourceDictionary = this.context.SourceType.IsOrImplementIDictionary(this.compilation);
        var isTargetDictionary = this.context.TargetType.IsOrImplementIDictionary(this.compilation)
            && IfInterfaceAcceptOnlyIDictionary();

        keyStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        valueStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        return isSourceDictionary && isTargetDictionary && this.context.TryGetKeyAndValueStrategy(
            this.compilation,
            out keyStrategy,
            out valueStrategy,
            this.cancellationToken);

        bool IfInterfaceAcceptOnlyIDictionary()
        {
            if (this.context.TargetType.TypeKind is TypeKind.Interface)
            {
                return this.context.TargetType.IsIDictionary(this.compilation);
            }

            // Target type MUST have a constructor with no arguments.
            if (this.context.TargetType is INamedTypeSymbol namedTypeSymbol)
            {
                return namedTypeSymbol.Constructors.Any(constructor => constructor.Parameters.Length == 0);
            }

            return false;
        }
    }
}