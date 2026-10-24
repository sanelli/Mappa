// <copyright file="NullSubstituteMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Substitutes a null source value (constant or method) before applying the element mapping strategy.
/// </summary>
internal sealed class NullSubstituteMapStrategy
    : MapStrategy
{
    private NullSubstituteMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        MapStrategy elementStrategy,
        object? assignValue,
        IMethodSymbol? method,
        ISymbol? fieldOrProperty,
        Type? invokeMethodType,
        bool isNullableEnabled,
        string? contextParameterName)
        : base(targetType, sourceType)
    {
        this.ElementStrategy = elementStrategy;
        this.AssignValue = assignValue;
        this.Method = method;
        this.FieldOrProperty = fieldOrProperty;
        this.InvokeMethodType = invokeMethodType;
        this.IsNullableEnabled = isNullableEnabled;
        this.ContextParameterName = contextParameterName;
    }

    /// <summary>
    /// Gets a value indicating whether this strategy uses AssignValue mode.
    /// </summary>
    internal bool IsAssignValueMode => this.Method is null;

    /// <summary>
    /// Gets the constant substitute value when in AssignValue mode.
    /// </summary>
    internal object? AssignValue { get; }

    /// <summary>
    /// Gets the method invoked when the source is null (InvokeMethod mode).
    /// </summary>
    internal IMethodSymbol? Method { get; }

    /// <summary>
    /// Gets the mapper field or property used as the invoke target, if any.
    /// </summary>
    internal ISymbol? FieldOrProperty { get; }

    /// <summary>
    /// Gets the type used to locate a static invoke method, if any.
    /// </summary>
    internal Type? InvokeMethodType { get; }

    /// <summary>
    /// Gets a value indicating whether nullable reference types are enabled.
    /// </summary>
    internal bool IsNullableEnabled { get; }

    /// <summary>
    /// Gets the root map method <c>MappaContext</c> parameter name when the invoke method requires it.
    /// </summary>
    internal string? ContextParameterName { get; }

    /// <summary>
    /// Gets the strategy applied after null substitution.
    /// </summary>
    internal MapStrategy ElementStrategy { get; }

    /// <summary>
    /// Creates an AssignValue null-substitute strategy.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source member type.</param>
    /// <param name="assignValue">The constant substitute value.</param>
    /// <param name="elementStrategy">The strategy applied after substitution.</param>
    /// <returns>The strategy.</returns>
    internal static NullSubstituteMapStrategy ForAssignValue(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        object? assignValue,
        MapStrategy elementStrategy)
        => new(
            targetType,
            sourceType,
            elementStrategy,
            assignValue,
            method: null,
            fieldOrProperty: null,
            invokeMethodType: null,
            isNullableEnabled: false,
            contextParameterName: null);

    /// <summary>
    /// Creates an InvokeMethod null-substitute strategy.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source member type.</param>
    /// <param name="method">The method invoked when the source is null.</param>
    /// <param name="fieldOrProperty">The optional mapper field or property invoke target.</param>
    /// <param name="invokeMethodType">The optional type used to locate a static method.</param>
    /// <param name="isNullableEnabled"><c>true</c> when nullable is enabled.</param>
    /// <param name="contextParameterName">The context parameter name when required.</param>
    /// <param name="elementStrategy">The strategy applied after substitution.</param>
    /// <returns>The strategy.</returns>
    internal static NullSubstituteMapStrategy ForInvokeMethod(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        IMethodSymbol method,
        ISymbol? fieldOrProperty,
        Type? invokeMethodType,
        bool isNullableEnabled,
        string? contextParameterName,
        MapStrategy elementStrategy)
        => new(
            targetType,
            sourceType,
            elementStrategy,
            assignValue: null,
            method,
            fieldOrProperty,
            invokeMethodType,
            isNullableEnabled,
            contextParameterName);

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new NullSubstituteMapStrategyBuilder(this);
}