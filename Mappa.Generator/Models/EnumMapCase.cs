// <copyright file="EnumMapCase.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a single <c>case</c> arm of a generated enum mapping switch.
/// </summary>
internal sealed class EnumMapCase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumMapCase"/> class.
    /// </summary>
    /// <param name="caseExpression">The C# expression used as case label.</param>
    /// <param name="assignmentExpression">The C# expression assigned to the mapping temporary.</param>
    /// <param name="sourceEnumMemberName">The source enum member name the arm originates from, when the mapping source is an enum.</param>
    internal EnumMapCase(
        string caseExpression,
        string assignmentExpression,
        string? sourceEnumMemberName)
    {
        this.CaseExpression = caseExpression;
        this.AssignmentExpression = assignmentExpression;
        this.SourceEnumMemberName = sourceEnumMemberName;
    }

    /// <summary>
    /// Gets the C# expression used as case label.
    /// </summary>
    internal string CaseExpression { get; }

    /// <summary>
    /// Gets the C# expression assigned to the mapping temporary.
    /// </summary>
    internal string AssignmentExpression { get; }

    /// <summary>
    /// Gets the source enum member name the arm originates from,
    /// or <c>null</c> when the mapping source is not an enum.
    /// </summary>
    internal string? SourceEnumMemberName { get; }
}