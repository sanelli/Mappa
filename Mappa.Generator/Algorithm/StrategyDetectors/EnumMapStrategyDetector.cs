// <copyright file="EnumMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for enum related strategies.
/// </summary>
internal sealed class EnumMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    public EnumMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation)
    {
        this.context = context;
        this.compilation = compilation;
    }

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // 01. enum -> string : EnumToString strategy.
        if (this.CanMapEnumToString())
        {
            // TODO [#15] Support using the Description attribute when mapping enum to string.
            mapStrategy = new EnumToStringMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 02. enum -> implicit-convertible-integral : EnumToIntegral strategy.
        else if (this.CanMapEnumToIntegral())
        {
            mapStrategy = new EnumToIntegralMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 03. string -> enum : StringToEnum strategy.
        else if (this.CanMapStringToEnum())
        {
            // TODO [#16] Support using the Description attribute when mapping string to enum.
            mapStrategy = new StringToEnumMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                this.context.MappaUserSettings.CaseInsensitiveStringToEnumMap);
        }

        // 04. integral -> enum : IntegralToEnum strategy.
        else if (this.CanMapIntegralToEnum())
        {
            mapStrategy = new IntegralToEnumMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 05. enum -> enum: EnumToEnumStrategy
        else if (this.CanMapEnumToEnum())
        {
            // TODO [#18] Allow to map the source enum to the target enum using numeric values instead than their name.
            mapStrategy = new EnumToEnumMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
            this.ReportUnmappedSourceEnumMembersIfAny();
        }

        return mapStrategy is not NoMapStrategy;
    }

    private void ReportUnmappedSourceEnumMembersIfAny()
    {
        var unmappedMemberNames = this.context.SourceType.GetUnmappedEnumMemberNamesByName(this.context.TargetType);
        if (unmappedMemberNames.Length == 0)
        {
            return;
        }

        var formattedUnmappedMemberNames = string.Join(", ", unmappedMemberNames.Select(name => $"'{name}'"));
        this.context.ReportDiagnostic(
            MappaDiagnostics.NotAllSourceEnumMembersCanBeMapped(
                this.context.GetRootMapMethod().MethodDeclarationSyntax,
                this.context.SourceType.ToDisplayString(),
                this.context.TargetType.ToDisplayString(),
                formattedUnmappedMemberNames));
    }

    private bool CanMapEnumToString()
    {
        var isEnum = this.context.SourceType.IsEnum();
        var isString = this.context.TargetType.IsString();
        return isEnum && isString;
    }

    private bool CanMapEnumToIntegral()
    {
        var isSourceEnum = this.context.SourceType.IsEnum();
        if (!isSourceEnum)
        {
            return false;
        }

        var enumUnderlyingType = ((INamedTypeSymbol)this.context.SourceType).EnumUnderlyingType;
        return this.compilation.HasImplicitConversion(enumUnderlyingType, this.context.TargetType);
    }

    private bool CanMapStringToEnum()
    {
        var isTargetEnum = this.context.TargetType.IsEnum();
        var isSourceString = this.context.SourceType.IsString();
        return isTargetEnum && isSourceString;
    }

    private bool CanMapIntegralToEnum()
    {
        var isTargetEnum = this.context.TargetType.IsEnum();
        if (!isTargetEnum)
        {
            return false;
        }

        var enumUnderlyingType = ((INamedTypeSymbol)this.context.TargetType).EnumUnderlyingType;
        return this.compilation.HasImplicitConversion(this.context.SourceType, enumUnderlyingType);
    }

    private bool CanMapEnumToEnum()
    {
        var isTargetEnum = this.context.TargetType.IsEnum();
        var isSourceEnum = this.context.SourceType.IsEnum();
        return isTargetEnum && isSourceEnum;
    }
}