// <copyright file="MappaApplyTransformerAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Instructs the Mappa source generator to apply a value transformer during generation of the
/// annotated map method. The selected method need not be marked with <see cref="MappaTransformerAttribute"/>,
/// but must satisfy the same signature, accessibility, and <c>static</c>/instance rules as transformers.
/// <para>
/// The method to invoke can be located in one of the following ways:
/// </para>
/// <list type="bullet">
/// <item><description>
/// <c>(methodName)</c>: a <c>static</c> or instance method on the mapper class or an accessible base class.
/// A <c>static</c> mapping method can only invoke a <c>static</c> transformer.
/// </description></item>
/// <item><description>
/// <c>(classType, methodName)</c>: a <c>static</c> method declared on <see cref="ClassType"/> or an
/// accessible base class of that type.
/// </description></item>
/// <item><description>
/// <c>(sourcePropertyName, methodName)</c>: apply only when mapping the source member identified by
/// <see cref="SourcePropertyName"/> (a single name or a dot-separated nested path; path segments may
/// name instance fields when <see cref="MappaSettingsAttribute.MapSourceFields"/> is enabled); look up
/// the method on the mapper class or an accessible base class. If the source member does not exist, a
/// diagnostic is reported.
/// </description></item>
/// <item><description>
/// <c>(sourcePropertyName, classType, methodName)</c>: apply only to that source member; look up a
/// <c>static</c> method on <see cref="ClassType"/>. If the source member does not exist, a diagnostic
/// is reported.
/// </description></item>
/// </list>
/// <para>
/// Optionally set <see cref="FieldName"/> to resolve the method on the type of a mapper field or
/// property (instance invocation through that member; <c>static</c> methods via the declared type).
/// <see cref="FieldName"/> is not a constructor argument. If both <see cref="ClassType"/> and
/// <see cref="FieldName"/> are set, the generator reports a warning and skips that attribute instance.
/// </para>
/// <para>
/// Eligible signatures match <see cref="MappaTransformerAttribute"/>: same input and output type
/// including nullability; optional <c>in</c> on the value parameter; optional trailing
/// <see cref="Mappa.MappaContext"/> when the map method provides context; no <c>ref</c>/<c>out</c>;
/// no <c>async</c>; open generics allowed when bound exactly. Multiple attributes are applied in
/// declaration order before class-level <see cref="MappaTransformerAttribute"/> methods.
/// Not supported on <c>IQueryable</c> projection map methods.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaApplyTransformerAttribute
    : Attribute, IMappaMethodAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaApplyTransformerAttribute"/> class.
    /// </summary>
    /// <param name="methodName">The transformer method on the mapper class or an accessible base class.</param>
    public MappaApplyTransformerAttribute(string methodName)
    {
        this.MethodName = methodName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaApplyTransformerAttribute"/> class.
    /// </summary>
    /// <param name="classType">The type defining the <c>static</c> transformer or one of its base classes.</param>
    /// <param name="methodName">The name of the <c>static</c> transformer to invoke.</param>
    public MappaApplyTransformerAttribute(Type classType, string methodName)
        : this(methodName)
    {
        this.ClassType = classType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaApplyTransformerAttribute"/> class.
    /// </summary>
    /// <param name="sourcePropertyName">The source member name or dot-separated nested path to transform.</param>
    /// <param name="methodName">The transformer method on the mapper class or an accessible base class.</param>
    public MappaApplyTransformerAttribute(string sourcePropertyName, string methodName)
        : this(methodName)
    {
        this.SourcePropertyName = sourcePropertyName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaApplyTransformerAttribute"/> class.
    /// </summary>
    /// <param name="sourcePropertyName">The source member name or dot-separated nested path to transform.</param>
    /// <param name="classType">The type defining the <c>static</c> transformer or one of its base classes.</param>
    /// <param name="methodName">The name of the <c>static</c> transformer to invoke.</param>
    public MappaApplyTransformerAttribute(string sourcePropertyName, Type classType, string methodName)
        : this(classType, methodName)
    {
        this.SourcePropertyName = sourcePropertyName;
    }

    /// <summary>
    /// Gets the name of the transformer method to invoke.
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Gets the type defining the <c>static</c> transformer, including methods on its base classes.
    /// </summary>
    public Type? ClassType { get; }

    /// <summary>
    /// Gets the source member name or nested path this transformer is limited to, if any.
    /// </summary>
    public string? SourcePropertyName { get; }

    /// <summary>
    /// Gets or sets the mapper field or property name whose declared type exposes the transformer.
    /// When set with <see cref="ClassType"/>, the attribute instance is skipped with a warning.
    /// </summary>
    public string? FieldName { get; set; }
}