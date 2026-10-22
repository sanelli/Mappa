// <copyright file="MappaProtobufPolymorphismSetupAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Identifies which protobuf <c>oneof</c> case property drives polymorphism for a map method.
/// Exactly one instance is required when using <see cref="MappaProtobufTypeMappingAttribute"/>
/// or <see cref="MappaProtobufTypeMappingDefaultAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class MappaProtobufPolymorphismSetupAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaProtobufPolymorphismSetupAttribute"/> class
    /// for proto→POCO or POCO→proto mappings. <paramref name="oneOfCasePropertyName"/> is the name
    /// of the <c>{Oneof}Case</c> property on the protobuf source or target message.
    /// </summary>
    /// <param name="oneOfCasePropertyName">The protobuf oneof case property name (for example <c>ResultCase</c>).</param>
    public MappaProtobufPolymorphismSetupAttribute(string oneOfCasePropertyName)
    {
        this.OneOfCasePropertyName = oneOfCasePropertyName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaProtobufPolymorphismSetupAttribute"/> class
    /// for proto→proto mappings.
    /// </summary>
    /// <param name="targetOneOfCasePropertyName">The target message oneof case property name.</param>
    /// <param name="sourceOneOfCasePropertyName">The source message oneof case property name.</param>
    public MappaProtobufPolymorphismSetupAttribute(string targetOneOfCasePropertyName, string sourceOneOfCasePropertyName)
    {
        this.TargetOneOfCasePropertyName = targetOneOfCasePropertyName;
        this.SourceOneOfCasePropertyName = sourceOneOfCasePropertyName;
    }

    /// <summary>
    /// Gets the single oneof case property name when only one side is a protobuf message,
    /// or <see langword="null"/> when both sides are protobuf messages.
    /// </summary>
    public string? OneOfCasePropertyName { get; }

    /// <summary>
    /// Gets the target oneof case property name for proto→proto mappings,
    /// or <see langword="null"/> when using the single-property form.
    /// </summary>
    public string? TargetOneOfCasePropertyName { get; }

    /// <summary>
    /// Gets the source oneof case property name for proto→proto mappings,
    /// or <see langword="null"/> when using the single-property form.
    /// </summary>
    public string? SourceOneOfCasePropertyName { get; }
}