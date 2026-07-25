// <copyright file="EnumMapConfigurationResolver.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Algorithm;

/// <summary>
/// Resolves the <see cref="EnumMapConfiguration"/> for an enum mapping leg by merging the
/// <see cref="MappaUserSettings"/> based pairing with the per-member overrides declared on the
/// root map method.
/// </summary>
internal sealed class EnumMapConfigurationResolver
{
    private const string MapEnumMemberAttributeName = "MappaMapEnumMember";
    private const string MapEnumIgnoreAttributeName = "MappaMapEnumIgnore";
    private const string MapEnumDefaultAttributeName = "MappaMapEnumDefault";

    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;
    private readonly EnumMapLegKind legKind;
    private readonly MapMethod rootMapMethod;
    private readonly MethodDeclarationSyntax? methodDeclarationSyntax;
    private readonly string methodName;
    private readonly INamedTypeSymbol? sourceEnumType;
    private readonly INamedTypeSymbol? targetEnumType;

    private EnumMapConfigurationResolver(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        EnumMapLegKind legKind)
    {
        this.context = context;
        this.compilation = compilation;
        this.legKind = legKind;
        this.rootMapMethod = context.GetRootMapMethod();
        this.methodDeclarationSyntax = this.rootMapMethod.MethodDeclarationSyntax;
        this.methodName = this.rootMapMethod.MethodName;
        this.sourceEnumType = legKind is EnumMapLegKind.EnumToEnum
            or EnumMapLegKind.EnumToString
            or EnumMapLegKind.EnumToIntegral
            ? context.SourceType as INamedTypeSymbol
            : null;
        this.targetEnumType = legKind is EnumMapLegKind.EnumToEnum
            or EnumMapLegKind.StringToEnum
            or EnumMapLegKind.IntegralToEnum
            ? context.TargetType as INamedTypeSymbol
            : null;
    }

    /// <summary>
    /// Resolves the enum mapping configuration for the current mapping leg.
    /// </summary>
    /// <param name="context">The mapping algorithm context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="legKind">The mapping leg being resolved.</param>
    /// <param name="configuration">The resolved configuration.</param>
    /// <returns><c>true</c> when the configuration is valid, <c>false</c> when a diagnostic error has been reported.</returns>
    internal static bool TryResolve(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        EnumMapLegKind legKind,
        out EnumMapConfiguration configuration)
        => new EnumMapConfigurationResolver(context, compilation, legKind)
            .TryResolveConfiguration(out configuration);

    private static EnumStringMapSetting GetEffectiveEnumStringMapSetting(EnumStringMapSetting enumStringMapSetting)
        => enumStringMapSetting is EnumStringMapSetting.Undefined
            ? EnumStringMapSetting.MemberName
            : enumStringMapSetting;

    private static EnumToEnumMapSetting GetEffectiveEnumToEnumMapSetting(EnumToEnumMapSetting enumToEnumMapSetting)
        => enumToEnumMapSetting is EnumToEnumMapSetting.Undefined
            ? EnumToEnumMapSetting.MemberName
            : enumToEnumMapSetting;

    private static bool AreSameType(ISymbol? left, ISymbol? right)
        => left is not null && right is not null && SymbolEqualityComparer.Default.Equals(left, right);

    private static string FormatIntegral(object value)
        => Convert.ToString(value, CultureInfo.InvariantCulture) ?? "0";

    private static HashSet<string> GetIgnoredMemberNames(
        EnumMapIgnoreInfoAttribute[] ignoreAttributes,
        INamedTypeSymbol? enumType)
        => new(
            ignoreAttributes
                .Where(ignoreAttribute => AreSameType(ignoreAttribute.EnumType, enumType))
                .Select(ignoreAttribute => ignoreAttribute.EnumMemberName),
            StringComparer.Ordinal);

