// <copyright file="TargetClassModelWithSingleMapperConstructorRequiringAMappingStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A target model for integer.
/// </summary>
public sealed class TargetClassModelWithSingleMapperConstructorRequiringAMappingStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TargetClassModelWithSingleMapperConstructorRequiringAMappingStrategy"/> class.
    /// </summary>
    /// <param name="value">The input model.</param>
    public TargetClassModelWithSingleMapperConstructorRequiringAMappingStrategy(int value)
    {
        this.ParamA = value;
        this.ParamB = (CountingValues)value;
    }

    /// <summary>
    /// Gets an integer value.
    /// </summary>
    public int ParamA { get; }

    /// <summary>
    /// Gets an enumeration value.
    /// </summary>
    public CountingValues ParamB { get; }
}