// <copyright file="MappaInvokeMethodAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// When the constructor strategy is used it allows
/// to force the <see cref="Mappa"/> source generator
/// to use the method <see cref="MethodName"/>.
/// The method <see cref="MethodName"/> can be a
/// <c>static</c> or non-<c>static</c> method in
/// the same class the method this attribute is applied
/// is contained, or can be a <c>static</c> method on type
/// <see cref="ClassType"/>, or can be a non-<c>static</c>
/// method available on a field in the same class the
/// method this attribute is applied is contained.<br/>
/// The method can have the following set of parameters:
/// <list type="bullet">
/// <item><description>Two parameters: the first one of the same type of the source <c>class</c>/<c>struct</c>/<c>record</c>, and the second one of the same type of the source property.</description></item>
/// <item><description>Two parameters: the first one of the same type of (or implicitly convertible from) the source <c>class</c>/<c>struct</c>/<c>record</c>, and the second one of the same type of (or implicitly convertible from) the source property.</description></item>
/// <item><description>One parameter of the same type of the source <c>class</c>/<c>struct</c>/<c>record</c>.</description></item>
/// <item><description>One parameter with type implicitly convertible from the source type.</description></item>
/// <item><description>One parameter of the same type of the source property.</description></item>
/// <item><description>One parameter with type implicitly convertible from the source property type.</description></item>
/// <item><description>No parameters.</description></item>
/// </list>
/// If multiple matching methods exist the method is
/// picked up following the order in the above list.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class MappaInvokeMethodAttribute
    : Attribute, IMappaTargetPropertyNameAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInvokeMethodAttribute"/> class.
    /// </summary>
    /// <param name="targetPropertyName">The name of the target property.</param>
    /// <param name="methodName">The name of the method to execute.</param>
    public MappaInvokeMethodAttribute(string targetPropertyName, string methodName)
    {
        this.TargetPropertyName = targetPropertyName;
        this.MethodName = methodName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInvokeMethodAttribute"/> class.
    /// </summary>
    /// <param name="targetPropertyName">The name of the target property.</param>
    /// <param name="classType">The name of the class defining the <c>static</c> method <paramref name="methodName"/>.</param>
    /// <param name="methodName">The name of the <c>static</c> method to execute.</param>
    public MappaInvokeMethodAttribute(string targetPropertyName, Type classType, string methodName)
        : this(targetPropertyName, methodName)
    {
        this.ClassType = classType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInvokeMethodAttribute"/> class.
    /// </summary>
    /// <param name="targetPropertyName">The name of the target property.</param>
    /// <param name="fieldName">The name of the field exposing the non-<c>static</c> method <paramref name="methodName"/>.</param>
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
    /// Gets the name of the class for which the static method
    /// <see cref="MethodName"/> should be invoked. If this
    /// property is set then <see cref="MethodName"/> must be <c>static</c>.
    /// </summary>
    public Type? ClassType { get; }

    /// <summary>
    /// Gets the name of the field inside the class this attribute is used
    /// on which the non-<c>static</c> method <see cref="MethodName"/> should be invoked.
    /// </summary>
    public string? FieldName { get; }
}