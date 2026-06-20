// <copyright file="StringMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for string related strategies.
/// </summary>
internal sealed class StringMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation settings.</param>
    public StringMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation)
    {
        this.context = context;
        this.compilation = compilation;
    }

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.TargetType);

        // 01. string -> numeric : ParseNumberStrategy
        if (this.CanMapStringToNumber())
        {
            mapStrategy = new StringToNumberMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                this.GetActualCultureSettingsInfo(),
                this.context.MappaUserSettings.CultureName);
        }

        // 02. string -> DateTime : InvokeParseStringWithFormatMapStrategy
        else if (this.CanMapStringToDateTime())
        {
            var actualCultureSettingsInfo = this.GetActualCultureSettingsInfo();
            WarnIfOnlyFormatIsProvided(actualCultureSettingsInfo, this.context.MappaUserSettings.DateTimeFormat);

            mapStrategy = new InvokeParseStringWithFormatMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                this.context.MappaUserSettings.DateTimeFormat,
                actualCultureSettingsInfo,
                this.context.MappaUserSettings.CultureName,
                this.context.MappaUserSettings.DateTimeStyle);
        }

        // 03. string -> DateTimeOffset : InvokeParseStringWithFormatMapStrategy
        else if (this.CanMapStringToDateTimeOffset())
        {
            var actualCultureSettingsInfo = this.GetActualCultureSettingsInfo();
            WarnIfOnlyFormatIsProvided(actualCultureSettingsInfo, this.context.MappaUserSettings.DateTimeOffsetFormat);

            mapStrategy = new InvokeParseStringWithFormatMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                this.context.MappaUserSettings.DateTimeOffsetFormat,
                actualCultureSettingsInfo,
                this.context.MappaUserSettings.CultureName,
                this.context.MappaUserSettings.DateTimeOffsetStyle);
        }

        // 04. string -> TimeSpan : InvokeParseStringWithFormatMapStrategy
        else if (this.CanMapStringToTimeSpan())
        {
            var actualCultureSettingsInfo = this.GetActualCultureSettingsInfo();
            WarnIfOnlyFormatIsProvided(actualCultureSettingsInfo, this.context.MappaUserSettings.TimeSpanFormat);

            mapStrategy = new InvokeParseStringWithFormatMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                this.context.MappaUserSettings.TimeSpanFormat,
                actualCultureSettingsInfo,
                this.context.MappaUserSettings.CultureName,
                null);
        }

        // 05. string -> TimeOnly : InvokeParseStringWithFormatMapStrategy
        else if (this.CanMapStringToTimeOnly())
        {
            mapStrategy = new InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                this.context.MappaUserSettings.TimeOnlyFormat,
                this.GetActualCultureSettingsInfo(),
                this.context.MappaUserSettings.CultureName,
                this.context.MappaUserSettings.TimeOnlyStyle);
        }

        // 06. string -> DateOnly : InvokeParseStringWithFormatMapStrategy
        else if (this.CanMapStringToDateOnly())
        {
            mapStrategy = new InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                this.context.MappaUserSettings.DateOnlyFormat,
                this.GetActualCultureSettingsInfo(),
                this.context.MappaUserSettings.CultureName,
                this.context.MappaUserSettings.DateOnlyStyle);
        }

        // 07. string -> Guid : ParseGuidStrategy
        else if (this.CanMapStringToGuid())
        {
            mapStrategy = new StringToGuidMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                this.context.MappaUserSettings.GuidFormat,
                this.GetActualCultureSettingsInfo(),
                this.context.MappaUserSettings.CultureName);
        }

        // 08. string -> Uri : ParseUriStrategy
        else if (this.CanMapStringToUri())
        {
            mapStrategy = new StringToUriMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 09. string -> T when static T T.Parse(string) method exists
        else if (this.CanMapUsingStaticParseMethod())
        {
           mapStrategy = new InvokeParseMethodMapStrategy(
               this.context.TargetType,
               this.context.SourceType);
        }

        // 10. S -> string : InvokeToStringStrategy
        else if (this.CanMapToString())
        {
            var formatAndCulture = this.IdentifyFormatAndCulture();
            mapStrategy = new InvokeToStringMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                formatAndCulture.Format,
                formatAndCulture.CultureInfoSetting,
                formatAndCulture.CultureName);
        }

        return mapStrategy is not NoMapStrategy;

        void WarnIfOnlyFormatIsProvided(CultureInfoSetting actualCultureSettingsInfo, string? format)
        {
            // If format is provided but not the culture then we have a problem
            // because some types (DateTime, DateTimeOffset, TimeSpan) does not support
            // ParseExact(string value, string format) so format will be ignored.
            if (!string.IsNullOrWhiteSpace(format)
                && (actualCultureSettingsInfo is CultureInfoSetting.None || actualCultureSettingsInfo is CultureInfoSetting.Undefined))
            {
                var rootMethod = this.context.GetRootMapMethod();
                this.context.ReportDiagnostic(MappaDiagnostics.ParseExactDoesNotAcceptOnlyFormat(
                    rootMethod.MethodDeclarationSyntax,
                    this.context.TargetType.ToDisplayString()));
            }
        }
    }

    private CultureInfoSetting GetActualCultureSettingsInfo()
    {
        var cultureSettingsInfo = this.context.MappaUserSettings.CultureInfoSetting;
        if (cultureSettingsInfo is CultureInfoSetting.UserDefined
            && string.IsNullOrWhiteSpace(this.context.MappaUserSettings.CultureName))
        {
            cultureSettingsInfo = CultureInfoSetting.None;
            this.context.ReportDiagnostic(MappaDiagnostics.UserDefinedCultureIsMissingCultureName(
                this.context.GetRootMapMethod().MethodDeclarationSyntax ?? throw new MappaGeneratorException("Method declaration syntax is missing")));
        }

        return cultureSettingsInfo;
    }

    private (string? Format, CultureInfoSetting CultureInfoSetting, string? CultureName) IdentifyFormatAndCulture()
    {
        var settings = this.context.MappaUserSettings;

        string? format = null;
        CultureInfoSetting cultureInfoSettings = CultureInfoSetting.None;
        string? cultureName = null;

        if (this.context.SourceType.IsGuid(this.compilation))
        {
            format = settings.GuidFormat;
            UpdateCultureSettingsAndName(false);
        }
        else if (this.context.SourceType.IsDateTime())
        {
            format = settings.DateTimeFormat;
            UpdateCultureSettingsAndName();
        }
        else if (this.context.SourceType.IsDateTimeOffset(this.compilation))
        {
            format = settings.DateTimeOffsetFormat;
            UpdateCultureSettingsAndName();
        }
        else if (this.context.SourceType.IsDateOnly(this.compilation))
        {
            format = settings.DateOnlyFormat;
            UpdateCultureSettingsAndName();
        }
        else if (this.context.SourceType.IsTimeOnly(this.compilation))
        {
            format = settings.TimeOnlyFormat;
            UpdateCultureSettingsAndName();
        }
        else if (this.context.SourceType.IsTimeSpan(this.compilation))
        {
            format = settings.TimeSpanFormat;
            UpdateCultureSettingsAndName(false);
        }
        else if (this.context.SourceType.IsNumeric())
        {
            format = GetNumericFormat(this.context.SourceType);
            UpdateCultureSettingsAndName();
        }

        return (format, cultureInfoSettings, cultureName);

        string? GetNumericFormat(ITypeSymbol typeSymbol)
            => typeSymbol.SpecialType switch
            {
                SpecialType.System_Byte => settings.ByteFormat,
                SpecialType.System_SByte => settings.SByteFormat,
                SpecialType.System_Int16 => settings.ShortFormat,
                SpecialType.System_UInt16 => settings.UShortFormat,
                SpecialType.System_Int32 => settings.IntFormat,
                SpecialType.System_UInt32 => settings.UIntFormat,
                SpecialType.System_Int64 => settings.LongFormat,
                SpecialType.System_UInt64 => settings.ULongFormat,
                SpecialType.System_Decimal => settings.DecimalFormat,
                SpecialType.System_Single => settings.FloatFormat,
                SpecialType.System_Double => settings.DoubleFormat,
                _ => null,
            };

        void UpdateCultureSettingsAndName(bool acceptFormatProviderOnly = true)
        {
            if (acceptFormatProviderOnly || !string.IsNullOrWhiteSpace(format))
            {
                if (settings.CultureInfoSetting is CultureInfoSetting.UserDefined
                    && string.IsNullOrWhiteSpace(settings.CultureName))
                {
                    this.context.ReportDiagnostic(MappaDiagnostics.UserDefinedCultureIsMissingCultureName(
                        this.context.GetRootMapMethod().MethodDeclarationSyntax!));
                }
                else
                {
                    cultureInfoSettings = settings.CultureInfoSetting;
                    cultureName = settings.CultureName;
                }
            }
        }
    }

    private bool CanMapStringToNumber()
    {
        var isTargetDateTime = this.context.TargetType.IsNumeric();
        var isSourceString = this.context.SourceType.IsString();
        return isTargetDateTime && isSourceString;
    }

    private bool CanMapStringToDateTime()
    {
        var isTargetDateTime = this.context.TargetType.IsDateTime();
        var isSourceString = this.context.SourceType.IsString();
        return isTargetDateTime && isSourceString;
    }

    private bool CanMapStringToDateTimeOffset()
    {
        var isTargetDateTimeOffset = this.context.TargetType.IsDateTimeOffset(this.compilation);
        var isSourceString = this.context.SourceType.IsString();
        return isTargetDateTimeOffset && isSourceString;
    }

    private bool CanMapToString()
    {
        var isTargetString = this.context.TargetType.IsString();
        return isTargetString;
    }

    private bool CanMapStringToTimeSpan()
    {
        var isTargetDateTime = this.context.TargetType.IsTimeSpan(this.compilation);
        var isSourceString = this.context.SourceType.IsString();
        return isTargetDateTime && isSourceString;
    }

    private bool CanMapStringToTimeOnly()
    {
        var isTargetTimeOnly = this.context.TargetType.IsTimeOnly(this.compilation);
        var isSourceString = this.context.SourceType.IsString();
        return isTargetTimeOnly && isSourceString;
    }

    private bool CanMapStringToDateOnly()
    {
        var isTargetDateOnly = this.context.TargetType.IsDateOnly(this.compilation);
        var isSourceString = this.context.SourceType.IsString();
        return isTargetDateOnly && isSourceString;
    }

    private bool CanMapStringToUri()
    {
        var isTargetUri = this.context.TargetType.IsUri(this.compilation);
        var isSourceString = this.context.SourceType.IsString();
        return isTargetUri && isSourceString;
    }

    private bool CanMapStringToGuid()
    {
        var isTargetGuid = this.context.TargetType.IsGuid(this.compilation);
        var isSourceString = this.context.SourceType.IsString();
        return isTargetGuid && isSourceString;
    }

    private bool CanMapUsingStaticParseMethod()
    {
        var isSourceString = this.context.SourceType.IsString();
        var targetHasParseMethod = false;
        if (this.context.TargetType is INamedTypeSymbol namedTypeSymbol)
        {
            var parseMethod = namedTypeSymbol
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(method => nameof(Guid.Parse).Equals(method.Name, StringComparison.Ordinal))
                .Where(method => method.IsStatic)
                .Where(method => this.context.MapMethod is not null && this.compilation.IsSymbolAccessibleWithin(method, this.context.MapMethod.MethodSymbol.ContainingSymbol))
                .FirstOrDefault(method => method.Parameters.Length == 1 && method.Parameters[0].Type.IsString());
            targetHasParseMethod = parseMethod is not null;
        }

        return isSourceString && targetHasParseMethod;
    }
}