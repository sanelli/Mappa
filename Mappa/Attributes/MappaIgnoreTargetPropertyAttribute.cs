// <copyright file="MappaIgnoreTargetPropertyAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Attribute that can be used to exclude a target property from empty-constructor property mapping.
/// <para>
/// <see cref="TargetPropertyName"/> may be a single property name or a dot-separated chain of nested property names
/// (for example <c>"Address.City"</c>).
/// </para>
/// <para>
/// For a path with multiple segments, the attribute applies when mapping the <strong>first</strong> segment; the remaining segments identify the nested property to ignore.
/// For example, <c>"Foo.Bar"</c> applies when mapping target property <c>Foo</c> and excludes nested property <c>Bar</c> from mapping inside <c>Foo</c>'s type.
/// </para>
/// </summary>
/// <param name="targetPropertyName">The name of the target property to ignore, or a dot-separated chain of nested target property names.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaIgnoreTargetPropertyAttribute(string targetPropertyName)
        : Attribute
{
    /// <summary>
    /// Gets the target property name, which may be a single name or a dot-separated chain of nested property names.
    /// </summary>
    public string TargetPropertyName { get; } = targetPropertyName;
}