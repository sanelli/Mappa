// <copyright file="MappaProtobufTypeMappingAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Defines one arm of a protobuf <c>oneof</c> polymorphism mapping on a map method.
/// Use a different constructor for each scenario (proto→POCO, POCO→proto, or proto→proto).
/// All constructors take arguments in <c>(target, source)</c> order.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MappaProtobufTypeMappingAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaProtobufTypeMappingAttribute"/> class
    /// for proto→POCO mappings.
    /// </summary>
    /// <param name="targetType">The CLR target type for this arm.</param>
    /// <param name="sourceOneOfCaseName">The source oneof case enum member name.</param>
    public MappaProtobufTypeMappingAttribute(Type targetType, string sourceOneOfCaseName)
    {
        this.TargetType = targetType;
        this.SourceOneOfCaseName = sourceOneOfCaseName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaProtobufTypeMappingAttribute"/> class
    /// for POCO→proto mappings.
    /// </summary>
    /// <param name="targetOneOfCaseName">The target oneof case enum member name.</param>
    /// <param name="sourceType">The CLR source type for this arm.</param>
    public MappaProtobufTypeMappingAttribute(string targetOneOfCaseName, Type sourceType)
    {
        this.TargetOneOfCaseName = targetOneOfCaseName;
        this.SourceType = sourceType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaProtobufTypeMappingAttribute"/> class
    /// for proto→proto mappings.
    /// </summary>
    /// <param name="targetOneOfCaseName">The target oneof case enum member name.</param>
    /// <param name="sourceOneOfCaseName">The source oneof case enum member name.</param>
    public MappaProtobufTypeMappingAttribute(string targetOneOfCaseName, string sourceOneOfCaseName)
    {
        this.TargetOneOfCaseName = targetOneOfCaseName;
        this.SourceOneOfCaseName = sourceOneOfCaseName;
    }

    /// <summary>
    /// Gets the source oneof case enum member name, or <see langword="null"/> for POCO→proto arms.
    /// </summary>
    public string? SourceOneOfCaseName { get; }

    /// <summary>
    /// Gets the target oneof case enum member name, or <see langword="null"/> for proto→POCO arms.
    /// </summary>
    public string? TargetOneOfCaseName { get; }

    /// <summary>
    /// Gets the CLR source type for POCO→proto arms, or <see langword="null"/> otherwise.
    /// </summary>
    public Type? SourceType { get; }

    /// <summary>
    /// Gets the CLR target type for proto→POCO arms, or <see langword="null"/> otherwise.
    /// </summary>
    public Type? TargetType { get; }
}