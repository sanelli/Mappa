// <copyright file="TypeMapIdentifierAlgorithm.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Algorithm to identify a suitable map strategy from <see cref="MappaMapAlgorithmContext.SourceType"/> to
/// <see cref="MappaMapAlgorithmContext.TargetType"/>.
/// </summary>
internal class TypeMapIdentifierAlgorithm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeMapIdentifierAlgorithm"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public TypeMapIdentifierAlgorithm(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        this.Context = context;
        this.Compilation = compilation;
        this.CancellationToken = cancellationToken;
    }

    /// <summary>
    /// Gets the context.
    /// </summary>
    protected MappaMapAlgorithmContext Context { get; }

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    protected CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets the compilation.
    /// </summary>
    private Compilation Compilation { get; }

    /// <summary>
    /// Compute a suitable strategy from type <see cref="MappaMapAlgorithmContext.SourceType"/> to
    /// <see cref="MappaMapAlgorithmContext.TargetType"/>.
    /// </summary>
    /// <returns>The strategy computed.</returns>
    internal virtual IMapStrategy GetStrategy()
    {
        this.CancellationToken.ThrowIfCancellationRequested();

        var nullableEnabled = this.Context.IsNullableEnabled();
        var notNullableEnabled = !nullableEnabled;

        // 01. Map to the very same type.
        if (CanMapUsingMapToSameTypeRule())
        {
            // TODO: Introduce the ability to perform a deep copy instead of shallow copy.
            return new IdentityMapStrategy(
                MappaAlgorithmRule.MapToSameType,
                this.Context.TargetType,
                this.Context.SourceType);
        }

        // 02. Map to object
        if (CanMapUsingMapToObjectRule())
        {
            return new IdentityMapStrategy(
                MappaAlgorithmRule.MapToObject,
                this.Context.TargetType,
                this.Context.SourceType);
        }

        // 03. Implicit conversion.
        if (CanMapUsingImplicitConversion())
        {
            return new IdentityMapStrategy(
                MappaAlgorithmRule.ImplicitConversion,
                this.Context.TargetType,
                this.Context.SourceType);
        }

        // 04. enum -> string : EnumToString strategy.
        if (CanMapEnumToString())
        {
            // TODO: Add ability to use content of Description attribute
            return new EnumToStringMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType);
        }

        // 05. enum -> implicit-convertible-integral : EnumToIntegral strategy.
        if (CanMapEnumToIntegral())
        {
            return new EnumToIntegralMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType);
        }

        // 06. string -> enum : StringToEnum strategy.
        if (CanMapStringToEnum())
        {
            // TODO: Add ability to use content of Description attribute.
            // TODO: Add ability to be case insensitive.
            return new StringToEnumMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType);
        }

        // 07. integral -> enum : IntegralToEnum strategy.
        if (CanMapIntegralToEnum())
        {
            return new IntegralToEnumMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType);
        }

        // 08. enum -> enum: EnumToEnumStrategy
        if (CanMapEnumToEnum())
        {
            // TODO: Allow to map using enum numeric value instead of their name.
            // TODO: Allow to fail generation if not all values can be mapped.
            return new EnumToEnumMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType);
        }

        // 09. string -> numeric : ParseNumberStrategy
        if (CanMapStringToNumber())
        {
            // TODO: Allow to setup different file format.
            return new StringToNumberMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType);
        }

        // 10. string -> DateTime : ParseDateTimeStrategy
        if (CanMapStringToDateTime())
        {
            // TODO: Allow to specify the expected format.
            return new StringToDateTimeMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType);
        }

        // 11. S -> string : InvokeToStringStrategy
        if (CanMapToString())
        {
            return new InvokeToStringMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType);
        }

        // 12. (struct) S? -> T? : NullableToNullableStrategy( IMapStrategy(T, S) )
        if (CanMapNullableToNullable(out var nullableElementStrategy))
        {
            return new NullableToNullableMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                nullableElementStrategy);
        }

        // 13. (struct) 12/A S? -> T
        // TODO: Implement me
        // 14. (struct) 12/B S -> T?
        // TODO: Implement me
        // 15. S[]/List<T> -> T[] : ArrayOrListToArrayMapStrategy ( IMapStrategy(T, S) ).
        if (CanMapArrayOrListToArray(out var arrayOrListToArrayElementStrategy))
        {
            // TODO: Add support for faster iteration using Span<>
            return new ArrayOrListToArrayMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                arrayOrListToArrayElementStrategy);
        }

        // 16. Collection<S>/Enumerable<S> -> T[] : CollectionOrEnumerableToArray( IMapStrategy(T, S) )
        if (CanMapCollectionOrEnumerableToArray(out var collectionOrEnumerableToArrayElementStrategy))
        {
            return new EnumerableOrCollectionToArrayMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                collectionOrEnumerableToArrayElementStrategy);
        }

        // 17. S[]/List<S> -> Collection<T>/IEnumerable<T> : ArrayOrListToCollectionMapStrategy ( IMapStrategy(T, S) ).
        if (CanMapArrayOrListToCollectionOrEnumerable(out var arrayOrListElementStrategy))
        {
            // TODO: Check if it is possible using Span<> here as well.
            // TODO: Allow to prefer returning array over lists
            return new ArrayOrListToCollectionMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                arrayOrListElementStrategy);
        }

        // 18. IEnumerable<S>/Collection<S> -> Collection<T>/IEnumerable<T> : EnumerableOrCollectionToCollectionMapStrategy ( IMapStrategy(T, S) ).
        if (CanMapCollectionOrEnumerableToCollectionOrEnumerable(out var collectionOrEnumerableElementStrategy))
        {
            // TODO: Check if it is possible using Span<> here as well.
            // TODO: Allow to prefer returning array over lists
            return new EnumerableOrCollectionToCollectionMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                collectionOrEnumerableElementStrategy);
        }

        // 19. Dictionary<SK,SV> -> Dictionary<TK,TV> : DictionaryStrategy( IMapStrategy(TK, SK), IMapStrategy(TV, SV) ).
        if (CanMapDictionaryToDictionary(out var dictionaryKeyStrategy, out var dictionaryValueStrategy))
        {
            // TODO: Allow the user to specify if they want to use .Add or the indexer
            return new DictionaryToDictionaryMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                dictionaryKeyStrategy,
                dictionaryValueStrategy);
        }

        // 20. (S1, ..., SN) -> (T1, ..., TN) : TupleStrategy( IMapStrategy(T1, S1), ..., IMapStrategy(TN, SN))
        if (CanMapTupleToTuple(out var tupleElementStrategies))
        {
            return new TupleToTupleMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                tupleElementStrategies.ToArray());
        }

        // 21. (nullable enabled) (ref type) S? -> T? : ReferenceNullableToReferenceNullable( IMapStrategy(T,S) )
        // TODO: Implement me
        // 22. (nullable enabled) (ref type) S? -> T
        // TODO: Implement me
        // 23. (nullable enabled) (ref type) S -> T?
        // TODO: Implement me
        // 24. S -> T : ConstructorStrategy(S, T)
        // TODO: Implement me
        // Report error
        this.Context.ReportDiagnostic(MappaDiagnostics.CannotIdentifyStrategy(
            this.Context.TargetType,
            this.Context.SourceType,
            this.Context.GetLocation()));
        return new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);

        bool TryGetElementStrategy(out IMapStrategy elementStrategy)
        {
            var sourceElementType = this.Context.SourceType.GetElementType();
            var targetElementType = this.Context.TargetType.GetElementType();
            var context = new GenericMappaMethodGeneratorContext(
                this.Context,
                targetElementType,
                sourceElementType);
            var algorithm = new TypeMapIdentifierWithMapMethodAlgorithm(context, this.Compilation, this.CancellationToken);
            elementStrategy = algorithm.GetStrategy();
            return elementStrategy is not NoMapStrategy;
        }

        bool TryGetKeyAndValueStrategy(out IMapStrategy keyStrategy, out IMapStrategy valueStrategy)
        {
            var (sourceKeyType, sourceKeyValueType) = this.Context.SourceType.GetKeyAndValueTypes();
            var (targetKeyType, targetValueType) = this.Context.TargetType.GetKeyAndValueTypes();

            // Get strategy for key
            var keyContext = new GenericMappaMethodGeneratorContext(
                this.Context,
                targetKeyType,
                sourceKeyType);
            var keyAlgorithm = new TypeMapIdentifierWithMapMethodAlgorithm(keyContext, this.Compilation, this.CancellationToken);
            keyStrategy = keyAlgorithm.GetStrategy();

            // Get strategy for value
            var valueContext = new GenericMappaMethodGeneratorContext(
                this.Context,
                targetValueType,
                sourceKeyValueType);
            var valueAlgorithm = new TypeMapIdentifierWithMapMethodAlgorithm(valueContext, this.Compilation, this.CancellationToken);
            valueStrategy = valueAlgorithm.GetStrategy();

            return keyStrategy is not NoMapStrategy && valueStrategy is not NoMapStrategy;
        }

        bool CanMapUsingMapToSameTypeRule()
        {
            // (non-nullable): T -> T
            // (nullable): T -> T or T? -> T?
            // (nullable) T -> T? && T is refType
            // (nullable || not-nullable) T -> T? && T is not refType
            return (notNullableEnabled &&
                    SymbolEqualityComparer.Default.Equals(this.Context.TargetType, this.Context.SourceType))
                   || (nullableEnabled &&
                       SymbolEqualityComparer.IncludeNullability.Equals(
                           this.Context.TargetType,
                           this.Context.SourceType))
                   || (nullableEnabled && SymbolEqualityComparer.Default.Equals(
                                           this.Context.TargetType,
                                           this.Context.SourceType)
                                       && this.Context.TargetType is
                                           { NullableAnnotation: NullableAnnotation.Annotated, IsReferenceType: true })
                   || (this.Context.TargetType is
                           { NullableAnnotation: NullableAnnotation.Annotated, IsReferenceType: false }
                       && this.Context.TargetType.IsNullableGenericType(this.Context.SourceType, nullableEnabled));
        }

        bool CanMapUsingMapToObjectRule()
        {
            // (non-nullable): T -> object
            // (nullable): T -> object?
            // (nullable) T -> object (When T is not NOT nullable annotated)
            return (notNullableEnabled && this.Context.TargetType.IsObject())
                   || (nullableEnabled && this.Context.TargetType.IsObject() &&
                       this.Context.TargetType.NullableAnnotation == NullableAnnotation.Annotated)
                   || (nullableEnabled
                       && this.Context.TargetType.IsObject()
                       && this.Context.TargetType.NullableAnnotation == NullableAnnotation.NotAnnotated
                       && this.Context.SourceType.NullableAnnotation == NullableAnnotation.NotAnnotated);
        }

        bool CanMapUsingImplicitConversion()
        {
            return this.Compilation.HasImplicitConversion(this.Context.SourceType, this.Context.TargetType);
        }

        bool CanMapEnumToString()
        {
            var isEnum = this.Context.SourceType.IsEnum();
            var isString = this.Context.TargetType.IsString();
            return isEnum && isString;
        }

        bool CanMapEnumToIntegral()
        {
            var isSourceEnum = this.Context.SourceType.IsEnum();
            if (!isSourceEnum)
            {
                return false;
            }

            var enumUnderlyingType = ((INamedTypeSymbol)this.Context.SourceType).EnumUnderlyingType;
            return this.Compilation.HasImplicitConversion(enumUnderlyingType, this.Context.TargetType);
        }

        bool CanMapStringToEnum()
        {
            var isTargetEnum = this.Context.TargetType.IsEnum();
            var isSourceString = this.Context.SourceType.IsString();
            return isTargetEnum && isSourceString;
        }

        bool CanMapIntegralToEnum()
        {
            var isTargetEnum = this.Context.TargetType.IsEnum();
            if (!isTargetEnum)
            {
                return false;
            }

            var enumUnderlyingType = ((INamedTypeSymbol)this.Context.TargetType).EnumUnderlyingType;
            return this.Compilation.HasImplicitConversion(this.Context.SourceType, enumUnderlyingType);
        }

        bool CanMapEnumToEnum()
        {
            var isTargetEnum = this.Context.TargetType.IsEnum();
            var isSourceEnum = this.Context.SourceType.IsEnum();
            return isTargetEnum && isSourceEnum;
        }

        bool CanMapStringToNumber()
        {
            var isTargetDateTime = this.Context.TargetType.IsNumeric();
            var isSourceString = this.Context.SourceType.IsString();
            return isTargetDateTime && isSourceString;
        }

        bool CanMapStringToDateTime()
        {
            var isTargetDateTime = this.Context.TargetType.IsDateTime();
            var isSourceString = this.Context.SourceType.IsString();
            return isTargetDateTime && isSourceString;
        }

        bool CanMapToString()
        {
            var isTargetString = this.Context.TargetType.IsString();
            return isTargetString;
        }

        bool CanMapNullableToNullable(out IMapStrategy elementStrategy)
        {
            var isSourceNullable = this.Context.SourceType.IsNullable();
            var isTargetNullable = this.Context.TargetType.IsNullable();

            elementStrategy = new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);
            return isSourceNullable && isTargetNullable && TryGetElementStrategy(out elementStrategy);
        }

        bool CanMapArrayOrListToArray(out IMapStrategy elementStrategy)
        {
            // Source can be S[], IList<S>, List<S>
            var acceptSource = this.Context.SourceType.IsArray();
            acceptSource = acceptSource || this.Context.SourceType.IsIList();
            acceptSource = acceptSource || this.Context.SourceType.IsList(this.Compilation);

            var isTargetArray = this.Context.TargetType.IsArray();

            elementStrategy = new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);
            return acceptSource && isTargetArray && TryGetElementStrategy(out elementStrategy);
        }

        bool CanMapCollectionOrEnumerableToArray(out IMapStrategy elementStrategy)
        {
            // Source can be IEnumerable<S>, ICollection<S> or IReadOnlyCollection<S>
            var acceptSource = this.Context.SourceType.IsIEnumerable();
            acceptSource = acceptSource || this.Context.SourceType.IsICollection();
            acceptSource = acceptSource || this.Context.SourceType.IsIReadOnlyCollection();

            var isTargetArray = this.Context.TargetType.IsArray();

            elementStrategy = new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);
            return acceptSource && isTargetArray && TryGetElementStrategy(out elementStrategy);
        }

        bool CanMapArrayOrListToCollectionOrEnumerable(out IMapStrategy elementStrategy)
        {
            // Source can be S[], IList<S>, List<S>
            var acceptSource = this.Context.SourceType.IsArray();
            acceptSource = acceptSource || this.Context.SourceType.IsIList();
            acceptSource = acceptSource || this.Context.SourceType.IsList(this.Compilation);

            // Target can be IList<T>, List<T>, ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>
            var acceptTarget = this.Context.TargetType.IsIList();
            acceptTarget = acceptTarget || this.Context.TargetType.IsList(this.Compilation);
            acceptTarget = acceptTarget || this.Context.TargetType.IsICollection();
            acceptTarget = acceptTarget || this.Context.TargetType.IsIReadOnlyCollection();
            acceptTarget = acceptTarget || this.Context.TargetType.IsIEnumerable();

            // Return result of check.
            elementStrategy = new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);
            return acceptSource && acceptTarget && TryGetElementStrategy(out elementStrategy);
        }

        bool CanMapCollectionOrEnumerableToCollectionOrEnumerable(out IMapStrategy elementStrategy)
        {
            // Source can be S[], IList<S>, List<S>
            var acceptSource = this.Context.SourceType.IsIEnumerable();
            acceptSource = acceptSource || this.Context.SourceType.IsICollection();
            acceptSource = acceptSource || this.Context.SourceType.IsIReadOnlyCollection();

            // Target can be IList<T>, List<T>, ICollection<T>, IReadOnlyCollection<T>, IEnumerable<T>
            var acceptTarget = this.Context.TargetType.IsIList();
            acceptTarget = acceptTarget || this.Context.TargetType.IsList(this.Compilation);
            acceptTarget = acceptTarget || this.Context.TargetType.IsICollection();
            acceptTarget = acceptTarget || this.Context.TargetType.IsIReadOnlyCollection();
            acceptTarget = acceptTarget || this.Context.TargetType.IsIEnumerable();

            // Return result of check.
            elementStrategy = new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);
            return acceptSource && acceptTarget && TryGetElementStrategy(out elementStrategy);
        }

        bool CanMapDictionaryToDictionary(out IMapStrategy keyStrategy, out IMapStrategy valueStrategy)
        {
            var isSourceDictionary = this.Context.SourceType.IsIDictionary(this.Compilation)
                                    || this.Context.SourceType.IsDictionary(this.Compilation);
            var isTargetDictionary = this.Context.TargetType.IsIDictionary(this.Compilation)
                                    || this.Context.TargetType.IsDictionary(this.Compilation);

            keyStrategy = new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);
            valueStrategy = new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);

            return isSourceDictionary && isTargetDictionary && TryGetKeyAndValueStrategy(out keyStrategy, out valueStrategy);
        }

        bool CanMapTupleToTuple(out IList<IMapStrategy> elementsStrategies)
        {
            var isSourceTuple = this.Context.SourceType.IsTuple(this.Compilation);
            var isTargetTuple = this.Context.TargetType.IsTuple(this.Compilation);

            var tmpTupleStrategies = elementsStrategies = new List<IMapStrategy>();

            return isSourceTuple
                   && isTargetTuple
                   && this.Context.SourceType.TryGetTypeArguments(out var sourceTypeArguments)
                   && this.Context.TargetType.TryGetTypeArguments(out var targetTypeArguments)
                   && sourceTypeArguments.Length == targetTypeArguments.Length
                   && Enumerable.Range(0, sourceTypeArguments.Length)
                       .All(index =>
                   {
                       var elementContext = new GenericMappaMethodGeneratorContext(
                           this.Context,
                           targetTypeArguments[index],
                           sourceTypeArguments[index]);
                       var elementAlgorithm = new TypeMapIdentifierWithMapMethodAlgorithm(elementContext, this.Compilation, this.CancellationToken);
                       var elementStrategy = elementAlgorithm.GetStrategy();

                       if (elementStrategy is NoMapStrategy)
                       {
                           return false;
                       }

                       tmpTupleStrategies.Add(elementStrategy);
                       return true;
                   });
        }
    }
}