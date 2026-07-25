// <copyright file="EnumMapConfiguration.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// Describes the resolved configuration of an enum mapping leg: the explicit switch arms,
/// the fallback behaviour and the fallback value expression.
/// </summary>
internal sealed class EnumMapConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumMapConfiguration"/> class.
    /// </summary>
    /// <param name="cases">The switch arms in emission order.</param>
    /// <param name="defaultBehavior">The fallback behaviour applied by the <c>default</c> arm.</param>
    /// <param name="defaultAssignmentExpression">The C# expression assigned by the <c>default</c> arm when the behaviour is <see cref="MappaMapEnumDefaultBehavior.UseDefaultValue"/>.</param>
    /// <param name="ignoredSourceEnumMemberNames">The source enum member names excluded from the mapping.</param>
    internal EnumMapConfiguration(
        IReadOnlyList<EnumMapCase> cases,
        MappaMapEnumDefaultBehavior defaultBehavior,
        string? defaultAssignmentExpression,
        IReadOnlyList<string> ignoredSourceEnumMemberNames)
    {
        this.Cases = cases;
        this.DefaultBehavior = defaultBehavior;
        this.DefaultAssignmentExpression = defaultAssignmentExpression;
        this.IgnoredSourceEnumMemberNames = ignoredSourceEnumMemberNames;
    }

    /// <summary>
    /// Gets the switch arms in emission order.
    /// </summary>
    internal IReadOnlyList<EnumMapCase> Cases { get; }

    /// <summary>
    /// Gets the fallback behaviour applied by the <c>default</c> arm.
    /// </summary>
    internal MappaMapEnumDefaultBehavior DefaultBehavior { get; }

    /// <summary>
    /// Gets the C# expression assigned by the <c>default</c> arm when
    /// <see cref="DefaultBehavior"/> is <see cref="MappaMapEnumDefaultBehavior.UseDefaultValue"/>.
    /// </summary>
    internal string? DefaultAssignmentExpression { get; }

    /// <summary>
    /// Gets the source enum member names excluded from the mapping.
    /// </summary>
    internal IReadOnlyList<string> IgnoredSourceEnumMemberNames { get; }

    /// <summary>
    /// Gets the source enum member names covered by an explicit switch arm.
    /// </summary>
    internal IEnumerable<string> MappedSourceEnumMemberNames
        => this.Cases
            .Select(mapCase => mapCase.SourceEnumMemberName)
            .OfType<string>();
}