// <copyright file="ContainerMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
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
    private static readonly Func<ITypeSymbol, Compilation, bool>[] SourceCollectionTypePredicates =
    {
        static (type, _) => type.IsOrImplementIEnumerable(),
        static (type, compilation) => type.IsSpan(compilation),
        static (type, compilation) => type.IsMemory(compilation),
        static (type, compilation) => type.IsReadOnlySpan(compilation),
        static (type, compilation) => type.IsReadOnlyMemory(compilation),
    };

    private static readonly Func<ITypeSymbol, Compilation, bool>[] SupportedCollectionTargetTypePredicates =
    {
        static (type, _) => type.IsArray(),
        static (type, _) => type.IsIEnumerable(),
        static (type, _) => type.IsIList(),
        static (type, _) => type.IsIReadOnlyList(),
        static (type, compilation) => type.IsList(compilation),
        static (type, _) => type.IsOrImplementICollection(),
        static (type, _) => type.IsIReadOnlyCollection(),
        static (type, compilation) => type.IsSpan(compilation),
        static (type, compilation) => type.IsReadOnlySpan(compilation),
        static (type, compilation) => type.IsMemory(compilation),
        static (type, compilation) => type.IsReadOnlyMemory(compilation),
        static (type, compilation) => type.IsOrDerivedFromStack(compilation),
        static (type, compilation) => type.IsOrDerivedFromQueue(compilation),
        static (type, compilation) => type.IsOrImplementISet(compilation),
        static (type, compilation) => type.IsIReadOnlySet(compilation),
        static (type, compilation) => type.IsHashSet(compilation),
        static (type, compilation) => type.IsReadOnlyCollection(compilation),
        static (type, compilation) => type.IsReadOnlySet(compilation),
        static (type, compilation) => type.IsFrozenSet(compilation),
        static (type, compilation) => type.IsIImmutableSet(compilation),
        static (type, compilation) => type.IsImmutableHashSet(compilation),
        static (type, compilation) => type.IsImmutableSortedSet(compilation),
        static (type, compilation) => type.IsIImmutableList(compilation),
        static (type, compilation) => type.IsImmutableArray(compilation),
        static (type, compilation) => type.IsImmutableList(compilation),
        static (type, compilation) => type.IsIImmutableQueue(compilation),
        static (type, compilation) => type.IsImmutableQueue(compilation),
        static (type, compilation) => type.IsIImmutableStack(compilation),
        static (type, compilation) => type.IsImmutableStack(compilation),
        static (type, compilation) => type.IsOrDerivedFromBlockingCollection(compilation),
        static (type, compilation) => type.IsOrDerivedFromConcurrentBag(compilation),
        static (type, compilation) => type.IsOrDerivedFromConcurrentStack(compilation),
        static (type, compilation) => type.IsOrImplementConcurrentQueue(compilation),
        static (type, compilation) => type.IsIProducerConsumerCollection(compilation),
    };

    private static readonly Func<ITypeSymbol, Compilation, bool>[] SupportedCollectionInterfaceTargetPredicates =
    {
        static (type, _) => type.IsIEnumerable(),
        static (type, _) => type.IsIList(),
        static (type, _) => type.IsIReadOnlyList(),
        static (type, _) => type.IsICollection(),
        static (type, _) => type.IsIReadOnlyCollection(),
        static (type, compilation) => type.IsISet(compilation),
        static (type, compilation) => type.IsIReadOnlySet(compilation),
        static (type, compilation) => type.IsIImmutableSet(compilation),
        static (type, compilation) => type.IsIImmutableList(compilation),
        static (type, compilation) => type.IsIImmutableQueue(compilation),
        static (type, compilation) => type.IsIImmutableStack(compilation),
        static (type, compilation) => type.IsIProducerConsumerCollection(compilation),
    };

    private static readonly Func<ITypeSymbol, Compilation, bool>[] BuiltInCollectionConstructorTargetPredicates =
    {
        static (type, compilation) => type.IsReadOnlyCollection(compilation),
        static (type, compilation) => type.IsReadOnlySet(compilation),
        static (type, compilation) => type.IsFrozenSet(compilation),
        static (type, compilation) => type.IsImmutableHashSet(compilation),
        static (type, compilation) => type.IsImmutableSortedSet(compilation),
        static (type, compilation) => type.IsImmutableArray(compilation),
        static (type, compilation) => type.IsImmutableList(compilation),
        static (type, compilation) => type.IsImmutableQueue(compilation),
        static (type, compilation) => type.IsImmutableStack(compilation),
    };

    private static readonly Func<ITypeSymbol, Compilation, bool>[] CapacityConstructorSupportPredicates =
    {
        static (type, _) => type.ImplementICollection(),
        static (type, compilation) => type.ImplementISet(compilation),
        static (type, compilation) => type.IsOrDerivedFromStack(compilation),
        static (type, compilation) => type.IsOrDerivedFromQueue(compilation),
        static (type, compilation) => type.IsOrDerivedFromBlockingCollection(compilation),
    };

    private static readonly Func<ITypeSymbol, Compilation, bool>[] DictionaryConcreteTargetTypePredicates =
    [
        static (type, compilation) => type.IsOrImplementIDictionary(compilation),
        static (type, compilation) => type.IsIEnumerableOfKeyValuePairs(compilation),
        static (type, compilation) => type.IsIReadOnlyDictionary(compilation),
        static (type, compilation) => type.IsReadOnlyDictionary(compilation),
        static (type, compilation) => type.IsIImmutableDictionary(compilation),
        static (type, compilation) => type.IsImmutableDictionary(compilation),
        static (type, compilation) => type.IsImmutableSortedDictionary(compilation),
        static (type, compilation) => type.IsFrozenDictionary(compilation),
    ];

    private static readonly Func<ITypeSymbol, Compilation, bool>[] DictionaryInterfaceTargetTypePredicates =
    [
        static (type, compilation) => type.IsIDictionary(compilation),
        static (type, compilation) => type.IsIEnumerableOfKeyValuePairs(compilation),
        static (type, compilation) => type.IsIReadOnlyDictionary(compilation),
        static (type, compilation) => type.IsIImmutableDictionary(compilation),
    ];

    private static readonly Func<ITypeSymbol, Compilation, bool>[] DictionaryBuiltInConstructorTargetPredicates =
    [
        static (type, compilation) => type.IsReadOnlyDictionary(compilation),
        static (type, compilation) => type.IsImmutableDictionary(compilation),
        static (type, compilation) => type.IsImmutableSortedDictionary(compilation),
        static (type, compilation) => type.IsFrozenDictionary(compilation),
    ];

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
            mapStrategy = new DictionaryToDictionaryMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                dictionaryKeyStrategy,
                dictionaryValueStrategy,
                DictionaryAssignmentSettingHelper.GetEffective(this.context.MappaUserSettings.DictionaryAssignment));
        }

        // 02. Collection -> Collection strategy.
        else if (this.CanMapCollectionToCollection(out var elementStrategy))
        {
            mapStrategy = new CollectionToCollectionMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                elementStrategy,
                this.context.MapMethod?.MethodSymbol,
                this.context.MappaUserSettings.FastCollections,
                this.context.MappaUserSettings.ContainerCapacityConstructors,
                this.context.MappaUserSettings.PreventEnumerableCount,
                GetEffectiveEnumerableConcreteType(this.context.MappaUserSettings.EnumerableConcreteType));
        }

        return mapStrategy is not NoMapStrategy;
    }

    private static EnumerableConcreteTypeSetting GetEffectiveEnumerableConcreteType(
        EnumerableConcreteTypeSetting enumerableConcreteTypeSetting)
        => enumerableConcreteTypeSetting is EnumerableConcreteTypeSetting.Undefined
            ? EnumerableConcreteTypeSetting.List
            : enumerableConcreteTypeSetting;

    private static bool MatchesAnyPredicate(
        ITypeSymbol type,
        Compilation compilation,
        Func<ITypeSymbol, Compilation, bool>[] predicates)
        => predicates.Any(predicate => predicate(type, compilation));

    private static bool IsSourceCollectionType(ITypeSymbol type, Compilation compilation)
        => MatchesAnyPredicate(type, compilation, SourceCollectionTypePredicates);

    private static bool IsSupportedCollectionTargetType(ITypeSymbol type, Compilation compilation)
        => MatchesAnyPredicate(type, compilation, SupportedCollectionTargetTypePredicates);

    private bool CanMapDictionaryToDictionary(out MapStrategy keyStrategy, out MapStrategy valueStrategy)
    {
        keyStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        valueStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        if (!this.IsDictionaryMappingSourceSideEligible())
        {
            return false;
        }

        if (!MatchesAnyPredicate(this.context.TargetType, this.compilation, DictionaryConcreteTargetTypePredicates)
            || !this.TargetTypeAcceptsDictionaryMapping())
        {
            return false;
        }

        return this.context.TryGetKeyAndValueStrategy(
            this.compilation,
            out keyStrategy,
            out valueStrategy,
            this.cancellationToken);
    }

    private bool IsDictionaryMappingSourceSideEligible()
        => this.context.SourceType.IsOrImplementIDictionary(this.compilation)
           || this.context.TargetType.IsIReadOnlyDictionary(this.compilation)
           || this.context.TargetType.IsOrImplementIEnumerableOfKeyValuePair(this.compilation);

    private bool TargetTypeAcceptsDictionaryMapping()
    {
        if (this.context.TargetType.TypeKind is TypeKind.Interface)
        {
            return MatchesAnyPredicate(
                this.context.TargetType,
                this.compilation,
                DictionaryInterfaceTargetTypePredicates);
        }

        if (MatchesAnyPredicate(
                this.context.TargetType,
                this.compilation,
                DictionaryBuiltInConstructorTargetPredicates))
        {
            return true;
        }

        return this.context.TargetType.HasSymbolAccessibleZeroParametersConstructor(this.compilation, this.context.MapMethod?.MethodSymbol);
    }

    private bool CanMapCollectionToCollection(out MapStrategy elementStrategy)
    {
        elementStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        if (this.RejectQueryableCollectionToCollectionMapping())
        {
            return false;
        }

        if (!IsSourceCollectionType(this.context.SourceType, this.compilation))
        {
            return false;
        }

        if (!IsSupportedCollectionTargetType(this.context.TargetType, this.compilation))
        {
            return false;
        }

        if (!this.PassesInterfaceAndConstructorChecks())
        {
            return false;
        }

        return this.context.TryGetElementStrategy(
            this.compilation,
            out elementStrategy,
            this.cancellationToken);
    }

    private bool RejectQueryableCollectionToCollectionMapping()
    {
        var sourceIsQueryable = this.context.SourceType.IsOrImplementIQueryable(this.compilation);
        var targetIsQueryable = this.context.TargetType.IsOrImplementIQueryable(this.compilation);
        if (!sourceIsQueryable && !targetIsQueryable)
        {
            return false;
        }

        if (sourceIsQueryable
            && !targetIsQueryable
            && this.context.MapMethod is not null
            && this.IsConcreteCollectionTarget())
        {
            this.context.ReportDiagnostic(
                MappaDiagnostics.IQueryableMappedAsCollection(
                    this.context.MapMethod.MethodDeclarationSyntax?.GetLocation(),
                    this.context.MapMethod.MethodName));
        }

        return true;
    }

    private bool PassesInterfaceAndConstructorChecks()
    {
        if (this.context.TargetType.TypeKind is TypeKind.Array)
        {
            return true;
        }

        if (this.context.TargetType.TypeKind is TypeKind.Interface)
        {
            return MatchesAnyPredicate(
                this.context.TargetType,
                this.compilation,
                SupportedCollectionInterfaceTargetPredicates);
        }

        // For the following concrete types a suitable constructor exists.
        if (MatchesAnyPredicate(
                this.context.TargetType,
                this.compilation,
                BuiltInCollectionConstructorTargetPredicates))
        {
            return true;
        }

        // Any other concrete type must either have:
        // - a constructor with no parameters
        // - (only if ContainerCapacityConstructors is enabled) a constructor with one integer
        //   parameter and implement either: ICollection{T}, ISet{T}, IQueue{T}, IStack{T}
        //   or derive BlockingCollection{T}.
        return this.context.TargetType.HasSymbolAccessibleZeroParametersConstructor(this.compilation, this.context.MapMethod?.MethodSymbol)
            || this.PassesCapacityConstructorChecks();
    }

    private bool PassesCapacityConstructorChecks()
        => this.context.MappaUserSettings.ContainerCapacityConstructors is BooleanSetting.Enable
            && this.context.TargetType.TypeKind != TypeKind.Interface
            && this.CanSupportImplementationWithCapacityConstructor()
            && this.context.TargetType.HasSymbolAccessibleSingleIntegerParametersConstructor(this.compilation, this.context.MapMethod?.MethodSymbol);

    private bool CanSupportImplementationWithCapacityConstructor()
        => MatchesAnyPredicate(
            this.context.TargetType,
            this.compilation,
            CapacityConstructorSupportPredicates);

    private bool IsConcreteCollectionTarget()
        => IsSupportedCollectionTargetType(this.context.TargetType, this.compilation);
}