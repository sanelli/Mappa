// <copyright file="ProjectionElementMethodDefinition.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.CodeDom.Compiler;
using System.Diagnostics;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a generated private element map method for queryable projections.
/// </summary>
internal sealed class ProjectionElementMethodDefinition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionElementMethodDefinition"/> class.
    /// </summary>
    /// <param name="methodName">The method name.</param>
    /// <param name="sourceType">The source element type.</param>
    /// <param name="targetType">The target element type.</param>
    /// <param name="sourceParameterName">The source parameter name.</param>
    /// <param name="expression">The return expression.</param>
    /// <param name="isStatic">Whether the containing mapper is static.</param>
    /// <param name="nullableEnabled">Whether nullable reference types are enabled.</param>
    internal ProjectionElementMethodDefinition(
        string methodName,
        ITypeSymbol sourceType,
        ITypeSymbol targetType,
        string sourceParameterName,
        string expression,
        bool isStatic,
        bool nullableEnabled)
    {
        this.MethodName = methodName;
        this.SourceType = sourceType;
        this.TargetType = targetType;
        this.SourceParameterName = sourceParameterName;
        this.Expression = expression;
        this.IsStatic = isStatic;
        this.NullableEnabled = nullableEnabled;
    }

    /// <summary>
    /// Gets the method name.
    /// </summary>
    internal string MethodName { get; }

    /// <summary>
    /// Gets the source element type.
    /// </summary>
    internal ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the target element type.
    /// </summary>
    internal ITypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the source parameter name.
    /// </summary>
    internal string SourceParameterName { get; }

    /// <summary>
    /// Gets the return expression.
    /// </summary>
    internal string Expression { get; }

    /// <summary>
    /// Gets a value indicating whether the containing mapper is static.
    /// </summary>
    internal bool IsStatic { get; }

    /// <summary>
    /// Gets a value indicating whether nullable reference types are enabled.
    /// </summary>
    internal bool NullableEnabled { get; }

    /// <summary>
    /// Builds the generated method source code.
    /// </summary>
    /// <returns>The method source code.</returns>
    internal string BuildSource()
    {
        var builder = new PrettyCode.StringBuilder();
        var staticModifier = this.IsStatic ? "static " : string.Empty;

        using (builder.NullableDirective(this.NullableEnabled))
        {
            builder
                .AppendLine($"[global::{typeof(DebuggerNonUserCodeAttribute).FullName}]")
                .AppendLine($"[global::{typeof(GeneratedCodeAttribute).FullName}(\"Mappa\", \"{typeof(MappaGenerator).Assembly.GetName().Version}\")]")
                .AppendLine($"private {staticModifier}{this.TargetType.ToDisplayString()} {this.MethodName}({this.SourceType.ToDisplayString()} {this.SourceParameterName})");
            using (builder.CurlyBracesBlock())
            {
                builder.AppendLine($"return {this.Expression};");
            }
        }

        return builder.ToString();
    }
}