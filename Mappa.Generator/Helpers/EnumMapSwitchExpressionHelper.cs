// <copyright file="EnumMapSwitchExpressionHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Builds switch expressions for enum mapping in queryable projections.
/// </summary>
internal static class EnumMapSwitchExpressionHelper
{
    /// <summary>
    /// Builds a switch expression for the specified enum mapping configuration.
    /// </summary>
    /// <param name="enumMapConfiguration">The resolved enum mapping configuration.</param>
    /// <param name="source">The source expression.</param>
    /// <returns>The switch expression.</returns>
    internal static string BuildSwitchExpression(
        EnumMapConfiguration enumMapConfiguration,
        string source)
        => BuildSwitchExpression(enumMapConfiguration, source, null);

    /// <summary>
    /// Builds a switch expression for the specified enum mapping configuration.
    /// </summary>
    /// <param name="enumMapConfiguration">The resolved enum mapping configuration.</param>
    /// <param name="source">The source expression.</param>
    /// <param name="sourceCastType">An optional cast applied to <paramref name="source"/> before switching.</param>
    /// <returns>The switch expression.</returns>
    internal static string BuildSwitchExpression(
        EnumMapConfiguration enumMapConfiguration,
        string source,
        string? sourceCastType)
    {
        var switchSource = string.IsNullOrWhiteSpace(sourceCastType)
            ? source
            : $"({sourceCastType}){source}";

        var arms = enumMapConfiguration.Cases
            .Select(enumMapCase => $"{enumMapCase.CaseExpression} => {enumMapCase.AssignmentExpression}")
            .ToList();

        var defaultArm = enumMapConfiguration.DefaultBehavior is MappaMapEnumDefaultBehavior.UseDefaultValue
                         && enumMapConfiguration.DefaultAssignmentExpression is { } defaultAssignmentExpression
            ? $"_ => {defaultAssignmentExpression}"
            : $"_ => throw new System.ArgumentOutOfRangeException({CSharpLiteralHelper.ToStringLiteral(source)})";

        arms.Add(defaultArm);
        return $"{switchSource} switch {{ {string.Join(", ", arms)} }}";
    }
}