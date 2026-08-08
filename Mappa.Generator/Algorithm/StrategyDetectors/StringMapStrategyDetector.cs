// <copyright file="StringMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for string related strategies.
/// </summary>
internal sealed partial class StringMapStrategyDetector
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

    private bool TryDetectStringToNumber(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!this.CanMapStringToNumber())
        {
            return false;
        }

        var numberStyle = this.GetNumericNumberStyleWithSource(this.context.TargetType, out var numberStylePropertyName);
        this.WarnIfInvalidNumberStyle(numberStyle, numberStylePropertyName);
        mapStrategy = new StringToNumberMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            this.GetActualCultureSettingsInfo(),
            this.context.MappaUserSettings.CultureName,
            numberStyle);
        return true;
    }

    private bool TryDetectStringToDateTime(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!this.CanMapStringToDateTime())
        {
            return false;
        }

        var actualCultureSettingsInfo = this.GetActualCultureSettingsInfo();
        this.WarnIfOnlyFormatIsProvided(actualCultureSettingsInfo, this.context.MappaUserSettings.DateTimeFormat);
        var dateTimeStyle = this.ResolveDateTimeStyleWithSource(
            this.context.MappaUserSettings.DateTimeStyle,
            nameof(MappaSettingsAttribute.DateTimeStyle),
            out var dateTimeStylePropertyName);
        this.WarnIfInvalidDateTimeStyle(dateTimeStyle, dateTimeStylePropertyName);
        mapStrategy = new InvokeParseStringWithFormatMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            this.context.MappaUserSettings.DateTimeFormat,
            actualCultureSettingsInfo,
            this.context.MappaUserSettings.CultureName,
            dateTimeStyle);
        return true;
    }

    private bool TryDetectStringToDateTimeOffset(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!this.CanMapStringToDateTimeOffset())
        {
            return false;
        }

        var actualCultureSettingsInfo = this.GetActualCultureSettingsInfo();
        this.WarnIfOnlyFormatIsProvided(actualCultureSettingsInfo, this.context.MappaUserSettings.DateTimeOffsetFormat);
        var dateTimeStyle = this.ResolveDateTimeStyleWithSource(
            this.context.MappaUserSettings.DateTimeOffsetStyle,
            nameof(MappaSettingsAttribute.DateTimeOffsetStyle),
            out var dateTimeStylePropertyName);
        this.WarnIfInvalidDateTimeStyle(dateTimeStyle, dateTimeStylePropertyName);
        mapStrategy = new InvokeParseStringWithFormatMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            this.context.MappaUserSettings.DateTimeOffsetFormat,
            actualCultureSettingsInfo,
            this.context.MappaUserSettings.CultureName,
            dateTimeStyle);
        return true;
    }

    private bool TryDetectStringToTimeSpan(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!this.CanMapStringToTimeSpan())
        {
            return false;
        }

        var actualCultureSettingsInfo = this.GetActualCultureSettingsInfo();
        this.WarnIfOnlyFormatIsProvided(actualCultureSettingsInfo, this.context.MappaUserSettings.TimeSpanFormat);
        mapStrategy = new InvokeParseStringWithFormatMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            this.context.MappaUserSettings.TimeSpanFormat,
            actualCultureSettingsInfo,
            this.context.MappaUserSettings.CultureName,
            null);
        return true;
    }

    private bool TryDetectStringToTimeOnly(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!this.CanMapStringToTimeOnly())
        {
            return false;
        }

        var dateTimeStyle = this.ResolveDateTimeStyleWithSource(
            this.context.MappaUserSettings.TimeOnlyStyle,
            nameof(MappaSettingsAttribute.TimeOnlyStyle),
            out var dateTimeStylePropertyName);
        this.WarnIfInvalidDateTimeStyle(dateTimeStyle, dateTimeStylePropertyName);
        mapStrategy = new InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            this.context.MappaUserSettings.TimeOnlyFormat,
            this.GetActualCultureSettingsInfo(),
            this.context.MappaUserSettings.CultureName,
            dateTimeStyle);
        return true;
    }

    private bool TryDetectStringToDateOnly(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!this.CanMapStringToDateOnly())
        {
            return false;
        }

        var dateTimeStyle = this.ResolveDateTimeStyleWithSource(
            this.context.MappaUserSettings.DateOnlyStyle,
            nameof(MappaSettingsAttribute.DateOnlyStyle),
            out var dateTimeStylePropertyName);
        this.WarnIfInvalidDateTimeStyle(dateTimeStyle, dateTimeStylePropertyName);
        mapStrategy = new InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            this.context.MappaUserSettings.DateOnlyFormat,
            this.GetActualCultureSettingsInfo(),
            this.context.MappaUserSettings.CultureName,
            dateTimeStyle);
        return true;
    }

    private bool TryDetectStringToGuid(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!this.CanMapStringToGuid())
        {
            return false;
        }

        mapStrategy = new StringToGuidMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            this.context.MappaUserSettings.GuidFormat,
            this.GetActualCultureSettingsInfo(),
            this.context.MappaUserSettings.CultureName);
        return true;
    }

    private bool TryDetectStringToUri(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!this.CanMapStringToUri())
        {
            return false;
        }

        mapStrategy = new StringToUriMapStrategy(this.context.TargetType, this.context.SourceType);
        return true;
    }

    private bool TryDetectUsingStaticParseMethod(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!this.CanMapUsingStaticParseMethod())
        {
            return false;
        }

        mapStrategy = new InvokeParseMethodMapStrategy(this.context.TargetType, this.context.SourceType);
        return true;
    }

    private bool TryDetectToString(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (!this.CanMapToString())
        {
            return false;
        }

        var formatAndCulture = this.IdentifyFormatAndCulture();
        mapStrategy = new InvokeToStringMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            formatAndCulture.Format,
            formatAndCulture.CultureInfoSetting,
            formatAndCulture.CultureName);
        return true;
    }

    private void WarnIfOnlyFormatIsProvided(CultureInfoSetting actualCultureSettingsInfo, string? format)
    {
        if (!string.IsNullOrWhiteSpace(format)
            && (actualCultureSettingsInfo is CultureInfoSetting.None || actualCultureSettingsInfo is CultureInfoSetting.Undefined))
        {
            var rootMethod = this.context.GetRootMapMethod();
            this.context.ReportDiagnostic(MappaDiagnostics.ParseExactDoesNotAcceptOnlyFormat(
                rootMethod.MethodDeclarationSyntax,
                this.context.TargetType.ToDisplayString()));
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
        var isTargetNumeric = this.context.TargetType.IsNumeric();
        var isSourceString = this.context.SourceType.IsString();
        return isTargetNumeric && isSourceString;
    }

    private NumberStyles? GetNumericNumberStyleWithSource(ITypeSymbol typeSymbol, out string? propertyName)
    {
        if (this.TryGetPerTypeNumericNumberStyle(typeSymbol.SpecialType, out var typeStyle, out var typeStylePropertyName))
        {
            propertyName = typeStylePropertyName;
            return typeStyle;
        }

        propertyName = this.context.MappaUserSettings.GlobalNumberStyle.HasValue
            ? nameof(MappaSettingsAttribute.GlobalNumberStyle)
            : null;
        return this.context.MappaUserSettings.GlobalNumberStyle;
    }

    private bool TryGetPerTypeNumericNumberStyle(
        SpecialType specialType,
        out NumberStyles? numberStyle,
        out string? propertyName)
    {
        var settings = this.context.MappaUserSettings;
        numberStyle = null;
        propertyName = null;

        switch (specialType)
        {
            case SpecialType.System_Byte:
                numberStyle = settings.ByteStyle;
                propertyName = nameof(MappaSettingsAttribute.ByteStyle);
                break;
            case SpecialType.System_SByte:
                numberStyle = settings.SByteStyle;
                propertyName = nameof(MappaSettingsAttribute.SByteStyle);
                break;
            case SpecialType.System_Int16:
                numberStyle = settings.ShortStyle;
                propertyName = nameof(MappaSettingsAttribute.ShortStyle);
                break;
            case SpecialType.System_UInt16:
                numberStyle = settings.UShortStyle;
                propertyName = nameof(MappaSettingsAttribute.UShortStyle);
                break;
            case SpecialType.System_Int32:
                numberStyle = settings.IntStyle;
                propertyName = nameof(MappaSettingsAttribute.IntStyle);
                break;
            case SpecialType.System_UInt32:
                numberStyle = settings.UIntStyle;
                propertyName = nameof(MappaSettingsAttribute.UIntStyle);
                break;
            case SpecialType.System_Int64:
                numberStyle = settings.LongStyle;
                propertyName = nameof(MappaSettingsAttribute.LongStyle);
                break;
            case SpecialType.System_UInt64:
                numberStyle = settings.ULongStyle;
                propertyName = nameof(MappaSettingsAttribute.ULongStyle);
                break;
            case SpecialType.System_Decimal:
                numberStyle = settings.DecimalStyle;
                propertyName = nameof(MappaSettingsAttribute.DecimalStyle);
                break;
            case SpecialType.System_Single:
                numberStyle = settings.FloatStyle;
                propertyName = nameof(MappaSettingsAttribute.FloatStyle);
                break;
            case SpecialType.System_Double:
                numberStyle = settings.DoubleStyle;
                propertyName = nameof(MappaSettingsAttribute.DoubleStyle);
                break;
            default:
                return false;
        }

        return numberStyle.HasValue;
    }

    private DateTimeStyles? ResolveDateTimeStyleWithSource(
        DateTimeStyles? typeStyle,
        string typeStylePropertyName,
        out string? propertyName)
    {
        if (typeStyle.HasValue)
        {
            propertyName = typeStylePropertyName;
            return typeStyle;
        }

        propertyName = this.context.MappaUserSettings.GlobalDateTimeStyle.HasValue
            ? nameof(MappaSettingsAttribute.GlobalDateTimeStyle)
            : null;
        return this.context.MappaUserSettings.GlobalDateTimeStyle;
    }

    private void WarnIfInvalidDateTimeStyle(DateTimeStyles? style, string? propertyName)
    {
        if (!style.HasValue || propertyName is not { Length: > 0 } validatedPropertyName)
        {
            return;
        }

        if (StyleEnumCodeHelper.IsValidDateTimeStyle(style.Value))
        {
            return;
        }

        var rootMethod = this.context.GetRootMapMethod();
        this.context.ReportDiagnostic(MappaDiagnostics.InvalidMappaSettingsStyleValue(
            rootMethod.MethodDeclarationSyntax,
            validatedPropertyName,
            (int)style.Value,
            nameof(DateTimeStyles)));
    }

    private void WarnIfInvalidNumberStyle(NumberStyles? style, string? propertyName)
    {
        if (!style.HasValue || propertyName is not { Length: > 0 } validatedPropertyName)
        {
            return;
        }

        if (StyleEnumCodeHelper.IsValidNumberStyle(style.Value))
        {
            return;
        }

        var rootMethod = this.context.GetRootMapMethod();
        this.context.ReportDiagnostic(MappaDiagnostics.InvalidMappaSettingsStyleValue(
            rootMethod.MethodDeclarationSyntax,
            validatedPropertyName,
            (int)style.Value,
            nameof(NumberStyles)));
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
                .Where(method => this.context.MapMethod is not null && this.compilation.IsSymbolAccessibleWithin(method, this.context.MapMethod.ContainingType))
                .FirstOrDefault(method => method.Parameters.Length == 1 && method.Parameters[0].Type.IsString());
            targetHasParseMethod = parseMethod is not null;
        }

        return isSourceString && targetHasParseMethod;
    }
}