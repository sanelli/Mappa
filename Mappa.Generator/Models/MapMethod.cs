// <copyright file="MapMethod.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describe a method that can be used for mapping.
/// </summary>
internal sealed class MapMethod
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapMethod"/> class.
    /// </summary>
    /// <param name="fieldName">
    /// The name of the field that can be used to access the method. This can be
    /// <c>"this"</c> for local methods, <c>""</c> for static methods and
    /// the name of the variable that can be used to access the class containing
    /// the method.
    /// </param>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="targetType">The target type of the method.</param>
    /// <param name="sourceType">The type of source parameter.</param>
    /// <param name="sourceParameterName">The name of the source parameter.</param>
    public MapMethod(
        string fieldName,
        string methodName,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        string sourceParameterName)
    {
        this.FieldName = fieldName;
        this.MethodName = methodName;
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.SourceParameterName = sourceParameterName;
        this.Mapped = false;
    }

    /// <summary>
    /// Gets the field name to access method.
    /// </summary>
    internal string FieldName { get; }

    /// <summary>
    /// Gets the method name.
    /// </summary>
    internal string MethodName { get; }

    /// <summary>
    /// Gets the target type.
    /// </summary>
    internal ITypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the source type.
    /// </summary>
    internal ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the source parameter name.
    /// </summary>
    internal string SourceParameterName { get; }

    /// <summary>
    /// Gets a value indicating whether the.
    /// </summary>
    internal bool Mapped { get; private set; }

    /// <summary>
    /// Mark the method as being mapped.
    /// </summary>
    internal void MarkMapped() => this.Mapped = true;

    /// <summary>
    /// Check if the method is map from <paramref name="sourceType"/>
    /// to <paramref name="targetType"/>.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <returns><c>true</c> if the method is a map from
    /// <paramref name="sourceType"/> to <paramref name="targetType"/>.</returns>
    internal bool IsMapFor(ITypeSymbol targetType, ITypeSymbol sourceType)
        => SymbolEqualityComparer.Default.Equals(targetType, this.TargetType)
            && SymbolEqualityComparer.Default.Equals(sourceType, this.SourceType);
}