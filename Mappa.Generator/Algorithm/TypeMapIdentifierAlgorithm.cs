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

        // 12. (struct) S? -> T? : NullableToNullableStrategy( Strategy(T, S) )
        if (CanMapNullableToNullable())
        {
            var sourceTypeFirstGenericType = this.Context.SourceType.GetFirstGenericType();
            var targetTypeFirstGenericType = this.Context.TargetType.GetFirstGenericType();

            var context = new GenericMappaMethodGeneratorContext(
                this.Context,
                targetTypeFirstGenericType,
                sourceTypeFirstGenericType,
                this.Context.TargetPropertyName,
                $"{this.Context.SourcePropertyName}.Value");
            var algorithm = new TypeMapIdentifierWithMapMethodAlgorithm(context, this.Compilation, this.CancellationToken);
            var innerStrategy = algorithm.GetStrategy();

            if (innerStrategy is NoMapStrategy noMapStrategy)
            {
                return noMapStrategy;
            }

            return new NullableToNullableMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                innerStrategy);
        }

        // 13. S[] -> T[] : ArrayStrategy ( Strategy(T, S) ).
        // 14. List<S> -> List<T> : ListStrategy ( Strategy(T, S) ).
        // 15. Dictionary<SK,SV> -> Dictionary<TK,TV> : DictionaryStrategy( Strategy(TK, SK), Strategy(TV, SV) ).
        // 16. S -> T : ConstructorStrategy(S, T)
        // 17. Report error
        this.Context.ReportDiagnostic(MappaDiagnostics.CannotIdentifyStrategy(
            this.Context.TargetType,
            this.Context.TargetPropertyName,
            this.Context.SourceType,
            this.Context.SourcePropertyName,
            this.Context.GetLocation()));
        return new NoMapStrategy(this.Context.TargetType, this.Context.SourceType);

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

        bool CanMapNullableToNullable()
        {
            var isSourceNullable = this.Context.SourceType.IsNullable();
            var isTargetNullable = this.Context.TargetType.IsNullable();
            return isSourceNullable && isTargetNullable;
        }
    }
}