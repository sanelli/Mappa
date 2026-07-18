// <copyright file="MappaUsePropertyAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Attribute that can be used to define the name of the source property for a specific target property or constructor parameter.
/// <para>
/// Both <see cref="TargetPropertyName"/> and <see cref="SourcePropertyName"/> may be a single property name or a dot-separated chain of nested property names
/// (for example <c>"Address.City"</c> or <c>"Location.Address.City"</c>).
/// </para>
/// <para>
/// For a target path with multiple segments, the attribute applies when mapping the <strong>first</strong> segment; the remaining segments are used while mapping the nested property type.
/// For example, <c>"Foo.Bar"</c> applies when mapping target property <c>Foo</c>; nested mapping inside <c>Foo</c> uses the remaining segment <c>Bar</c>.
/// </para>
/// <para>
/// The source path may contain the same number of segments as the target path or more, but not fewer.
/// When the remaining target path has a single segment, the generator reads the full remaining source path from the current source receiver using conditional member access where appropriate.
/// </para>
/// </summary>
/// <param name="targetPropertyName">The name of the target property or constructor parameter, or a dot-separated chain of nested target property names.</param>
/// <param name="sourcePropertyName">The name of the source property, or a dot-separated chain of nested source property names.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaUsePropertyAttribute(string targetPropertyName, string sourcePropertyName)
        : Attribute
{
    /// <summary>
    /// Gets the target property name, which may be a single name or a dot-separated chain of nested property names.
    /// </summary>
    public string TargetPropertyName { get; } = targetPropertyName;

    /// <summary>
    /// Gets the source property name, which may be a single name or a dot-separated chain of nested property names.
    /// </summary>
    public string SourcePropertyName { get; } = sourcePropertyName;
}