    private static void ApplyOverrides(
        List<(string MemberName, string ValueExpression)> pairs,
        IEnumerable<EnumMapMemberInfoAttribute> memberAttributes,
        Func<EnumMapMemberInfoAttribute, string> valueExpressionSelector)
    {
        foreach (var memberAttribute in memberAttributes)
        {
            var valueExpression = valueExpressionSelector(memberAttribute);
            var index = pairs.FindIndex(pair => pair.MemberName.Equals(memberAttribute.EnumMemberName, StringComparison.Ordinal));
            if (index >= 0)
            {
                pairs[index] = (memberAttribute.EnumMemberName, valueExpression);
            }
            else
            {
                pairs.Add((memberAttribute.EnumMemberName, valueExpression));
            }
        }
    }

    private bool TryResolveConfiguration(out EnumMapConfiguration configuration)
    {
        configuration = new EnumMapConfiguration([], MappaMapEnumDefaultBehavior.Throw, null, []);

        var memberAttributes = this.rootMapMethod.GetAttributes<EnumMapMemberInfoAttribute>();
        var ignoreAttributes = this.rootMapMethod.GetAttributes<EnumMapIgnoreInfoAttribute>();
        var defaultAttributes = this.rootMapMethod.GetAttributes<EnumMapDefaultInfoAttribute>();

        if (!this.ValidateDefaultAttributeMultiplicity(defaultAttributes)
            || !this.ValidateAttributeEnumTypes(memberAttributes, ignoreAttributes, defaultAttributes))
        {
            return false;
        }

        var relevantMemberAttributes = memberAttributes
            .Where(this.IsRelevantMemberAttribute)
            .ToArray();
        var ignoredSourceMemberNames = GetIgnoredMemberNames(ignoreAttributes, this.sourceEnumType);
        var ignoredTargetMemberNames = GetIgnoredMemberNames(ignoreAttributes, this.targetEnumType);

        if (!this.ValidateNoMemberClashes(relevantMemberAttributes)
            || !this.ValidateIgnoreDoesNotConflict(relevantMemberAttributes, ignoredSourceMemberNames, ignoredTargetMemberNames))
        {
            return false;
        }

        if (!this.TryResolveDefault(defaultAttributes, out var defaultBehavior, out var defaultAssignmentExpression))
        {
            return false;
        }

        var cases = this.BuildCases(relevantMemberAttributes, ignoredSourceMemberNames, ignoredTargetMemberNames);
        if (relevantMemberAttributes.Length > 0 && !this.ValidateNoDuplicateCases(cases))
        {
            return false;
        }

        configuration = new EnumMapConfiguration(
            cases,
            defaultBehavior,
            defaultAssignmentExpression,
            [.. ignoredSourceMemberNames.OrderBy(name => name, StringComparer.Ordinal)]);
        return true;
    }

    private bool IsDirectEnumMap()
        => this.rootMapMethod.SourceType.IsEnum() || this.rootMapMethod.TargetType.IsEnum();

    private bool MatchesActiveEnum(INamedTypeSymbol enumType)
        => AreSameType(enumType, this.sourceEnumType) || AreSameType(enumType, this.targetEnumType);

