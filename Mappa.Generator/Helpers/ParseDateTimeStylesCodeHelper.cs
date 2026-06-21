// <copyright file="ParseDateTimeStylesCodeHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Generator.Exceptions;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Builds parse invocation code for date/time types with optional format, culture, and <see cref="DateTimeStyles"/>.
/// </summary>
internal static class ParseDateTimeStylesCodeHelper
{
    /// <summary>
    /// Parses a <see cref="DateTimeStyles"/> value from an editorconfig string.
    /// </summary>
    /// <param name="value">The editorconfig value.</param>
    /// <returns>The parsed value, or <c>null</c> when unset or invalid.</returns>
    internal static DateTimeStyles? TryParseFromString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        DateTimeStyles result = DateTimeStyles.None;
        foreach (var token in value!.Split(['|', ','], StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedToken = token.Trim();
            if (trimmedToken.Length == 0)
            {
                continue;
            }

            if (!Enum.TryParse(trimmedToken, ignoreCase: true, out DateTimeStyles flag))
            {
                return null;
            }

            result |= flag;
        }

        return result;
    }

    /// <summary>
    /// Builds parse method name and parameters for types that require culture when using <see cref="DateTimeStyles"/>.
    /// </summary>
    /// <param name="source">The source variable name.</param>
    /// <param name="format">The optional format.</param>
    /// <param name="cultureInfoSetting">The culture info settings.</param>
    /// <param name="cultureName">The culture name when user-defined culture is selected.</param>
    /// <param name="dateTimeStyle">The optional date time style.</param>
    /// <returns>The parse method and parameter list.</returns>
    internal static (string ParseMethod, string Parameters) BuildParseInvocation(
        string source,
        string? format,
        CultureInfoSetting cultureInfoSetting,
        string? cultureName,
        DateTimeStyles? dateTimeStyle)
    {
        var hasFormat = !string.IsNullOrWhiteSpace(format);
        var hasCulture = HasCulture(cultureInfoSetting);
        var hasStyle = dateTimeStyle.HasValue;

        if (!hasStyle)
        {
            if (hasCulture)
            {
                var cultureParameter = GetCultureParameter(cultureInfoSetting, cultureName);
                if (hasFormat)
                {
                    return ("ParseExact", $"{source}, {CSharpLiteralHelper.ToRequiredStringLiteral(format)}, {cultureParameter}");
                }

                return ("Parse", $"{source}, {cultureParameter}");
            }

            if (hasFormat)
            {
                return ("ParseExact", $"{source}, {CSharpLiteralHelper.ToRequiredStringLiteral(format)}");
            }

            return ("Parse", source);
        }

        var cultureExpression = hasCulture ? GetCultureParameter(cultureInfoSetting, cultureName) : "null";
        var styleExpression = GetStyleExpression(dateTimeStyle!.Value);

        if (hasFormat)
        {
            return ("ParseExact", $"{source}, {CSharpLiteralHelper.ToRequiredStringLiteral(format)}, {cultureExpression}, {styleExpression}");
        }

        return ("Parse", $"{source}, {cultureExpression}, {styleExpression}");
    }

    /// <summary>
    /// Builds parse method name and parameters for <see cref="DateTime"/> and <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="source">The source variable name.</param>
    /// <param name="format">The optional format.</param>
    /// <param name="cultureInfoSetting">The culture info settings.</param>
    /// <param name="cultureName">The culture name when user-defined culture is selected.</param>
    /// <param name="dateTimeStyle">The optional date time style.</param>
    /// <returns>The parse method and parameter list.</returns>
    internal static (string ParseMethod, string Parameters) BuildDateTimeOrDateTimeOffsetParseInvocation(
        string source,
        string? format,
        CultureInfoSetting cultureInfoSetting,
        string? cultureName,
        DateTimeStyles? dateTimeStyle)
    {
        var hasFormat = !string.IsNullOrWhiteSpace(format);
        var hasCulture = HasCulture(cultureInfoSetting);
        var hasStyle = dateTimeStyle.HasValue;

        if (!hasStyle)
        {
            if (hasCulture)
            {
                var cultureParameter = GetCultureParameter(cultureInfoSetting, cultureName);
                if (hasFormat)
                {
                    return ("ParseExact", $"{source}, {CSharpLiteralHelper.ToRequiredStringLiteral(format)}, {cultureParameter}");
                }

                return ("Parse", $"{source}, {cultureParameter}");
            }

            return ("Parse", source);
        }

        var cultureExpression = hasCulture ? GetCultureParameter(cultureInfoSetting, cultureName) : "null";
        var styleExpression = GetStyleExpression(dateTimeStyle!.Value);

        if (hasFormat)
        {
            return ("ParseExact", $"{source}, {CSharpLiteralHelper.ToRequiredStringLiteral(format)}, {cultureExpression}, {styleExpression}");
        }

        return ("Parse", $"{source}, {cultureExpression}, {styleExpression}");
    }

    private static bool HasCulture(CultureInfoSetting cultureInfoSetting)
        => cultureInfoSetting is not CultureInfoSetting.Undefined and not CultureInfoSetting.None;

    private static string GetCultureParameter(CultureInfoSetting cultureInfoSetting, string? cultureName)
    {
        switch (cultureInfoSetting)
        {
            case CultureInfoSetting.CurrentCulture:
                return "System.Globalization.CultureInfo.CurrentCulture";
            case CultureInfoSetting.InvariantCulture:
                return "System.Globalization.CultureInfo.InvariantCulture";
            case CultureInfoSetting.UserDefined:
                if (!string.IsNullOrWhiteSpace(cultureName))
                {
                    return $"System.Globalization.CultureInfo.GetCultureInfo({CSharpLiteralHelper.ToRequiredStringLiteral(cultureName)})";
                }

                throw new MappaGeneratorException("Unexpected scenario when building GeyCultureInfo without culture name");
        }

        throw new MappaGeneratorException($"Unexpected culture info setting '{cultureInfoSetting}'.");
    }

    private static string GetStyleExpression(DateTimeStyles styles)
    {
        if (styles == DateTimeStyles.None)
        {
            return "System.Globalization.DateTimeStyles.None";
        }

        var flags = (DateTimeStyles[])Enum.GetValues(typeof(DateTimeStyles));
        Array.Sort(flags, (left, right) => ((int)right).CompareTo((int)left));

        var parts = new List<DateTimeStyles>();
        var remaining = styles;

        foreach (DateTimeStyles flag in flags)
        {
            if (flag == DateTimeStyles.None)
            {
                continue;
            }

            if ((remaining & flag) == flag)
            {
                parts.Add(flag);
                remaining &= ~flag;
            }
        }

        if (parts.Count == 0)
        {
            return "System.Globalization.DateTimeStyles.None";
        }

        parts.Sort((left, right) => ((int)left).CompareTo((int)right));

        if (parts.Count == 1)
        {
            return $"System.Globalization.DateTimeStyles.{parts[0]}";
        }

        return string.Join(" | ", parts.ConvertAll(static part => $"System.Globalization.DateTimeStyles.{part}"));
    }
}