// <copyright file="MappaAssignFromConstantAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Attribute that can be used to assign a constant to a target property or constructor parameter.
/// <para>
/// <see cref="TargetPropertyName"/> may be a single property name or a dot-separated chain of nested property names
/// (for example <c>"Address.City"</c>).
/// </para>
/// <para>
/// For a path with multiple segments, the attribute applies when mapping the <strong>first</strong> segment; the remaining segments identify the nested member that receives the constant value.
/// For example, <c>"Foo.Bar"</c> with value <c>42</c> applies when mapping target property <c>Foo</c> and assigns <c>42</c> to nested property <c>Bar</c>.
/// </para>
/// </summary>
/// <param name="targetPropertyName">The name of the target property or constructor parameter, or a dot-separated chain of nested target property names.</param>
/// <param name="value">The constant value to assign.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaAssignFromConstantAttribute(string targetPropertyName, object? value)
        : Attribute, IMappaTargetPropertyNameAttribute
{
    /// <summary>
    /// Gets the target property name, which may be a single name or a dot-separated chain of nested property names.
    /// </summary>
    public string TargetPropertyName { get; } = targetPropertyName;

    /// <summary>
    /// Gets the constant value to assign.
    /// </summary>
    public object? Value { get; } = value;
}