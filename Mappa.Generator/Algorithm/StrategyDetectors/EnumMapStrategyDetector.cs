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
            return this.TryCreateEnumToStringStrategy(out mapStrategy);
        }

        // 02. enum -> implicit-convertible-integral : EnumToIntegral strategy.
        if (this.CanMapEnumToIntegral())
        {
            mapStrategy = new EnumToIntegralMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 03. string -> enum : StringToEnum strategy.
        else if (this.CanMapStringToEnum())
        {
            return this.TryCreateStringToEnumStrategy(out mapStrategy);
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
            return this.TryCreateEnumToEnumStrategy(out mapStrategy);
        }

        return mapStrategy is not NoMapStrategy;
    }

    private static EnumStringMapSetting GetEffectiveEnumStringMapSetting(EnumStringMapSetting enumStringMapSetting)
        => enumStringMapSetting is EnumStringMapSetting.Undefined
            ? EnumStringMapSetting.MemberName
            : enumStringMapSetting;

    private static EnumToEnumMapSetting GetEffectiveEnumToEnumMapSetting(EnumToEnumMapSetting enumToEnumMapSetting)
        => enumToEnumMapSetting is EnumToEnumMapSetting.Undefined
            ? EnumToEnumMapSetting.MemberName
            : enumToEnumMapSetting;

    private static string FormatMemberNames(IReadOnlyList<string> memberNames)
        => string.Join(", ", memberNames.Select(name => $"'{name}'"));

    private bool TryCreateEnumToStringStrategy(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        var settings = this.context.MappaUserSettings;
        var effectiveEnumStringMapSetting = GetEffectiveEnumStringMapSetting(settings.EnumStringMapSetting);
        var caseInsensitive = settings.CaseInsensitiveEnumMap is BooleanSetting.Enable;

        if (effectiveEnumStringMapSetting is EnumStringMapSetting.Description
            && !this.ValidateDescriptionMapping(this.context.SourceType, caseInsensitive))
        {
            return false;
        }

        mapStrategy = new EnumToStringMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            settings.EnumStringMapSetting);
        return true;
    }

    private bool TryCreateStringToEnumStrategy(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        var settings = this.context.MappaUserSettings;
        var effectiveEnumStringMapSetting = GetEffectiveEnumStringMapSetting(settings.EnumStringMapSetting);
        var caseInsensitive = settings.CaseInsensitiveEnumMap is BooleanSetting.Enable;

        if (effectiveEnumStringMapSetting is EnumStringMapSetting.Description
            && !this.ValidateDescriptionMapping(this.context.TargetType, caseInsensitive))
        {
            return false;
        }

        mapStrategy = new StringToEnumMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            settings.CaseInsensitiveEnumMap,
            settings.EnumStringMapSetting);
        return true;
    }

    private bool TryCreateEnumToEnumStrategy(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        var settings = this.context.MappaUserSettings;
        var effectiveEnumToEnumMapSetting = GetEffectiveEnumToEnumMapSetting(settings.EnumToEnumMapSetting);
        var caseInsensitive = settings.CaseInsensitiveEnumMap is BooleanSetting.Enable;

        if (effectiveEnumToEnumMapSetting is EnumToEnumMapSetting.Description)
        {
            if (!this.ValidateDescriptionMapping(this.context.SourceType, caseInsensitive)
                || !this.ValidateDescriptionMapping(this.context.TargetType, caseInsensitive))
            {
                return false;
            }

            if (this.context.SourceType.HasAmbiguousEnumMemberDescriptionMap(
                this.context.TargetType,
                this.compilation,
                caseInsensitive,
                out var ambiguityDetails))
            {
                this.ReportAmbiguousEnumMap(ambiguityDetails);
                return false;
            }
        }
        else if (effectiveEnumToEnumMapSetting is EnumToEnumMapSetting.MemberName
                 && caseInsensitive
                 && this.context.SourceType.HasAmbiguousEnumMemberNameCaseInsensitiveMap(
                     this.context.TargetType,
                     out var ambiguityDetails))
        {
            this.ReportAmbiguousEnumMap(ambiguityDetails);
            return false;
        }

        mapStrategy = new EnumToEnumMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            settings.EnumToEnumMapSetting,
            settings.CaseInsensitiveEnumMap);
        this.ReportUnmappedSourceEnumMembersIfAny();
        return true;
    }

    private bool ValidateDescriptionMapping(ITypeSymbol enumType, bool caseInsensitive)
    {
        var missingMemberNames = enumType.GetEnumMemberNamesMissingDescription(this.compilation);
        if (missingMemberNames.Length > 0)
        {
            this.context.ReportDiagnostic(
                MappaDiagnostics.EnumMemberMissingDescription(
                    this.context.GetRootMapMethod().MethodDeclarationSyntax,
                    enumType.ToDisplayString(),
                    FormatMemberNames(missingMemberNames)));
            return false;
        }

        var duplicateDescriptionGroups = enumType.GetDuplicateDescriptionGroups(this.compilation, caseInsensitive);
        if (duplicateDescriptionGroups.Length > 0)
        {
            this.ReportAmbiguousEnumMap(
                $"Enum '{enumType.ToDisplayString()}' has duplicate Description values for members: {string.Join("; ", duplicateDescriptionGroups)}.");
            return false;
        }

        return true;
    }

    private void ReportAmbiguousEnumMap(string ambiguityDetails)
        => this.context.ReportDiagnostic(
            MappaDiagnostics.AmbiguousEnumMap(
                this.context.GetRootMapMethod().MethodDeclarationSyntax,
                ambiguityDetails));

    private void ReportUnmappedSourceEnumMembersIfAny()
    {
        var settings = this.context.MappaUserSettings;
        var effectiveEnumToEnumMapSetting = GetEffectiveEnumToEnumMapSetting(settings.EnumToEnumMapSetting);
        var caseInsensitive = settings.CaseInsensitiveEnumMap is BooleanSetting.Enable;

        var unmappedMemberNames = effectiveEnumToEnumMapSetting switch
        {
            EnumToEnumMapSetting.NumericValue => this.context.SourceType.GetUnmappedEnumMemberNamesByValue(this.context.TargetType),
            EnumToEnumMapSetting.Description => this.context.SourceType.GetUnmappedEnumMemberNamesByDescription(
                this.context.TargetType,
                this.compilation,
                caseInsensitive),
            _ when caseInsensitive => this.GetUnmappedEnumMemberNamesByNameCaseInsensitive(),
            _ => this.context.SourceType.GetUnmappedEnumMemberNamesByName(this.context.TargetType),
        };
        if (unmappedMemberNames.Length == 0)
        {
            return;
        }

        var formattedUnmappedMemberNames = FormatMemberNames(unmappedMemberNames);
        this.context.ReportDiagnostic(
            MappaDiagnostics.NotAllSourceEnumMembersCanBeMapped(
                this.context.GetRootMapMethod().MethodDeclarationSyntax,
                this.context.SourceType.ToDisplayString(),
                this.context.TargetType.ToDisplayString(),
                formattedUnmappedMemberNames));
    }

    private string[] GetUnmappedEnumMemberNamesByNameCaseInsensitive()
    {
        var mappedSourceMemberNames = new HashSet<string>(
            this.context.SourceType.GetSharedEnumMemberMappingsByNameCaseInsensitive(this.context.TargetType)
                .Select(mapping => mapping.SourceMemberName),
            StringComparer.Ordinal);

        return this.context.SourceType.GetEnumValues()
            .Select(enumValue => enumValue.Name)
            .Where(sourceMemberName => !mappedSourceMemberNames.Contains(sourceMemberName))
            .OrderBy(sourceMemberName => sourceMemberName)
            .ToArray();
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