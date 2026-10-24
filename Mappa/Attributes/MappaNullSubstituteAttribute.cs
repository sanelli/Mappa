// <copyright file="MappaNullSubstituteAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Substitutes a null source member value before mapping or conversion to the target member.
/// <para>
/// Both <see cref="TargetPropertyName"/> and <see cref="SourcePropertyName"/> may be a single property name
/// or a dot-separated chain of nested property names (for example <c>"Address.City"</c>).
/// </para>
/// <para>
/// Exactly one of <see cref="AssignValue"/> or <see cref="InvokeMethod"/> must be set:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <see cref="AssignValue"/>: when the source value is null, use the constant (typed as the source member type <c>T</c>)
/// instead, then continue with normal mapping (<c>source ?? AssignValue</c>).
/// </description></item>
/// <item><description>
/// <see cref="InvokeMethod"/>: when the source value is null, invoke a method that accepts <c>T?</c>
/// (nullable enabled) or <c>T</c> (nullable disabled) and returns non-null <c>T</c>; when the source is non-null,
/// use it as-is without calling the method.
/// </description></item>
/// </list>
/// <para>
/// Locate the invoke method as follows:
/// </para>
/// <list type="bullet">
/// <item><description>Neither <see cref="InvokeMethodType"/> nor <see cref="InvokeMethodSource"/>: method on the mapper class or an accessible base class.</description></item>
/// <item><description><see cref="InvokeMethodType"/>: a <c>static</c> method on that type or a base class.</description></item>
/// <item><description><see cref="InvokeMethodSource"/>: an instance method on the type of the named mapper field or property (the member must be <c>static</c> when the root map method is <c>static</c>).</description></item>
/// </list>
/// <para>
/// Distinct from <see cref="MappaAssignFromConstantAttribute"/>, which assigns a constant to the target and skips mapping.
/// </para>
/// </summary>
/// <param name="targetPropertyName">The name of the target property or constructor parameter, or a dot-separated chain of nested target property names.</param>
/// <param name="sourcePropertyName">The name of the source property, or a dot-separated chain of nested source property names, whose null value is substituted.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaNullSubstituteAttribute(string targetPropertyName, string sourcePropertyName)
        : Attribute, IMappaTargetPropertyNameAttribute
{
    /// <summary>
    /// Gets the target property name, which may be a single name or a dot-separated chain of nested property names.
    /// </summary>
    public string TargetPropertyName { get; } = targetPropertyName;

    /// <summary>
    /// Gets the source property name, which may be a single name or a dot-separated chain of nested property names.
    /// </summary>
    public string SourcePropertyName { get; } = sourcePropertyName;

    /// <summary>
    /// Gets or sets the constant substitute value of the source member type.
    /// Mutually exclusive with <see cref="InvokeMethod"/>. Presence is detected from the named attribute argument.
    /// </summary>
    public object? AssignValue { get; set; }

    /// <summary>
    /// Gets or sets the name of the method invoked when the source value is null.
    /// The method accepts <c>T?</c> or <c>T</c> and returns non-null <c>T</c>.
    /// Mutually exclusive with <see cref="AssignValue"/>.
    /// </summary>
    public string? InvokeMethod { get; set; }

    /// <summary>
    /// Gets or sets the type defining the <c>static</c> method named by <see cref="InvokeMethod"/>,
    /// including methods declared on its base classes. Mutually exclusive with <see cref="InvokeMethodSource"/>.
    /// </summary>
    public Type? InvokeMethodType { get; set; }

    /// <summary>
    /// Gets or sets the name of the field or property on the mapper class or an accessible base class
    /// whose type exposes the instance method named by <see cref="InvokeMethod"/>.
    /// Must be <c>static</c> when the root map method is <c>static</c>.
    /// Mutually exclusive with <see cref="InvokeMethodType"/>.
    /// </summary>
    public string? InvokeMethodSource { get; set; }
}