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
    // TODO: For many of these scenario where we support IList<T>/ICollection<T>/IEnumerable<T> we might want to check if
    //       the type implements IList<T>/ICollection<T>/IEnumerable<T>.
    public bool TryDetect(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // 01. S[]/List<T> -> T[] : ArrayOrListToArrayMapStrategy ( IMapStrategy(T, S) ).
        if (this.CanMapArrayOrListToArray(out var arrayOrListToArrayElementStrategy))
        {
            // TODO: Add Support for input implementing IList<>
            // TODO: Add support for faster iteration using Span<>
            mapStrategy = new ArrayOrListToArrayMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                arrayOrListToArrayElementStrategy);
        }

        // 02. Collection<S>/Enumerable<S> -> T[] : CollectionOrEnumerableToArray( IMapStrategy(T, S) )
        else if (this.CanMapCollectionOrEnumerableToArray(out var collectionOrEnumerableToArrayElementStrategy))
        {
            // TODO: Add Support for input implementing IEnumerable<>, ICollection<>, IReadOnlyCollection<>
            mapStrategy = new EnumerableOrCollectionToArrayMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                collectionOrEnumerableToArrayElementStrategy);
        }

        // 03. S[]/List<S> -> Collection<T>/IEnumerable<T> : ArrayOrListToCollectionMapStrategy ( IMapStrategy(T, S) ).
        else if (this.CanMapArrayOrListToCollectionOrEnumerable(out var arrayOrListElementStrategy))
        {
            // TODO: Add Support for input implementing IList<>
            // TODO: Add Support for output implementing IList<>, IEnumerable<>, IReadOnlyCollection<>, ICollection<>
            // TODO: Check if it is possible using Span<> here as well.
            // TODO: Allow to prefer returning array over lists
            mapStrategy = new ArrayOrListToCollectionMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                arrayOrListElementStrategy);
        }

        // 04. IEnumerable<S>/Collection<S> -> Collection<T>/IEnumerable<T> : EnumerableOrCollectionToCollectionMapStrategy ( IMapStrategy(T, S) ).
        else if (this.CanMapCollectionOrEnumerableToCollectionOrEnumerable(out var collectionOrEnumerableElementStrategy))
        {
            // TODO: Add Support for input implementing IEnumerable<>, ICollection<>, IReadOnlyCollection<>
            // TODO: Add Support for output implementing IList<>, IEnumerable<>, IReadOnlyCollection<>, ICollection<>
            // TODO: Check if it is possible using Span<> here as well.
            // TODO: Allow to prefer returning array over lists
            mapStrategy = new EnumerableOrCollectionToCollectionMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                collectionOrEnumerableElementStrategy);
        }

        // 05. Dictionary<SK,SV> -> Dictionary<TK,TV> : DictionaryStrategy( IMapStrategy(TK, SK), IMapStrategy(TV, SV) ).
        else if (this.CanMapDictionaryToDictionary(out var dictionaryKeyStrategy, out var dictionaryValueStrategy))
        {
            // TODO: Allow the user to specify if they want to use .Add or the indexer
            // TODO: Add Support for input implementing IDictionary<>
            // TODO: Add Support for output implementing IDictionary<>
            mapStrategy = new DictionaryToDictionaryMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                dictionaryKeyStrategy,
                dictionaryValueStrategy);
        }

        return mapStrategy is not NoMapStrategy;
    }

    private bool CanMapArrayOrListToArray(out IMapStrategy elementStrategy)
    {
        // Source can be S[], IList<S>, List<S>
        var acceptSource = this.context.SourceType.IsArray();
        acceptSource = acceptSource || this.context.SourceType.IsIList();
        acceptSource = acceptSource || this.context.SourceType.IsList(this.compilation);

        var isTargetArray = this.context.TargetType.IsArray();

        elementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        return acceptSource && isTargetArray && this.TryGetElementStrategy(out elementStrategy);
    }

    private bool CanMapCollectionOrEnumerableToArray(out IMapStrategy elementStrategy)
    {
        // Source can be IEnumerable<S>, ICollection<S> or IReadOnlyCollection<S>
        var acceptSource = this.context.SourceType.IsIEnumerable();
        acceptSource = acceptSource || this.context.SourceType.IsICollection();
        acceptSource = acceptSource || this.context.SourceType.IsIReadOnlyCollection();

        var isTargetArray = this.context.TargetType.IsArray();

        elementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        return acceptSource && isTargetArray && this.TryGetElementStrategy(out elementStrategy);
    }

    private bool CanMapArrayOrListToCollectionOrEnumerable(out IMapStrategy elementStrategy)
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
        return acceptSource && acceptTarget && this.TryGetElementStrategy(out elementStrategy);
    }

    private bool CanMapCollectionOrEnumerableToCollectionOrEnumerable(out IMapStrategy elementStrategy)
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
        return acceptSource && acceptTarget && this.TryGetElementStrategy(out elementStrategy);
    }

    private bool CanMapDictionaryToDictionary(out IMapStrategy keyStrategy, out IMapStrategy valueStrategy)
    {
        var isSourceDictionary = this.context.SourceType.IsIDictionary(this.compilation)
                                 || this.context.SourceType.IsDictionary(this.compilation);
        var isTargetDictionary = this.context.TargetType.IsIDictionary(this.compilation)
                                 || this.context.TargetType.IsDictionary(this.compilation);

        keyStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        valueStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        return isSourceDictionary && isTargetDictionary && this.TryGetKeyAndValueStrategy(out keyStrategy, out valueStrategy);
    }

    private bool TryGetElementStrategy(out IMapStrategy elementStrategy)
    {
        var sourceElementType = this.context.SourceType.GetElementType();
        var targetElementType = this.context.TargetType.GetElementType();
        var derivedContext = new DerivedMappaMapAlgorithmContext(
            this.context,
            targetElementType,
            sourceElementType);
        var algorithm = new TypeMapIdentifierWithMapMethodAlgorithm(derivedContext, this.compilation, this.cancellationToken);
        elementStrategy = algorithm.GetStrategy();
        return elementStrategy is not NoMapStrategy;
    }

    private bool TryGetKeyAndValueStrategy(out IMapStrategy keyStrategy, out IMapStrategy valueStrategy)
    {
        var (sourceKeyType, sourceKeyValueType) = this.context.SourceType.GetKeyAndValueTypes();
        var (targetKeyType, targetValueType) = this.context.TargetType.GetKeyAndValueTypes();

        // Get strategy for key
        var keyContext = new DerivedMappaMapAlgorithmContext(
            this.context,
            targetKeyType,
            sourceKeyType);
        var keyAlgorithm = new TypeMapIdentifierWithMapMethodAlgorithm(keyContext, this.compilation, this.cancellationToken);
        keyStrategy = keyAlgorithm.GetStrategy();

        // Get strategy for value
        var valueContext = new DerivedMappaMapAlgorithmContext(
            this.context,
            targetValueType,
            sourceKeyValueType);
        var valueAlgorithm = new TypeMapIdentifierWithMapMethodAlgorithm(valueContext, this.compilation, this.cancellationToken);
        valueStrategy = valueAlgorithm.GetStrategy();

        return keyStrategy is not NoMapStrategy && valueStrategy is not NoMapStrategy;
    }
}