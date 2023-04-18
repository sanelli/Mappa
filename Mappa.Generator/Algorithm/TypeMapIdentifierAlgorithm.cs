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
                this.Context.SourceType,
                this.Context.SourcePropertyName);
        }

        // 02. Map to object
        if (CanMapUsingMapToObjectRule())
        {
            return new IdentityMapStrategy(
                MappaAlgorithmRule.MapToObject,
                this.Context.TargetType,
                this.Context.SourceType,
                this.Context.SourcePropertyName);
        }

        // 03. Implicit conversion.
        if (CanMapUsingImplicitConversion())
        {
            return new IdentityMapStrategy(
                MappaAlgorithmRule.ImplicitConversion,
                this.Context.TargetType,
                this.Context.SourceType,
                this.Context.SourcePropertyName);
        }

        // 04. enum -> string : EnumToString strategy.
        if (CanMapEnumToString())
        {
            // TODO: Add ability to use content of Description attribute
            return new EnumToStringMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                this.Context.SourcePropertyName);
        }

        // 05. enum -> implicit-convertible-integral : EnumToIntegral strategy.
        if (CanMapEnumToIntegral())
        {
            return new EnumToIntegralMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                this.Context.SourcePropertyName);
        }

        // 06. string -> enum : StringToEnum strategy.
        if (CanMapStringToEnum())
        {
            // TODO: Add ability to use content of Description attribute
            return new StringToEnumMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                this.Context.SourcePropertyName);
        }

        // 07. integral -> enum : IntegralToEnum strategy.
        if (CanMapIntegralToEnum())
        {
            return new IntegralToEnumMapStrategy(
                this.Context.TargetType,
                this.Context.SourceType,
                this.Context.SourcePropertyName);
        }

        // XX. enum -> enum: EnumToEnumStrategy
        // XX. string -> numeric : ParseNumberStrategy
        // XX. S -> string : InvokeToStringStrategy
        // XX. (struct) S? -> T? : NullableStructStrategy( Strategy(T, S) )
        // XX. S[] -> T[] : ArrayStrategy ( Strategy(T, S) ).
        // XX. List<S> -> List<T> : ListStrategy ( Strategy(T, S) ).
        // XX. Dictionary<SK,SV> -> Dictionary<TK,TV> : DictionaryStrategy( Strategy(TK, SK), Strategy(TV, SV) ).
        // XX. S -> T : ConstructorStrategy(S, T)
        // XX. Report error
        this.Context.ReportDiagnostic(MappaDiagnostics.CannotIdentifyStrategy(
            this.Context.TargetType,
            this.Context.TargetPropertyName,
            this.Context.SourceType,
            this.Context.SourcePropertyName,
            this.Context.GetLocation()));
        return new NoMapStrategy(this.Context.TargetType, this.Context.SourceType, this.Context.SourcePropertyName);

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
            var isEnum = this.Context.SourceType.IsEnum();
            if (!isEnum)
            {
                return false;
            }

            var enumUnderlyingType = ((INamedTypeSymbol)this.Context.SourceType).EnumUnderlyingType;
            return this.Compilation.HasImplicitConversion(enumUnderlyingType, this.Context.TargetType);
        }

        bool CanMapStringToEnum()
        {
            var isEnum = this.Context.TargetType.IsEnum();
            var isString = this.Context.SourceType.IsString();
            return isEnum && isString;
        }

        bool CanMapIntegralToEnum()
        {
            var isEnum = this.Context.TargetType.IsEnum();
            if (!isEnum)
            {
                return false;
            }

            var enumUnderlyingType = ((INamedTypeSymbol)this.Context.TargetType).EnumUnderlyingType;
            return this.Compilation.HasImplicitConversion(this.Context.SourceType, enumUnderlyingType);
        }
    }
}