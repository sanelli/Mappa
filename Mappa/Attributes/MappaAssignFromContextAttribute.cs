// <copyright file="MappaAssignFromContextAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Instruct the Mappa source generator to set a target property or constructor parameter value from the input context.
/// <para>
/// <see cref="TargetPropertyName"/> may be a single property name or a dot-separated chain of nested property names
/// (for example <c>"Address.City"</c>).
/// </para>
/// <para>
/// For a path with multiple segments, the attribute applies when mapping the <strong>first</strong> segment; the remaining segments identify the nested member that receives the context value.
/// For example, <c>"Foo.Bar"</c> applies when mapping target property <c>Foo</c> and assigns the context item to nested property <c>Bar</c>.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaAssignFromContextAttribute
    : Attribute, IMappaTargetPropertyNameAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAssignFromContextAttribute"/> class.
    /// </summary>
    /// <param name="targetPropertyName">The target property name involved in the mapping, or a dot-separated chain of nested target property names.</param>
    /// <param name="itemName">The name of the context item to use.</param>
    public MappaAssignFromContextAttribute(string targetPropertyName, string itemName)
    {
        this.TargetPropertyName = targetPropertyName;
        this.ItemName = itemName;
    }

    /// <inheritdoc/>
    public string TargetPropertyName { get; }

    /// <summary>
    /// Gets the name of the context item to be assigned to the member identified by <see cref="TargetPropertyName"/>.
    /// </summary>
    public string ItemName { get; }
}