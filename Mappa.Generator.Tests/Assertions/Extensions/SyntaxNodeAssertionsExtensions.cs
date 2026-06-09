// <copyright file="SyntaxNodeAssertionsExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests.Assertions.Extensions;

/// <summary>
/// Extensions for <see cref="SyntaxNodeAssertions"/>.
/// </summary>
internal static class SyntaxNodeAssertionsExtensions
{
    /// <summary>
    /// Assert that the syntax node assigns a target member value to a context entry.
    /// </summary>
    /// <param name="this">The syntax node assertions.</param>
    /// <param name="contextParameterName">The context parameter name.</param>
    /// <param name="contextKey">The context key.</param>
    /// <param name="targetTemporaryName">The target temporary variable name.</param>
    /// <param name="memberName">The target member name.</param>
    /// <returns>The assertions instance.</returns>
    internal static SyntaxNodeAssertions BeAssignToContextStatement(
        this SyntaxNodeAssertions @this,
        string contextParameterName,
        string contextKey,
        string targetTemporaryName,
        string memberName)
    {
        ArgumentNullException.ThrowIfNull(@this);

        return @this.BeAssignmentExpressionStatement(
            leftExpressionAssertions => leftExpressionAssertions.BeElementAccessExpressionSyntaxWithLiteralSyntax(contextParameterName, contextKey),
            rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax($"{targetTemporaryName}.{memberName}"));
    }
}