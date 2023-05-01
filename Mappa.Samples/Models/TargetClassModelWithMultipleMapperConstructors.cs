// <copyright file="TargetClassModelWithMultipleMapperConstructors.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A target model for class <see cref="CountingValues"/>.
/// </summary>
public sealed class TargetClassModelWithMultipleMapperConstructors
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TargetClassModelWithMultipleMapperConstructors"/> class.
    /// </summary>
    /// <param name="value">The input model.</param>
    public TargetClassModelWithMultipleMapperConstructors(int value)
    {
        this.ParamA = value;
        this.ParamB = (CountingValues)value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetClassModelWithMultipleMapperConstructors"/> class.
    /// </summary>
    /// <param name="value">The input model.</param>
    public TargetClassModelWithMultipleMapperConstructors(CountingValues value)
    {
        this.ParamA = (int)value;
        this.ParamB = value;
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