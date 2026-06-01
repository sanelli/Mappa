// <copyright file="AotJson.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;
using System.Globalization;
using System.Text;

namespace Mappa.Samples.Aot;

/// <summary>
/// Display-string formatting and method signature helpers for the AOT report.
/// </summary>
internal static class AotJson
{
    /// <summary>
    /// Builds a method signature string including the return type.
    /// </summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="parameterType">The parameter type name.</param>
    /// <param name="returnType">The return type name.</param>
    /// <returns>The formatted method signature.</returns>
    public static string FormatMethod(string methodName, string parameterType, string returnType)
        => $"{methodName}({parameterType}) -> {returnType}";

    /// <summary>
    /// Converts a value to a human-readable display string for JSON report fields.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The display string.</returns>
    public static string ToDisplayString(object? value)
    {
        if (value is null)
        {
            return "<null>";
        }

        return value switch
        {
            string s => FormatQuotedString(s),
            char c => FormatQuotedChar(c),
            bool b => b ? "true" : "false",
            Enum e => $"{e.GetType().Name}.{e}",
            byte or sbyte or short or ushort or int or uint or long or ulong
                or float or double or decimal => Convert.ToString(value, CultureInfo.InvariantCulture)!,
            DateTime dt => $"DateTime '{dt:O}'",
            DateTimeOffset dto => $"DateTimeOffset '{dto:O}'",
            DateOnly d => $"DateOnly '{d:O}'",
            TimeOnly t => $"TimeOnly '{t:O}'",
            TimeSpan ts => $"TimeSpan '{ts:c}'",
            Guid g => $"Guid '{g}'",
            Uri u => $"Uri '{u}'",
            IDictionary dictionary => FormatDictionary(dictionary),
            IEnumerable enumerable when value is not string => FormatEnumerable(enumerable),
            _ => $"{value.GetType().Name} '{value}'",
        };
    }

    /// <summary>
    /// Appends a JSON-encoded string literal to the builder.
    /// </summary>
    /// <param name="builder">The string builder.</param>
    /// <param name="value">The string value.</param>
    public static void AppendJsonString(StringBuilder builder, string value)
    {
        builder.Append('"');
        foreach (var character in value)
        {
            switch (character)
            {
                case '"':
                    builder.Append("\\\"");
                    break;
                case '\\':
                    builder.Append("\\\\");
                    break;
                case '\n':
                    builder.Append("\\n");
                    break;
                case '\r':
                    builder.Append("\\r");
                    break;
                case '\t':
                    builder.Append("\\t");
                    break;
                default:
                    builder.Append(character);
                    break;
            }
        }

        builder.Append('"');
    }

    private static string FormatQuotedString(string s)
    {
        var escaped = s.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("'", "\\'", StringComparison.Ordinal);
        return $"'{escaped}'";
    }

    private static string FormatQuotedChar(char c)
    {
        if (c == '\'')
        {
            return "'\\''";
        }

        if (c == '\\')
        {
            return "'\\\\'";
        }

        return $"'{c}'";
    }

    private static string FormatDictionary(IDictionary dictionary)
    {
        var items = new List<string>();
        foreach (DictionaryEntry entry in dictionary)
        {
            items.Add($"{{ {ToDisplayString(entry.Key)}: {ToDisplayString(entry.Value)} }}");
        }

        return $"[ {string.Join(", ", items)} ]";
    }

    private static string FormatEnumerable(IEnumerable enumerable)
    {
        var items = new List<string>();
        foreach (var item in enumerable)
        {
            if (item is DictionaryEntry entry)
            {
                items.Add($"{{ {ToDisplayString(entry.Key)}: {ToDisplayString(entry.Value)} }}");
            }
            else
            {
                items.Add(ToDisplayString(item));
            }
        }

        return $"[ {string.Join(", ", items)} ]";
    }
}