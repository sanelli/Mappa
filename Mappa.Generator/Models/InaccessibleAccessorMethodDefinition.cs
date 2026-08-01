// <copyright file="InaccessibleAccessorMethodDefinition.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a generated <c>UnsafeAccessor</c> extern method.
/// </summary>
internal sealed class InaccessibleAccessorMethodDefinition
{
    private InaccessibleAccessorMethodDefinition(
        string methodName,
        InaccessibleAccessorUnsafeKind unsafeKind,
        string runtimeName,
        ITypeSymbol containingType,
        string returnTypeDisplay,
        IReadOnlyList<(ITypeSymbol Type, string Name)> parameters)
    {
        this.MethodName = methodName;
        this.UnsafeKind = unsafeKind;
        this.RuntimeName = runtimeName;
        this.ContainingType = containingType;
        this.ReturnTypeDisplay = returnTypeDisplay;
        this.Parameters = parameters;
    }

    /// <summary>
    /// Gets the generated C# method identifier (a <c>__mappa_tmp_*</c> temporary).
    /// </summary>
    internal string MethodName { get; }

    /// <summary>
    /// Gets the unsafe accessor kind to emit.
    /// </summary>
    internal InaccessibleAccessorUnsafeKind UnsafeKind { get; }

    /// <summary>
    /// Gets the runtime member name passed to the <c>Name</c> attribute argument
    /// (for example <c>get_Property</c>, <c>set_Property</c>, or <c>.ctor</c>).
    /// </summary>
    internal string RuntimeName { get; }

    /// <summary>
    /// Gets the type that declares the inaccessible member.
    /// </summary>
    internal ITypeSymbol ContainingType { get; }

    /// <summary>
    /// Gets the return type display string of the accessor.
    /// </summary>
    internal string ReturnTypeDisplay { get; }

    /// <summary>
    /// Gets the accessor parameters in declaration order.
    /// </summary>
    internal IReadOnlyList<(ITypeSymbol Type, string Name)> Parameters { get; }

    /// <summary>
    /// Creates a getter accessor definition.
    /// </summary>
    /// <param name="methodName">The generated method name.</param>
    /// <param name="containingType">The declaring type.</param>
    /// <param name="property">The property.</param>
    /// <returns>The accessor definition.</returns>
    internal static InaccessibleAccessorMethodDefinition ForPropertyGetter(
        string methodName,
        ITypeSymbol containingType,
        IPropertySymbol property)
        => new(
            methodName,
            InaccessibleAccessorUnsafeKind.Method,
            $"get_{property.Name}",
            containingType,
            property.Type.ToDisplayString(),
            [(containingType, "instance")]);

    /// <summary>
    /// Creates a setter accessor definition.
    /// </summary>
    /// <param name="methodName">The generated method name.</param>
    /// <param name="containingType">The declaring type.</param>
    /// <param name="property">The property.</param>
    /// <returns>The accessor definition.</returns>
    internal static InaccessibleAccessorMethodDefinition ForPropertySetter(
        string methodName,
        ITypeSymbol containingType,
        IPropertySymbol property)
        => new(
            methodName,
            InaccessibleAccessorUnsafeKind.Method,
            $"set_{property.Name}",
            containingType,
            "void",
            [(containingType, "instance"), (property.Type, "value")]);

    /// <summary>
    /// Creates a constructor accessor definition.
    /// </summary>
    /// <param name="methodName">The generated method name.</param>
    /// <param name="constructor">The constructor.</param>
    /// <returns>The accessor definition.</returns>
    internal static InaccessibleAccessorMethodDefinition ForConstructor(
        string methodName,
        IMethodSymbol constructor)
    {
        var parameters = constructor.Parameters
            .Select(parameter => (parameter.Type, parameter.Name))
            .ToArray();
        return new(
            methodName,
            InaccessibleAccessorUnsafeKind.Constructor,
            ".ctor",
            constructor.ContainingType,
            constructor.ContainingType.ToDisplayString(),
            parameters);
    }

    /// <summary>
    /// Builds the extern method source code.
    /// </summary>
    /// <returns>The method source.</returns>
    internal string BuildSource()
    {
        var builder = new PrettyCode.StringBuilder();
        var unsafeKindLiteral = this.UnsafeKind switch
        {
            InaccessibleAccessorUnsafeKind.Constructor => "Constructor",
            _ => "Method",
        };

        builder.AppendLine(
            $"[global::System.Runtime.CompilerServices.UnsafeAccessor(global::System.Runtime.CompilerServices.UnsafeAccessorKind.{unsafeKindLiteral}, Name = \"{this.RuntimeName}\")]");

        var parameterList = string.Join(
            ", ",
            this.Parameters.Select(parameter => $"{parameter.Type.ToDisplayString()} {parameter.Name}"));
        builder.AppendLine(
            $"extern static {this.ReturnTypeDisplay} {this.MethodName}({parameterList});");

        return builder.ToString();
    }
}