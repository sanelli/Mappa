// <copyright file="TargetClassModelWithOnePrivateSetterProperty.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A target model with a few parameters
/// similar to <see cref="SourceClassModel"/>
/// and one of the setters is private so it should be ignored.
/// </summary>
public sealed class TargetClassModelWithOnePrivateSetterProperty
{
    /// <summary>
    /// Gets or sets an integer value.
    /// </summary>
    public string ParamA { get; set; } = string.Empty;

    /// <summary>
    /// Gets an enumeration value.
    /// </summary>
    public int ParamB { get; private set; } = -1;
}