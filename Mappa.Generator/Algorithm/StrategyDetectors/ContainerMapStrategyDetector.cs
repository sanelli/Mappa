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

        // 01. Dictionary -> Dictionary strategy.
        if (this.CanMapDictionaryToDictionary(out var dictionaryKeyStrategy, out var dictionaryValueStrategy))
        {
            // TODO [#34] Allow the user to specify if they want to use .Add or the indexer.
            mapStrategy = new DictionaryToDictionaryMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                dictionaryKeyStrategy,
                dictionaryValueStrategy);
        }

        // 02. Collection -> Collection strategy.
        // TODO [#24] Add support for faster iteration using Span<>.
        // TODO [#29] Allow to prefer returning array over lists for interfaces.
        // TODO [#108] Prevent using Enumerable.Count() when the user asks for it.
        else if (this.CanMapCollectionToCollection(out var elementStrategy))
        {
            mapStrategy = new CollectionToCollectionMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                elementStrategy);
        }

        return mapStrategy is not NoMapStrategy;
    }

    // TODO [#105] Support SortedDictionary as output type.
    // TODO [#105] Support ImmutableSortedDictionary as output type.
    // TODO [#105] Support IImmutableDictionary as output type.
    private bool CanMapDictionaryToDictionary(out MapStrategy keyStrategy, out MapStrategy valueStrategy)
    {
        var isSourceDictionary = this.context.SourceType.IsOrImplementIDictionary(this.compilation)
                                 || this.context.TargetType.IsIReadOnlyDictionary(this.compilation)
                                 || this.context.TargetType.IsOrImplementIEnumerableOfKeyValuePair(this.compilation);
        var isTargetDictionary = (this.context.TargetType.IsOrImplementIDictionary(this.compilation)
                                  || this.context.TargetType.IsIEnumerableOfKeyValuePairs(this.compilation)
                                  || this.context.TargetType.IsIReadOnlyDictionary(this.compilation)
                                  || this.context.TargetType.IsReadOnlyDictionary(this.compilation)
                                  || this.context.TargetType.IsImmutableDictionary(this.compilation)
                                  || this.context.TargetType.IsFrozenDictionary(this.compilation))
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
                return this.context.TargetType.IsIDictionary(this.compilation)
                       || this.context.TargetType.IsIEnumerableOfKeyValuePairs(this.compilation)
                       || this.context.TargetType.IsIReadOnlyDictionary(this.compilation);
            }

            // Target type MUST have a constructor with no arguments
            // unless it is a ReadOnlyDictionary, ImmutableDictionary or a FrozenDictionary
            // for which special coding is provided.
            if (this.context.TargetType.IsReadOnlyDictionary(this.compilation)
                || this.context.TargetType.IsImmutableDictionary(this.compilation)
                || this.context.TargetType.IsFrozenDictionary(this.compilation))
            {
                return true;
            }

            // TODO [#105] Check the constructor is accessible from the called location.
            return this.context.TargetType.HasZeroParametersConstructor();
        }
    }

    private bool CanMapCollectionToCollection(out MapStrategy elementStrategy)
    {
        var isSourceCollection = this.context.SourceType.IsOrImplementIEnumerable()
            || this.context.SourceType.IsSpan(this.compilation)
            || this.context.SourceType.IsMemory(this.compilation)
            || this.context.SourceType.IsReadOnlySpan(this.compilation)
            || this.context.SourceType.IsReadOnlyMemory(this.compilation);

        // TODO [#105] ImmutableHashSet.
        // TODO [#105] IImmutableSet.
        // TODO [#105] ImmutableSet.
        // TODO [#105] ImmutableArray.
        // TODO [#105] ImmutableList.
        // TODO [#105] IImmutableList.
        // TODO [#105] ImmutableQueue.
        // TODO [#105] IImmutableQueue.
        // TODO [#105] ImmutableSortedSet.
        // TODO [#105] ImmutableStack.
        // TODO [#105] IImmutableStack.
        var isTargetCollection = (this.context.TargetType.IsArray()
                                 || this.context.TargetType.IsIEnumerable()
                                 || this.context.TargetType.IsIList()
                                 || this.context.TargetType.IsIReadOnlyList()
                                 || this.context.TargetType.IsList(this.compilation)
                                 || this.context.TargetType.IsOrImplementICollection()
                                 || this.context.TargetType.IsIReadOnlyCollection()
                                 || this.context.TargetType.IsSpan(this.compilation)
                                 || this.context.TargetType.IsReadOnlySpan(this.compilation)
                                 || this.context.TargetType.IsMemory(this.compilation)
                                 || this.context.TargetType.IsReadOnlyMemory(this.compilation)
                                 || this.context.TargetType.IsOrImplementStack(this.compilation)
                                 || this.context.TargetType.IsOrImplementQueue(this.compilation)
                                 || this.context.TargetType.IsOrImplementISet(this.compilation)
                                 || this.context.TargetType.IsIReadOnlySet(this.compilation)
                                 || this.context.TargetType.IsHashSet(this.compilation)
                                 || this.context.TargetType.IsReadOnlyCollection(this.compilation)
                                 || this.context.TargetType.IsReadOnlySet(this.compilation)
                                 || this.context.TargetType.IsFrozenSet(this.compilation))
                                 && InterfaceAndConstructorChecks();

        elementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        return isSourceCollection && isTargetCollection && this.context.TryGetElementStrategy(
            this.compilation,
            out elementStrategy,
            this.cancellationToken);

        bool InterfaceAndConstructorChecks()
        {
            if (this.context.TargetType.TypeKind is TypeKind.Array)
            {
                return true;
            }

            if (this.context.TargetType.TypeKind is TypeKind.Interface)
            {
                // TODO [#105] IImmutableList.
                // TODO [#105] IImmutableStack.
                // TODO [#105] IImmutableQueue.
                // TODO [#105] IImmutableSet.
                return this.context.TargetType.IsIEnumerable()
                       || this.context.TargetType.IsIList()
                       || this.context.TargetType.IsIReadOnlyList()
                       || this.context.TargetType.IsICollection()
                       || this.context.TargetType.IsIReadOnlyCollection()
                       || this.context.TargetType.IsISet(this.compilation)
                       || this.context.TargetType.IsIReadOnlySet(this.compilation)
                    ;
            }

            // Target type MUST have a constructor with no arguments
            // unless it is a ReadOnlyDictionary, ImmutableDictionary or a FrozenDictionary
            // for which special coding is provided.
            // TODO [#105] ImmutableHashSet -- Add exception.
            // TODO [#105] ImmutableArray -- Add exception.
            // TODO [#105] ImmutableList -- Add exception.
            // TODO [#105] ImmutableQueue -- Add exception.
            // TODO [#105] ImmutableSortedSet -- Add exception.
            // TODO [#105] ImmutableStack -- Add exception.
            // TODO [#105] ImmutableSet -- Add exception.
            if (this.context.TargetType.IsReadOnlyCollection(this.compilation)
                || this.context.TargetType.IsReadOnlySet(this.compilation)
                || this.context.TargetType.IsFrozenSet(this.compilation))
            {
                return true;
            }

            // TODO [#109] Support constructor with 1 integer parameter (capacity) via mappaSettings.
            // TODO [#105] Check the constructor is accessible from the called location.
            return this.context.TargetType.HasZeroParametersConstructor();
        }
    }
}