    private bool ValidateDefaultAttributeMultiplicity(EnumMapDefaultInfoAttribute[] defaultAttributes)
    {
        if (defaultAttributes.Length <= 1)
        {
            return true;
        }

        if (this.IsDirectEnumMap())
        {
            this.context.ReportDiagnostic(MappaDiagnostics.TooManyEnumMapDefaultAttributesOnDirectEnumMap(
                this.methodDeclarationSyntax,
                this.methodName,
                defaultAttributes.Length));
            return false;
        }

        var duplicatedEnumTypes = defaultAttributes
            .GroupBy(attribute => attribute.EnumType, SymbolEqualityComparer.Default)
            .Where(group => group.Count() > 1)
            .Select(group => group.First().EnumType)
            .ToArray();
        foreach (var duplicatedEnumType in duplicatedEnumTypes)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.DuplicateEnumMapDefaultAttribute(
                this.methodDeclarationSyntax,
                this.methodName,
                duplicatedEnumType.ToDisplayString()));
        }

        return duplicatedEnumTypes.Length == 0;
    }

    private bool ValidateAttributeEnumTypes(
        EnumMapMemberInfoAttribute[] memberAttributes,
        EnumMapIgnoreInfoAttribute[] ignoreAttributes,
        EnumMapDefaultInfoAttribute[] defaultAttributes)
    {
        if (!this.IsDirectEnumMap())
        {
            return true;
        }

        var isValid = true;
        foreach (var memberAttribute in memberAttributes)
        {
            isValid &= this.ValidateAttributeEnumType(MapEnumMemberAttributeName, memberAttribute.EnumType);
            if (memberAttribute.OtherEnumType is { } otherEnumType)
            {
                isValid &= this.ValidateAttributeEnumType(MapEnumMemberAttributeName, otherEnumType);
            }
        }

        foreach (var ignoreAttribute in ignoreAttributes)
        {
            isValid &= this.ValidateAttributeEnumType(MapEnumIgnoreAttributeName, ignoreAttribute.EnumType);
        }

        foreach (var defaultAttribute in defaultAttributes)
        {
            isValid &= this.ValidateAttributeEnumType(MapEnumDefaultAttributeName, defaultAttribute.EnumType);
        }

        return isValid;
    }

    private bool ValidateAttributeEnumType(string attributeName, INamedTypeSymbol enumType)
    {
        if (this.MatchesActiveEnum(enumType))
        {
            return true;
        }

        this.context.ReportDiagnostic(MappaDiagnostics.EnumMapAttributeEnumTypeMismatch(
            this.methodDeclarationSyntax,
            this.methodName,
            attributeName,
            enumType.ToDisplayString(),
            this.context.SourceType.ToDisplayString(),
            this.context.TargetType.ToDisplayString()));
        return false;
    }

    private bool IsRelevantMemberAttribute(EnumMapMemberInfoAttribute memberAttribute)
        => this.legKind switch
        {
            EnumMapLegKind.EnumToEnum => this.TryGetEnumToEnumPair(memberAttribute, out _, out _),
            EnumMapLegKind.EnumToString or EnumMapLegKind.StringToEnum
                => memberAttribute.OtherEnumType is null
                   && memberAttribute.StringValue is not null
                   && this.MatchesActiveEnum(memberAttribute.EnumType),
            _ => memberAttribute.OtherEnumType is null
                 && memberAttribute.IntegerValue.HasValue
                 && this.MatchesActiveEnum(memberAttribute.EnumType),
        };

    private bool TryGetEnumToEnumPair(
        EnumMapMemberInfoAttribute memberAttribute,
        out string sourceMemberName,
        out string targetMemberName)
    {
        sourceMemberName = string.Empty;
        targetMemberName = string.Empty;

        if (memberAttribute.OtherEnumType is not { } otherEnumType
            || memberAttribute.OtherEnumMemberName is not { } otherEnumMemberName)
        {
            return false;
        }

        if (AreSameType(memberAttribute.EnumType, this.sourceEnumType)
            && AreSameType(otherEnumType, this.targetEnumType))
        {
            sourceMemberName = memberAttribute.EnumMemberName;
            targetMemberName = otherEnumMemberName;
            return true;
        }

        if (AreSameType(memberAttribute.EnumType, this.targetEnumType)
            && AreSameType(otherEnumType, this.sourceEnumType))
        {
            sourceMemberName = otherEnumMemberName;
            targetMemberName = memberAttribute.EnumMemberName;
            return true;
        }

        return false;
    }

    private bool ValidateNoMemberClashes(EnumMapMemberInfoAttribute[] relevantMemberAttributes)
    {
        if (this.legKind is EnumMapLegKind.EnumToEnum)
        {
            return this.ValidateNoDuplicates(
                relevantMemberAttributes
                    .Select(memberAttribute =>
                    {
                        this.TryGetEnumToEnumPair(memberAttribute, out var sourceMemberName, out _);
                        return sourceMemberName;
                    }),
                duplicate => $"enum member '{duplicate}' is configured more than once");
        }

        var membersAreValid = this.ValidateNoDuplicates(
            relevantMemberAttributes.Select(memberAttribute => memberAttribute.EnumMemberName),
            duplicate => $"enum member '{duplicate}' is configured more than once");
        var valuesAreValid = this.ValidateNoDuplicates(
            relevantMemberAttributes.Select(this.GetPairedValueKey),
            duplicate => $"value '{duplicate}' is configured more than once");
        return membersAreValid && valuesAreValid;
    }

    private string GetPairedValueKey(EnumMapMemberInfoAttribute memberAttribute)
    {
        if (this.legKind is EnumMapLegKind.EnumToString or EnumMapLegKind.StringToEnum)
        {
            var stringValue = memberAttribute.StringValue ?? string.Empty;
            return this.context.MappaUserSettings.CaseInsensitiveEnumMap is BooleanSetting.Enable
                ? stringValue.ToUpperInvariant()
                : stringValue;
        }

        return (memberAttribute.IntegerValue ?? 0).ToString(CultureInfo.InvariantCulture);
    }

    private bool ValidateNoDuplicates(IEnumerable<string> keys, Func<string, string> detailsBuilder)
    {
        var duplicates = keys
            .GroupBy(key => key, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();
        foreach (var duplicate in duplicates)
        {
            this.ReportMemberMappingClash(detailsBuilder(duplicate));
        }

        return duplicates.Length == 0;
    }

    private bool ValidateIgnoreDoesNotConflict(
        EnumMapMemberInfoAttribute[] relevantMemberAttributes,
        HashSet<string> ignoredSourceMemberNames,
        HashSet<string> ignoredTargetMemberNames)
    {
        var isValid = true;
        foreach (var memberAttribute in relevantMemberAttributes)
        {
            if (this.legKind is EnumMapLegKind.EnumToEnum)
            {
                this.TryGetEnumToEnumPair(memberAttribute, out var sourceMemberName, out var targetMemberName);
                if (ignoredSourceMemberNames.Contains(sourceMemberName))
                {
                    isValid = false;
                    this.ReportIgnoreConflict(this.sourceEnumType, sourceMemberName);
                }

                if (ignoredTargetMemberNames.Contains(targetMemberName))
                {
                    isValid = false;
                    this.ReportIgnoreConflict(this.targetEnumType, targetMemberName);
                }

                continue;
            }

            var configuredEnumType = this.sourceEnumType ?? this.targetEnumType;
            if (ignoredSourceMemberNames.Contains(memberAttribute.EnumMemberName)
                || ignoredTargetMemberNames.Contains(memberAttribute.EnumMemberName))
            {
                isValid = false;
                this.ReportIgnoreConflict(configuredEnumType, memberAttribute.EnumMemberName);
            }
        }

        return isValid;
    }

    private bool TryResolveDefault(
        EnumMapDefaultInfoAttribute[] defaultAttributes,
        out MappaMapEnumDefaultBehavior defaultBehavior,
        out string? defaultAssignmentExpression)
    {
        defaultBehavior = MappaMapEnumDefaultBehavior.Throw;
        defaultAssignmentExpression = null;

        var relevantDefaultAttributes = defaultAttributes
            .Where(defaultAttribute => this.MatchesActiveEnum(defaultAttribute.EnumType))
            .ToArray();
        if (relevantDefaultAttributes.Length == 0)
        {
            return true;
        }

        var defaultAttributeInfo = relevantDefaultAttributes
                .FirstOrDefault(defaultAttribute => AreSameType(defaultAttribute.EnumType, this.targetEnumType))
            ?? relevantDefaultAttributes[0];

        if (defaultAttributeInfo.Behavior is not MappaMapEnumDefaultBehavior.UseDefaultValue)
        {
            if (defaultAttributeInfo.HasDefaultValue)
            {
                this.context.ReportDiagnostic(MappaDiagnostics.EnumMapDefaultAttributeUnusedDefaultValue(
                    this.methodDeclarationSyntax,
                    this.methodName,
                    defaultAttributeInfo.EnumType.ToDisplayString()));
            }

            return true;
        }

        if (!defaultAttributeInfo.HasDefaultValue)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.EnumMapDefaultBehaviorRequiresDefaultValue(
                this.methodDeclarationSyntax,
                this.methodName,
                defaultAttributeInfo.EnumType.ToDisplayString()));
            return false;
        }

        if (!this.TryBuildDefaultAssignmentExpression(defaultAttributeInfo, out defaultAssignmentExpression))
        {
            this.context.ReportDiagnostic(MappaDiagnostics.EnumMapDefaultValueConstructorMismatch(
                this.methodDeclarationSyntax,
                this.methodName,
                defaultAttributeInfo.EnumType.ToDisplayString(),
                this.context.TargetType.ToDisplayString()));
            return false;
        }

        defaultBehavior = MappaMapEnumDefaultBehavior.UseDefaultValue;
        return true;
    }

    private bool TryBuildDefaultAssignmentExpression(
        EnumMapDefaultInfoAttribute defaultAttributeInfo,
        out string? defaultAssignmentExpression)
    {
        defaultAssignmentExpression = null;

        switch (this.legKind)
        {
            case EnumMapLegKind.EnumToString:
                if (defaultAttributeInfo.StringDefaultValue is { } stringDefaultValue)
                {
                    defaultAssignmentExpression = TypeSymbolExtensions.ToCSharpStringLiteral(stringDefaultValue);
                }

                break;

            case EnumMapLegKind.EnumToIntegral:
                if (defaultAttributeInfo.IntegerDefaultValue is { } integerDefaultValue)
                {
                    defaultAssignmentExpression = integerDefaultValue.ToString(CultureInfo.InvariantCulture);
                }

                break;

            default:
                if (defaultAttributeInfo.EnumDefaultMemberName is { } enumDefaultMemberName
                    && AreSameType(defaultAttributeInfo.EnumType, this.targetEnumType))
                {
                    defaultAssignmentExpression = $"{this.context.TargetType.ToDisplayString()}.{enumDefaultMemberName}";
                }

                break;
        }

        return defaultAssignmentExpression is not null;
    }

    private List<EnumMapCase> BuildCases(
        EnumMapMemberInfoAttribute[] relevantMemberAttributes,
        HashSet<string> ignoredSourceMemberNames,
        HashSet<string> ignoredTargetMemberNames)
        => this.legKind switch
        {
            EnumMapLegKind.EnumToEnum => this.BuildEnumToEnumCases(
                relevantMemberAttributes,
                ignoredSourceMemberNames,
                ignoredTargetMemberNames),
            EnumMapLegKind.EnumToString => this.BuildEnumToStringCases(relevantMemberAttributes, ignoredSourceMemberNames),
            EnumMapLegKind.EnumToIntegral => this.BuildEnumToIntegralCases(relevantMemberAttributes, ignoredSourceMemberNames),
            EnumMapLegKind.StringToEnum => this.BuildStringToEnumCases(relevantMemberAttributes, ignoredTargetMemberNames),
            _ => this.BuildIntegralToEnumCases(relevantMemberAttributes, ignoredTargetMemberNames),
        };

    private List<EnumMapCase> BuildEnumToEnumCases(
        EnumMapMemberInfoAttribute[] relevantMemberAttributes,
        HashSet<string> ignoredSourceMemberNames,
        HashSet<string> ignoredTargetMemberNames)
    {
        var settings = this.context.MappaUserSettings;
        var caseInsensitive = settings.CaseInsensitiveEnumMap is BooleanSetting.Enable;
        var sourceType = this.context.SourceType;
        var targetType = this.context.TargetType;
        IEnumerable<(string SourceMemberName, string TargetMemberName)> baseMappings =
            GetEffectiveEnumToEnumMapSetting(settings.EnumToEnumMapSetting) switch
            {
                EnumToEnumMapSetting.NumericValue => sourceType.GetSharedEnumMemberMappingsByValue(targetType),
                EnumToEnumMapSetting.Description => sourceType.GetSharedEnumMemberMappingsByDescription(
                    targetType,
                    this.compilation,
                    caseInsensitive),
                _ when caseInsensitive => sourceType.GetSharedEnumMemberMappingsByNameCaseInsensitive(targetType),
                _ => sourceType.GetSharedEnumMemberNamesByName(targetType).Select(enumName => (enumName, enumName)),
            };

        var pairs = new List<(string SourceMemberName, string TargetMemberName)>(baseMappings);
        foreach (var memberAttribute in relevantMemberAttributes)
        {
            if (!this.TryGetEnumToEnumPair(memberAttribute, out var sourceMemberName, out var targetMemberName))
            {
                continue;
            }

            var index = pairs.FindIndex(pair => pair.SourceMemberName.Equals(sourceMemberName, StringComparison.Ordinal));
            if (index >= 0)
            {
                pairs[index] = (sourceMemberName, targetMemberName);
            }
            else
            {
                pairs.Add((sourceMemberName, targetMemberName));
            }
        }

        var sourceEnumFullName = sourceType.ToDisplayString();
        var targetEnumFullName = targetType.ToDisplayString();
        return
        [
            .. pairs
                .Where(pair => !ignoredSourceMemberNames.Contains(pair.SourceMemberName)
                               && !ignoredTargetMemberNames.Contains(pair.TargetMemberName))
                .Select(pair => new EnumMapCase(
                    $"{sourceEnumFullName}.{pair.SourceMemberName}",
                    $"{targetEnumFullName}.{pair.TargetMemberName}",
                    pair.SourceMemberName)),
        ];
    }

    private List<EnumMapCase> BuildEnumToStringCases(
        EnumMapMemberInfoAttribute[] relevantMemberAttributes,
        HashSet<string> ignoredSourceMemberNames)
    {
        var sourceType = this.context.SourceType;
        var sourceEnumFullName = sourceType.ToDisplayString();
        var pairs = new List<(string MemberName, string ValueExpression)>();
        if (GetEffectiveEnumStringMapSetting(this.context.MappaUserSettings.EnumStringMapSetting) is EnumStringMapSetting.Description)
        {
            pairs.AddRange(sourceType.GetEnumMembersWithDescriptions(this.compilation)
                .Select(member => (member.Name, TypeSymbolExtensions.ToCSharpStringLiteral(member.Description))));
        }
        else
        {
            pairs.AddRange(sourceType.GetEnumValues()
                .Select(enumValue => (enumValue.Name, $"nameof({sourceEnumFullName}.{enumValue.Name})")));
        }

        ApplyOverrides(
            pairs,
            relevantMemberAttributes,
            memberAttribute => TypeSymbolExtensions.ToCSharpStringLiteral(memberAttribute.StringValue ?? string.Empty));

        return
        [
            .. pairs
                .Where(pair => !ignoredSourceMemberNames.Contains(pair.MemberName))
                .Select(pair => new EnumMapCase(
                    $"{sourceEnumFullName}.{pair.MemberName}",
                    pair.ValueExpression,
                    pair.MemberName)),
        ];
    }

    private List<EnumMapCase> BuildEnumToIntegralCases(
        EnumMapMemberInfoAttribute[] relevantMemberAttributes,
        HashSet<string> ignoredSourceMemberNames)
    {
        var sourceType = this.context.SourceType;
        var sourceEnumFullName = sourceType.ToDisplayString();
        var pairs = new List<(string MemberName, string ValueExpression)>(
            sourceType.GetEnumValues().Select(enumValue => (enumValue.Name, FormatIntegral(enumValue.Value))));

        ApplyOverrides(
            pairs,
            relevantMemberAttributes,
            memberAttribute => (memberAttribute.IntegerValue ?? 0).ToString(CultureInfo.InvariantCulture));

        return
        [
            .. pairs
                .Where(pair => !ignoredSourceMemberNames.Contains(pair.MemberName))
                .Select(pair => new EnumMapCase(
                    $"{sourceEnumFullName}.{pair.MemberName}",
                    pair.ValueExpression,
                    pair.MemberName)),
        ];
    }

    private List<EnumMapCase> BuildStringToEnumCases(
        EnumMapMemberInfoAttribute[] relevantMemberAttributes,
        HashSet<string> ignoredTargetMemberNames)
    {
        var targetType = this.context.TargetType;
        var targetEnumFullName = targetType.ToDisplayString();
        var caseInsensitive = this.context.MappaUserSettings.CaseInsensitiveEnumMap is BooleanSetting.Enable;
        var pairs = new List<(string MemberName, string ValueExpression)>();
        if (GetEffectiveEnumStringMapSetting(this.context.MappaUserSettings.EnumStringMapSetting) is EnumStringMapSetting.Description)
        {
            pairs.AddRange(targetType.GetEnumMembersWithDescriptions(this.compilation)
                .Select(member => (
                    member.Name,
                    TypeSymbolExtensions.ToCSharpStringLiteral(caseInsensitive ? member.Description.ToUpperInvariant() : member.Description))));
        }
        else
        {
            pairs.AddRange(targetType.GetEnumValues()
                .Select(enumValue => (
                    enumValue.Name,
                    caseInsensitive
                        ? $"\"{enumValue.Name.ToUpperInvariant()}\""
                        : $"nameof({targetEnumFullName}.{enumValue.Name})")));
        }

        ApplyOverrides(
            pairs,
            relevantMemberAttributes,
            memberAttribute => TypeSymbolExtensions.ToCSharpStringLiteral(
                caseInsensitive
                    ? (memberAttribute.StringValue ?? string.Empty).ToUpperInvariant()
                    : memberAttribute.StringValue ?? string.Empty));

        return
        [
            .. pairs
                .Where(pair => !ignoredTargetMemberNames.Contains(pair.MemberName))
                .Select(pair => new EnumMapCase(
                    pair.ValueExpression,
                    $"{targetEnumFullName}.{pair.MemberName}",
                    null)),
        ];
    }

    private List<EnumMapCase> BuildIntegralToEnumCases(
        EnumMapMemberInfoAttribute[] relevantMemberAttributes,
        HashSet<string> ignoredTargetMemberNames)
    {
        var targetType = this.context.TargetType;
        var targetEnumFullName = targetType.ToDisplayString();
        var pairs = new List<(string MemberName, string ValueExpression)>(
            targetType.GetEnumValues().Select(enumValue => (enumValue.Name, FormatIntegral(enumValue.Value))));

        ApplyOverrides(
            pairs,
            relevantMemberAttributes,
            memberAttribute => (memberAttribute.IntegerValue ?? 0).ToString(CultureInfo.InvariantCulture));

        return
        [
            .. pairs
                .Where(pair => !ignoredTargetMemberNames.Contains(pair.MemberName))
                .Select(pair => new EnumMapCase(
                    pair.ValueExpression,
                    $"{targetEnumFullName}.{pair.MemberName}",
                    null)),
        ];
    }

    private bool ValidateNoDuplicateCases(List<EnumMapCase> cases)
        => this.ValidateNoDuplicates(
            cases.Select(mapCase => mapCase.CaseExpression),
            duplicate => $"case label '{duplicate}' is generated more than once");

    private void ReportIgnoreConflict(INamedTypeSymbol? enumType, string enumMemberName)
        => this.context.ReportDiagnostic(MappaDiagnostics.EnumMapIgnoreConflictsWithMemberMapping(
            this.methodDeclarationSyntax,
            this.methodName,
            enumType?.ToDisplayString() ?? string.Empty,
            enumMemberName));

    private void ReportMemberMappingClash(string details)
        => this.context.ReportDiagnostic(MappaDiagnostics.EnumMapMemberMappingClash(
            this.methodDeclarationSyntax,
            this.methodName,
            (this.sourceEnumType ?? this.targetEnumType)?.ToDisplayString() ?? string.Empty,
            details));
}