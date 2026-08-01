// <copyright file="MappaMustMapTargetPropertyAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Attribute that can be used to require that specific non-required target properties
/// are mapped when using empty-constructor (or empty-constructor-like) property mapping.
/// <para>
/// When applied with no property names, every non-required mappable target property must be mapped.
/// When applied with one or more property names, only those listed non-required properties must be mapped;
/// other unmapped non-required properties continue to produce the usual skip warning.
/// </para>
/// <para>
/// Listing a property that is already <c>required</c>, or a name that does not exist on the target type,
/// produces a warning and does not stop mapping. Listing a property that is also ignored via
/// <see cref="MappaIgnoreTargetPropertyAttribute"/> is an error.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class MappaMustMapTargetPropertyAttribute
    : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMustMapTargetPropertyAttribute"/> class
    /// requiring that all non-required target properties are mapped.
    /// </summary>
    public MappaMustMapTargetPropertyAttribute()
    {
        this.TargetPropertyNames = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaMustMapTargetPropertyAttribute"/> class
    /// requiring that the listed non-required target properties are mapped.
    /// </summary>
    /// <param name="targetPropertyNames">
    /// The names of the target properties that must be mapped.
    /// An empty or <c>null</c> list has the same meaning as the parameterless constructor.
    /// </param>
    public MappaMustMapTargetPropertyAttribute(params string[]? targetPropertyNames)
    {
        this.TargetPropertyNames = targetPropertyNames ?? [];
    }

    /// <summary>
    /// Gets the names of the target properties that must be mapped.
    /// An empty array means all non-required target properties must be mapped.
    /// </summary>
    public string[] TargetPropertyNames { get; }
}