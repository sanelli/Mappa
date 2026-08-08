// <copyright file="ParseNumberStylesCodeHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Generator.Exceptions;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Builds parse invocation code for numeric types with optional culture and <see cref="NumberStyles"/>.
/// </summary>
internal static class ParseNumberStylesCodeHelper
{
    /// <summary>
    /// Parses a <see cref="NumberStyles"/> value from an editorconfig string.
    /// </summary>
    /// <param name="value">The editorconfig value.</param>
    /// <returns>The parsed value, or <c>null</c> when unset or invalid.</returns>
    internal static NumberStyles? TryParseFromString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        NumberStyles result = NumberStyles.None;
        foreach (var token in value!.Split(['|', ','], StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedToken = token.Trim();
            if (trimmedToken.Length == 0)
            {
                continue;
            }

            if (!Enum.TryParse(trimmedToken, ignoreCase: true, out NumberStyles flag))
            {
                return null;
            }

            result |= flag;
        }

        return result;
    }

    /// <summary>
    /// Builds the parameter list for a numeric <c>Parse</c> invocation.
    /// </summary>
    /// <param name="source">The source variable name.</param>
    /// <param name="cultureInfoSetting">The culture info settings.</param>
    /// <param name="cultureName">The culture name when user-defined culture is selected.</param>
    /// <param name="numberStyle">The optional number style.</param>
    /// <returns>The parse parameter list.</returns>
    internal static string BuildParseInvocation(
        string source,
        CultureInfoSetting cultureInfoSetting,
        string? cultureName,
        NumberStyles? numberStyle)
    {
        var hasCulture = HasCulture(cultureInfoSetting);
        var hasStyle = numberStyle.HasValue;

        if (!hasStyle)
        {
            if (hasCulture)
            {
                return $"{source}, {GetCultureParameter(cultureInfoSetting, cultureName)}";
            }

            return source;
        }

        var styleExpression = StyleEnumCodeHelper.GetNumberStyleExpression(numberStyle!.Value);

        if (hasCulture)
        {
            return $"{source}, {styleExpression}, {GetCultureParameter(cultureInfoSetting, cultureName)}";
        }

        return $"{source}, {styleExpression}";
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

                throw new MappaGeneratorException("Unexpected scenario when building GetCultureInfo without culture name");
        }

        throw new MappaGeneratorException($"Unexpected culture info setting '{cultureInfoSetting}'.");
    }
}