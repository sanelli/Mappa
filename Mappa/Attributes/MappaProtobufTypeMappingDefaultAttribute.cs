// <copyright file="MappaProtobufTypeMappingDefaultAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Describe the behaviour when mapping via <see cref="MappaProtobufTypeMappingAttribute"/>
/// and none of the defined arms matches the actual input (including source oneof <c>None</c>).
/// If this attribute is not specified the generator assumes <see cref="MappaProtobufTypeMappingDefaultBehavior.Throw"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class MappaProtobufTypeMappingDefaultAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaProtobufTypeMappingDefaultAttribute"/> class
    /// for <see cref="MappaProtobufTypeMappingDefaultBehavior.Throw"/> (default exception) or
    /// <see cref="MappaProtobufTypeMappingDefaultBehavior.NoMap"/>.
    /// </summary>
    /// <param name="behavior">The expected behaviour.</param>
    public MappaProtobufTypeMappingDefaultAttribute(MappaProtobufTypeMappingDefaultBehavior behavior)
        : this(behavior, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaProtobufTypeMappingDefaultAttribute"/> class
    /// for <see cref="MappaProtobufTypeMappingDefaultBehavior.Throw"/> with a custom exception type.
    /// </summary>
    /// <param name="behavior">The expected behaviour (<see cref="MappaProtobufTypeMappingDefaultBehavior.Throw"/>).</param>
    /// <param name="exceptionType">The exception type to throw.</param>
    public MappaProtobufTypeMappingDefaultAttribute(MappaProtobufTypeMappingDefaultBehavior behavior, Type exceptionType)
        : this(behavior, exceptionType, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaProtobufTypeMappingDefaultAttribute"/> class
    /// for <see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/>.
    /// </summary>
    /// <param name="behavior">The expected behaviour (<see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/>).</param>
    /// <param name="targetOneOfCaseName">The target oneof case enum member name to map into.</param>
    public MappaProtobufTypeMappingDefaultAttribute(MappaProtobufTypeMappingDefaultBehavior behavior, string targetOneOfCaseName)
        : this(behavior, null, targetOneOfCaseName)
    {
    }

    private MappaProtobufTypeMappingDefaultAttribute(
        MappaProtobufTypeMappingDefaultBehavior behavior,
        Type? exceptionType,
        string? targetOneOfCaseName)
    {
        this.Behavior = behavior;
        this.ExceptionType = exceptionType;
        this.TargetOneOfCaseName = targetOneOfCaseName;
    }

    /// <summary>
    /// Gets the user-defined behaviour.
    /// </summary>
    public MappaProtobufTypeMappingDefaultBehavior Behavior { get; }

    /// <summary>
    /// Gets the exception type for <see cref="MappaProtobufTypeMappingDefaultBehavior.Throw"/>,
    /// or <see langword="null"/> when using the default exception.
    /// </summary>
    public Type? ExceptionType { get; }

    /// <summary>
    /// Gets the target oneof case enum member name for <see cref="MappaProtobufTypeMappingDefaultBehavior.MapTo"/>,
    /// or <see langword="null"/> otherwise.
    /// </summary>
    public string? TargetOneOfCaseName { get; }
}