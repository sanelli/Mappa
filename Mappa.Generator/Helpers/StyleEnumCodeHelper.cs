// <copyright file="StyleEnumCodeHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Attributes;
using Mappa.Generator.Exceptions;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Decomposes and validates <see cref="DateTimeStyles"/> and <see cref="NumberStyles"/> flag values.
/// </summary>
internal static class StyleEnumCodeHelper
{
    /// <summary>
    /// Returns <c>true</c> when <paramref name="styles"/> is unset, <c>None</c>, or fully decomposable into known flags.
    /// </summary>
    /// <param name="styles">The date time styles value.</param>
    /// <returns><c>true</c> when the value is valid.</returns>
    internal static bool IsValidDateTimeStyle(DateTimeStyles styles)
        => IsValid(styles, MappaSettingsAttribute.UndefinedDateTimeStyle);

    /// <summary>
    /// Returns <c>true</c> when <paramref name="styles"/> is unset, <c>None</c>, or fully decomposable into known flags.
    /// </summary>
    /// <param name="styles">The number styles value.</param>
    /// <returns><c>true</c> when the value is valid.</returns>
    internal static bool IsValidNumberStyle(NumberStyles styles)
        => IsValid(styles, MappaSettingsAttribute.UndefinedNumberStyle);

    /// <summary>
    /// Builds a C# expression for <paramref name="styles"/>.
    /// </summary>
    /// <param name="styles">The date time styles value.</param>
    /// <returns>The style expression.</returns>
    internal static string GetDateTimeStyleExpression(DateTimeStyles styles)
        => GetStyleExpression(styles, "System.Globalization.DateTimeStyles");

    /// <summary>
    /// Builds a C# expression for <paramref name="styles"/>.
    /// </summary>
    /// <param name="styles">The number styles value.</param>
    /// <returns>The style expression.</returns>
    internal static string GetNumberStyleExpression(NumberStyles styles)
        => GetStyleExpression(styles, "System.Globalization.NumberStyles");

    private static bool IsValid<T>(T styles, T undefinedSentinel)
        where T : struct, Enum
    {
        if (EqualityComparer<T>.Default.Equals(styles, undefinedSentinel))
        {
            return true;
        }

        return TryDecompose(styles).Remaining == 0;
    }

    private static (List<T> Parts, int Remaining) TryDecompose<T>(T styles)
        where T : struct, Enum
    {
        var noneValue = GetNoneValue<T>();
        var noneInt = Convert.ToInt32(noneValue, CultureInfo.InvariantCulture);
        var stylesInt = Convert.ToInt32(styles, CultureInfo.InvariantCulture);

        if (stylesInt == noneInt)
        {
            return ([], 0);
        }

        var flags = (T[])Enum.GetValues(typeof(T));
        Array.Sort(flags, (left, right) => Convert.ToInt32(right, CultureInfo.InvariantCulture).CompareTo(Convert.ToInt32(left, CultureInfo.InvariantCulture)));

        var parts = new List<T>();
        var remaining = stylesInt;

        foreach (T flag in flags)
        {
            var flagInt = Convert.ToInt32(flag, CultureInfo.InvariantCulture);
            if (flagInt == noneInt)
            {
                continue;
            }

            if ((remaining & flagInt) == flagInt)
            {
                parts.Add(flag);
                remaining &= ~flagInt;
            }
        }

        return (parts, remaining);
    }

    private static T GetNoneValue<T>()
        where T : struct, Enum
    {
        foreach (T value in Enum.GetValues(typeof(T)))
        {
            if (Convert.ToInt32(value, CultureInfo.InvariantCulture) == 0)
            {
                return value;
            }
        }

        throw new MappaGeneratorException($"Cannot obtain the zero-valued member for enum type '{typeof(T).FullName}'.");
    }

    private static string GetStyleExpression<T>(T styles, string enumTypeName)
        where T : struct, Enum
    {
        var noneValue = GetNoneValue<T>();
        if (EqualityComparer<T>.Default.Equals(styles, noneValue))
        {
            return $"{enumTypeName}.None";
        }

        var (parts, _) = TryDecompose(styles);

        if (parts.Count == 0)
        {
            return $"{enumTypeName}.None";
        }

        parts.Sort((left, right) => Convert.ToInt32(left, CultureInfo.InvariantCulture).CompareTo(Convert.ToInt32(right, CultureInfo.InvariantCulture)));

        if (parts.Count == 1)
        {
            return $"{enumTypeName}.{parts[0]}";
        }

        return string.Join(" | ", parts.ConvertAll(part => $"{enumTypeName}.{part}"));
    }
}