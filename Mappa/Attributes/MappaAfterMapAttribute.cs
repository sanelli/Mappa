// <copyright file="MappaAfterMapAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Instructs the Mappa source generator to invoke <see cref="MethodName"/> immediately after
/// executing the generated mapping body and before returning the mapped target.
/// <para>
/// The hook can be applied to a mapper class or to an individual mapping method and can be located
/// in one of the following ways:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <c>(methodName)</c>: a <c>static</c> or instance method declared on the mapper class or an accessible
/// base class. A <c>static</c> mapping method can only invoke a <c>static</c> hook.
/// </description></item>
/// <item><description>
/// <c>(classType, methodName)</c>: a <c>static</c> method declared on <see cref="ClassType"/> or an
/// accessible base class.
/// </description></item>
/// <item><description>
/// <c>(fieldName, methodName)</c>: a method declared on the type of the field or property identified by
/// <see cref="FieldName"/>. Instance hooks are invoked through the field or property. Static hooks are
/// invoked through its declared type without evaluating the field or property. An instance hook used by
/// a <c>static</c> mapping method requires a <c>static</c> field or property.
/// </description></item>
/// </list>
/// <para>
/// A hook must return <c>void</c> and can have no parameters, one <see cref="Mappa.MappaContext"/>
/// parameter, one <c>ref</c> parameter whose type exactly matches the mapping target type, or a matching
/// <c>ref</c> target parameter followed by <see cref="Mappa.MappaContext"/>. Context-aware hooks are
/// considered only when the mapping method provides a context. Candidate signatures are considered in
/// the following order: target and context, target only, context only, and no parameters.
/// </para>
/// <para>
/// Method-level after-map hooks execute before class-level after-map hooks. Declaration order is
/// preserved within each scope. If class and method scope resolve to the same after-map hook, the
/// generator reports a warning and invokes it once at the method-level position.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class MappaAfterMapAttribute
    : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAfterMapAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The name of the hook on the mapper class or an accessible base class.</param>
    public MappaAfterMapAttribute(string methodName)
    {
        this.MethodName = methodName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAfterMapAttribute"/> class.
    /// </summary>
    /// <param name="classType">The type defining the <c>static</c> hook or one of its base classes.</param>
    /// <param name="methodName">The name of the <c>static</c> hook to invoke.</param>
    public MappaAfterMapAttribute(Type classType, string methodName)
        : this(methodName)
    {
        this.ClassType = classType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAfterMapAttribute"/> class.
    /// </summary>
    /// <param name="fieldName">The mapper field or property whose declared type exposes the hook.</param>
    /// <param name="methodName">The name of the hook to invoke.</param>
    public MappaAfterMapAttribute(string fieldName, string methodName)
        : this(methodName)
    {
        this.FieldName = fieldName;
    }

    /// <summary>
    /// Gets the name of the hook to invoke.
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Gets the type defining the <c>static</c> hook, including hooks declared on its base classes.
    /// </summary>
    public Type? ClassType { get; }

    /// <summary>
    /// Gets the mapper field or property name whose declared type exposes the hook.
    /// </summary>
    public string? FieldName { get; }
}