// <copyright file="MappaObjectFactoryAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Instructs the Mappa source generator to construct (or fully produce) instances of
/// <see cref="TargetType"/> by invoking <see cref="MethodName"/> instead of emitting
/// <c>new</c> / selecting a constructor during the constructor-map strategy.
/// <para>
/// The attribute can be applied to a mapper class or to an individual mapping method and
/// can locate the factory in one of the following ways:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <c>(targetType, methodName)</c>: a <c>static</c> or instance method declared on the mapper
/// class or an accessible base class. A <c>static</c> mapping method can only invoke a
/// <c>static</c> factory. Instance factories on the mapper are accessed via <c>this.</c>.
/// </description></item>
/// <item><description>
/// <c>(targetType, classType, methodName)</c>: a <c>static</c> method declared on
/// <see cref="ClassType"/> or an accessible base class, always invoked via the type name.
/// </description></item>
/// <item><description>
/// <c>(targetType, fieldName, methodName)</c>: a method declared on the type of the field or
/// property identified by <see cref="FieldName"/>. Instance factories are invoked through the
/// field or property (prefixed with <c>this.</c> when the member is not <c>static</c>).
/// Static factories are invoked through the declared type of the field or property without
/// evaluating the member. An instance factory used by a <c>static</c> mapping method requires
/// a <c>static</c> field or property.
/// </description></item>
/// </list>
/// <para>
/// The factory return type must be <see cref="TargetType"/> or a more derived type.
/// Candidate factory signatures are considered in the following priority order:
/// </para>
/// <list type="number">
/// <item><description>
/// Two parameters: the source type being mapped and <see cref="Mappa.MappaContext"/>.
/// The invocation alone is enough; no property assignment is performed.
/// </description></item>
/// <item><description>
/// One parameter: the source type being mapped. The invocation alone is enough; no property
/// assignment is performed.
/// </description></item>
/// <item><description>
/// One parameter of type <see cref="Mappa.MappaContext"/>. Treated like the empty-constructor
/// path (property initializers and post-construction fills may apply).
/// </description></item>
/// <item><description>
/// No parameters. Treated like the empty-constructor path.
/// </description></item>
/// <item><description>
/// Any other parameter list. Treated like the parameterized-constructor path (factory
/// parameters are mapped from source properties; leftover properties are not assigned).
/// </description></item>
/// </list>
/// <para>
/// Context-aware signatures are considered only when the mapping method provides a
/// <see cref="Mappa.MappaContext"/>. Class-level factories apply to every mapping method on
/// the mapper; method-level factories apply only to that method. Multiple factories for the
/// same <see cref="TargetType"/> on the class, on the method, or across both scopes is an
/// error and stops code generation. When a factory method cannot be identified, the generator
/// reports a warning and continues with the normal constructor strategy.
/// </para>
/// <para>
/// Limitations: when a factory fully produces the target (source and/or context signatures),
/// init-only properties and post-construction collection filling are not applied by the
/// generator. Object factories are not supported on <c>IQueryable</c> projection methods.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class MappaObjectFactoryAttribute
    : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaObjectFactoryAttribute"/> class.
    /// </summary>
    /// <param name="targetType">The target type for which the object factory is registered.</param>
    /// <param name="methodName">The name of the factory on the mapper class or an accessible base class.</param>
    public MappaObjectFactoryAttribute(Type targetType, string methodName)
    {
        this.TargetType = targetType;
        this.MethodName = methodName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaObjectFactoryAttribute"/> class.
    /// </summary>
    /// <param name="targetType">The target type for which the object factory is registered.</param>
    /// <param name="classType">The type defining the <c>static</c> factory or one of its base classes.</param>
    /// <param name="methodName">The name of the <c>static</c> factory to invoke.</param>
    public MappaObjectFactoryAttribute(Type targetType, Type classType, string methodName)
        : this(targetType, methodName)
    {
        this.ClassType = classType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaObjectFactoryAttribute"/> class.
    /// </summary>
    /// <param name="targetType">The target type for which the object factory is registered.</param>
    /// <param name="fieldName">The mapper field or property whose declared type exposes the factory.</param>
    /// <param name="methodName">The name of the factory to invoke.</param>
    public MappaObjectFactoryAttribute(Type targetType, string fieldName, string methodName)
        : this(targetType, methodName)
    {
        this.FieldName = fieldName;
    }

    /// <summary>
    /// Gets the target type for which the object factory is registered.
    /// </summary>
    public Type TargetType { get; }

    /// <summary>
    /// Gets the name of the factory method to invoke.
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Gets the type defining the <c>static</c> factory, including factories declared on its base classes.
    /// </summary>
    public Type? ClassType { get; }

    /// <summary>
    /// Gets the mapper field or property name whose declared type exposes the factory.
    /// </summary>
    public string? FieldName { get; }
}