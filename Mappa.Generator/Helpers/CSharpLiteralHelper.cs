// <copyright file="CSharpLiteralHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Emits properly escaped C# literal expressions for generated source code.
/// </summary>
internal static class CSharpLiteralHelper
{
    /// <summary>
    /// Returns a fully escaped C# string literal including surrounding quotes.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The escaped string literal.</returns>
    internal static string ToStringLiteral(string value)
        => ToStringLiteralCore(value);

    /// <summary>
    /// Returns a fully escaped C# string literal when <paramref name="value"/> is not null or whitespace.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The escaped string literal.</returns>
    /// <exception cref="MappaGeneratorException">When <paramref name="value"/> is null or whitespace.</exception>
    internal static string ToRequiredStringLiteral(string? value)
    {
        if (value is null)
        {
            throw new MappaGeneratorException("Cannot emit a string literal for a null value.");
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new MappaGeneratorException("Cannot emit a string literal for a whitespace value.");
        }

        return ToStringLiteral(value);
    }

    private static string ToStringLiteralCore(string value)
        => SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(value))
            .ToFullString();
}