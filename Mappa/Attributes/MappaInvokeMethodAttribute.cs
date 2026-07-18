// <copyright file="MappaInvokeMethodAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// When the constructor-map strategy is used, forces the <see cref="Mappa"/> source generator
/// to map a target property or constructor parameter by invoking <see cref="MethodName"/>.<br/>
/// <br/>
/// <see cref="TargetPropertyName"/> may be a single property name or a dot-separated chain of nested property names
/// (for example <c>"Address.City"</c>). For a path with multiple segments, the attribute applies when mapping the first segment;
/// the remaining segments are used while mapping the nested property type.<br/>
/// <see cref="SourcePropertyName"/> may likewise be a single name or a dot-separated chain of nested source property names.
/// The source path may contain the same number of segments as the target path or more, but not fewer.
/// When the remaining target path has a single segment, the generator reads the full remaining source path from the current source receiver.<br/>
/// <br/>
/// The method <see cref="MethodName"/> can be located in one of the following ways,
/// depending on which constructor overload is used:<br/>
/// <list type="bullet">
/// <item><description><c>(targetPropertyName, methodName)</c>: a <c>static</c> or non-<c>static</c> method declared on the mapper class that contains the map method, or on an accessible base class. When the root map method is <c>static</c>, only <c>static</c> methods are considered; otherwise both <c>static</c> and instance methods are considered.</description></item>
/// <item><description><c>(targetPropertyName, classType, methodName)</c>: a <c>static</c> method declared on <see cref="ClassType"/> or one of its base classes.</description></item>
/// <item><description><c>(targetPropertyName, fieldName, methodName)</c>: a non-<c>static</c> method declared on the type of the field or property identified by <see cref="FieldName"/> (including its base classes). The field or property must be declared on the mapper class or an accessible base class. When the root map method is <c>static</c>, the field or property must also be <c>static</c>.</description></item>
/// </list>
/// <see cref="SourcePropertyName"/> can be supplied as an optional named parameter on any constructor overload to specify which source property supplies the value for the invoked method. When set, it overrides the default name-based source property match for this target member.<br/>
/// When multiple methods with the same name exist in a type hierarchy, methods declared on the most derived type are preferred.<br/>
/// <br/>
/// Every candidate method must satisfy all of the following requirements:<br/>
/// <list type="bullet">
/// <item><description>Its name matches <see cref="MethodName"/> exactly.</description></item>
/// <item><description>Its return type is equal to the target property or constructor parameter type, or is implicitly convertible to it.</description></item>
/// <item><description>It is accessible from the mapper class.</description></item>
/// <item><description>It satisfies the <c>static</c> or instance requirement described above for the attribute overload in use.</description></item>
/// </list>
/// <br/>
/// When the root map method accepts a <see cref="Mappa.MappaContext"/> as its second parameter, invoked methods may also accept a <see cref="Mappa.MappaContext"/> parameter. The <see cref="Mappa.MappaContext"/> parameter is always the last parameter when combined with source and/or source-property arguments, and its type must be <c>Mappa.MappaContext</c>. Overloads that require <see cref="Mappa.MappaContext"/> are ignored when the map method does not provide one. If only <see cref="Mappa.MappaContext"/>-requiring overloads exist and the map method has no <see cref="Mappa.MappaContext"/> parameter, mapping fails.<br/>
/// <br/>
/// If multiple candidate methods match, the method is selected following the priority order below. The first matching overload wins. Tiers that reference a source property apply only when a source property is available for the target member (via name matching, <c>[MappaUseProperty]</c>, or <see cref="SourcePropertyName"/>):<br/>
/// <list type="number">
/// <item><description>Three parameters: the source <c>class</c>/<c>struct</c>/<c>record</c> (exact type), the source property (exact type), and <see cref="Mappa.MappaContext"/>. Requires the map method to provide <see cref="Mappa.MappaContext"/>.</description></item>
/// <item><description>Two parameters: the source <c>class</c>/<c>struct</c>/<c>record</c> (exact type) and the source property (exact type).</description></item>
/// <item><description>Three parameters: the source <c>class</c>/<c>struct</c>/<c>record</c> (exact or implicitly convertible type), the source property (exact or implicitly convertible type), and <see cref="Mappa.MappaContext"/>. Requires the map method to provide <see cref="Mappa.MappaContext"/>.</description></item>
/// <item><description>Two parameters: the source <c>class</c>/<c>struct</c>/<c>record</c> (exact or implicitly convertible type) and the source property (exact or implicitly convertible type).</description></item>
/// <item><description>Two parameters: the source <c>class</c>/<c>struct</c>/<c>record</c> (exact type) and <see cref="Mappa.MappaContext"/>. Requires the map method to provide <see cref="Mappa.MappaContext"/>.</description></item>
/// <item><description>One parameter of the same type as the source <c>class</c>/<c>struct</c>/<c>record</c>.</description></item>
/// <item><description>Two parameters: the source <c>class</c>/<c>struct</c>/<c>record</c> (exact or implicitly convertible type) and <see cref="Mappa.MappaContext"/>. Requires the map method to provide <see cref="Mappa.MappaContext"/>.</description></item>
/// <item><description>One parameter with a type implicitly convertible from the source <c>class</c>/<c>struct</c>/<c>record</c> type.</description></item>
/// <item><description>Two parameters: the source property (exact type) and <see cref="Mappa.MappaContext"/>. Requires the map method to provide <see cref="Mappa.MappaContext"/>.</description></item>
/// <item><description>One parameter of the same type as the source property.</description></item>
/// <item><description>Two parameters: the source property (exact or implicitly convertible type) and <see cref="Mappa.MappaContext"/>. Requires the map method to provide <see cref="Mappa.MappaContext"/>.</description></item>
/// <item><description>One parameter with a type implicitly convertible from the source property type.</description></item>
/// <item><description>One parameter of type <see cref="Mappa.MappaContext"/>. Requires the map method to provide <see cref="Mappa.MappaContext"/>.</description></item>
/// <item><description>No parameters.</description></item>
/// </list>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaInvokeMethodAttribute
    : Attribute, IMappaTargetPropertyNameAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInvokeMethodAttribute"/> class.
    /// </summary>
    /// <param name="targetPropertyName">The name of the target property or constructor parameter, or a dot-separated chain of nested target property names.</param>
    /// <param name="methodName">The name of the method to invoke on the mapper class or an accessible base class.</param>
    public MappaInvokeMethodAttribute(string targetPropertyName, string methodName)
    {
        this.TargetPropertyName = targetPropertyName;
        this.MethodName = methodName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInvokeMethodAttribute"/> class.
    /// </summary>
    /// <param name="targetPropertyName">The name of the target property or constructor parameter, or a dot-separated chain of nested target property names.</param>
    /// <param name="classType">The type defining the <c>static</c> method <paramref name="methodName"/> or one of its base classes.</param>
    /// <param name="methodName">The name of the <c>static</c> method to execute.</param>
    public MappaInvokeMethodAttribute(string targetPropertyName, Type classType, string methodName)
        : this(targetPropertyName, methodName)
    {
        this.ClassType = classType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInvokeMethodAttribute"/> class.
    /// </summary>
    /// <param name="targetPropertyName">The name of the target property or constructor parameter, or a dot-separated chain of nested target property names.</param>
    /// <param name="fieldName">The name of the field or property on the mapper class or an accessible base class whose type exposes the non-<c>static</c> method <paramref name="methodName"/>. Must be <c>static</c> when the root map method is <c>static</c>.</param>
    /// <param name="methodName">The name of the non-<c>static</c> method to execute.</param>
    public MappaInvokeMethodAttribute(string targetPropertyName, string fieldName, string methodName)
        : this(targetPropertyName, methodName)
    {
        this.FieldName = fieldName;
    }

    /// <inheritdoc />
    public string TargetPropertyName { get; }

    /// <summary>
    /// Gets the name of the method that should be invoked.
    /// </summary>
    public string MethodName { get; }

    /// <summary>
    /// Gets the type for which the <c>static</c> method <see cref="MethodName"/> should be invoked, including methods declared on its base classes. When this property is set, <see cref="MethodName"/> must refer to a <c>static</c> method.
    /// </summary>
    public Type? ClassType { get; }

    /// <summary>
    /// Gets the name of the field or property on the mapper class or an accessible base class whose type exposes the non-<c>static</c> method <see cref="MethodName"/>. When the root map method is <c>static</c>, the field or property must also be <c>static</c>.
    /// </summary>
    public string? FieldName { get; }

    /// <summary>
    /// Gets or sets the name of the source property to use when resolving method overloads that accept a source property argument.
    /// When set, this value overrides the default name-based source property match for the target member specified by <see cref="TargetPropertyName"/>.
    /// May be a single property name or a dot-separated chain of nested source property names.
    /// </summary>
    public string? SourcePropertyName { get; set; }
}