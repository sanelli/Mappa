// <copyright file="StringMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

// TODO [#56] Add missing String -> DateTimeOffset.
// TODO [#56] Use MappaUserSettings for TimeSpan.
// TODO [#56] Use MappaUserSettings for DateTime.
// TODO [#56] Use MappaUserSettings for DateOnly.
// TODO [#56] Use MappaUserSettings for TimeOnly.

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
    public bool TryDetect(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.TargetType);

        // 01. string -> numeric : ParseNumberStrategy
        if (this.CanMapStringToNumber())
        {
            mapStrategy = new StringToNumberMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 02. string -> DateTime : ParseDateTimeStrategy
        else if (this.CanMapStringToDateTime())
        {
            mapStrategy = new StringToDateTimeMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 03. string -> TimeSpan : ParseTimeStampStrategy
        else if (this.CanMapStringToTimeSpan())
        {
            mapStrategy = new StringToTimeSpanMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 04. string -> TimeOnly : ParseTimeOnlyStrategy
        else if (this.CanMapStringToTimeOnly())
        {
            mapStrategy = new StringToTimeOnlyMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 05. string -> DateOnly : ParseDateOnlyStrategy
        else if (this.CanMapStringToDateOnly())
        {
            mapStrategy = new StringToDateOnlyMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 06. string -> Guid : ParseGuidStrategy
        else if (this.CanMapStringToGuid())
        {
            mapStrategy = new StringToGuidMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                this.context.MappaUserSettings.GuidFormat);
        }

        // 07. string -> Uri : ParseUriStrategy
        else if (this.CanMapStringToUri())
        {
            mapStrategy = new StringToUriMapStrategy(
                this.context.TargetType,
                this.context.SourceType);
        }

        // 08. S -> string : InvokeToStringStrategy
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
    }

    private (string? Format, CultureInfoSetting CultureInfoSetting, string? CultureName) IdentifyFormatAndCulture()
    {
        var settings = this.context.MappaUserSettings;

        string? format = null;
        bool acceptFormatProviderOnly = true; // Some types do not have a ToString(string, IFormatProvider).
        if (this.context.SourceType.IsGuid(this.compilation))
        {
            acceptFormatProviderOnly = false;
            format = settings.GuidFormat;
        }
        else if (this.context.SourceType.IsDateTime())
        {
            format = settings.DateTimeFormat;
        }
        else if (this.context.SourceType.IsDateTimeOffset(this.compilation))
        {
            format = settings.DateTimeOffsetFormat;
        }
        else if (this.context.SourceType.IsDateOnly(this.compilation))
        {
            format = settings.DateOnlyFormat;
        }
        else if (this.context.SourceType.IsTimeOnly(this.compilation))
        {
            format = settings.TimeOnlyFormat;
        }
        else if (this.context.SourceType.IsTimeSpan(this.compilation))
        {
            acceptFormatProviderOnly = false;
            format = settings.TimeSpanFormat;
        }

        CultureInfoSetting cultureInfoSettings = CultureInfoSetting.None;
        string? cultureName = null;
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

        return (format, cultureInfoSettings, cultureName);
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
        var isTimeOnly = this.context.TargetType.IsTimeOnly(this.compilation);
        var isSourceString = this.context.SourceType.IsString();
        return isTimeOnly && isSourceString;
    }

    private bool CanMapStringToDateOnly()
    {
        var isDateOnly = this.context.TargetType.IsDateOnly(this.compilation);
        var isSourceString = this.context.SourceType.IsString();
        return isDateOnly && isSourceString;
    }

    private bool CanMapStringToUri()
    {
        var isUri = this.context.TargetType.IsUri(this.compilation);
        var isSourceString = this.context.SourceType.IsString();
        return isUri && isSourceString;
    }

    private bool CanMapStringToGuid()
    {
        var isGuid = this.context.TargetType.IsGuid(this.compilation);
        var isSourceString = this.context.SourceType.IsString();
        return isGuid && isSourceString;
    }
}