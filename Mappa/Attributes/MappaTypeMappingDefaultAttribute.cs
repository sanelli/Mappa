// <copyright file="MappaTypeMappingDefaultAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Describe the behavior when mapping types via <see cref="MappaTypeMappingAttribute"/>
/// and none of the defined source types matches the actual input.
/// If this attribute is not specified the generator will assume that the user
/// decided to throw an exception.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class MappaTypeMappingDefaultAttribute
    : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaTypeMappingDefaultAttribute"/> class.
    /// This defaults to <see cref="MappaTypeMappingDefaultBehavior.InvokeMethod"/>.
    /// </summary>
    /// <param name="type">The type the method should be invoked on.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    public MappaTypeMappingDefaultAttribute(Type type, string methodName)
        : this(MappaTypeMappingDefaultBehavior.InvokeMethod, type, methodName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaTypeMappingDefaultAttribute"/> class.
    /// This defaults to <see cref="MappaTypeMappingDefaultBehavior.InvokeMethod"/>.
    /// </summary>
    /// <param name="methodName">The name of the method to invoke.</param>
    public MappaTypeMappingDefaultAttribute(string methodName)
        : this(MappaTypeMappingDefaultBehavior.InvokeMethod, null, methodName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaTypeMappingDefaultAttribute"/> class.
    /// This can be used for <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/> and
    /// <see cref="MappaTypeMappingDefaultBehavior.Throw"/>.
    /// </summary>
    /// <param name="behavior">The expected behavior.</param>
    /// <param name="type">The type the method should be invoked on.</param>
    public MappaTypeMappingDefaultAttribute(MappaTypeMappingDefaultBehavior behavior, Type type)
        : this(behavior, type, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaTypeMappingDefaultAttribute"/> class.
    /// This can be used for <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/> (using map method target type
    /// as target type) and <see cref="MappaTypeMappingDefaultBehavior.Throw"/> (using <see cref="ArgumentOutOfRangeException"/>
    /// as default exception) and <see cref="MappaTypeMappingDefaultBehavior.Default"/>
    /// and <see cref="MappaTypeMappingDefaultBehavior.Null"/>.
    /// </summary>
    /// <param name="behavior">The expected behavior.</param>
    public MappaTypeMappingDefaultAttribute(MappaTypeMappingDefaultBehavior behavior)
        : this(behavior, null, null)
    {
    }

    private MappaTypeMappingDefaultAttribute(MappaTypeMappingDefaultBehavior behavior, Type? type, string? methodName)
    {
        this.Behavior = behavior;
        this.Type = type;
        this.MethodName = methodName;
    }

    /// <summary>
    /// Gets the user-defined behaviour.
    /// </summary>
    public MappaTypeMappingDefaultBehavior Behavior { get; }

    /// <summary>
    /// Gets the target mapping type for <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/>
    /// or the exception to throw for <see cref="MappaTypeMappingDefaultBehavior.Throw"/>
    /// or the static class containing the method for <see cref="MappaTypeMappingDefaultBehavior.InvokeMethod"/>.
    /// </summary>
    public Type? Type { get; }

    /// <summary>
    /// Gets the name of the method to invoke for  <see cref="MappaTypeMappingDefaultBehavior.InvokeMethod"/>.
    /// </summary>
    public string? MethodName { get; }
}