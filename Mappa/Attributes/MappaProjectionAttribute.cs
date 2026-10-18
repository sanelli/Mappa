// <copyright file="MappaProjectionAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Controls whether map methods with an <c>IQueryable&lt;TSource&gt;</c> →
/// <c>IQueryable&lt;TTarget&gt;</c> signature are handled as deferred queryable
/// projections by the Mappa source generator.
/// <para>
/// Projection remains signature-driven when this attribute is absent. Apply the
/// attribute to a mapper class or to an individual map method to opt in or out:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <see cref="BooleanSetting.Enable"/> (default when the attribute is present): treat
/// eligible <c>IQueryable</c> map methods as projections.
/// </description></item>
/// <item><description>
/// <see cref="BooleanSetting.Disable"/>: do not use the projection strategy for the
/// annotated method (or for projection-signature methods on the annotated class);
/// the generator falls through to the normal container / eager mapping path.
/// </description></item>
/// <item><description>
/// <see cref="BooleanSetting.Undefined"/>: ignore this attribute application and use
/// the value from a parent scope (method overrides class) or the signature-driven default.
/// </description></item>
/// </list>
/// <para>
/// Useful on mappers that declare both in-memory map methods and
/// <c>IQueryable</c> projection methods. Does not replace <see cref="MappaIgnoreAttribute"/>,
/// which excludes a method from generation entirely.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class MappaProjectionAttribute
    : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether queryable projection is enabled for the
    /// annotated mapper class or map method. Defaults to <see cref="BooleanSetting.Enable"/>.
    /// </summary>
    public BooleanSetting Enabled { get; set; } = BooleanSetting.Enable;
}