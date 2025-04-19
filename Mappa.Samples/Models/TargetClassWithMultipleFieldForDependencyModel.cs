// <copyright file="TargetClassWithMultipleFieldForDependencyModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A class with an inner class as property.
/// </summary>
public sealed class TargetClassWithMultipleFieldForDependencyModel
{
    /// <summary>
    /// Gets or sets the inner model.
    /// </summary>
    public required TargetClassModel InnerModel { get; set; } = new();

    /// <summary>
    /// Gets or sets a string value.
    /// </summary>
    public string Property1 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string value.
    /// </summary>
    public string Property2 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string value.
    /// </summary>
    public string Property3 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string value.
    /// </summary>
    public string Property4 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string value.
    /// </summary>
    public string Property5 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string value.
    /// </summary>
    public string Property6 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a string value.
    /// </summary>
    public string Property7 { get; set; } = string.Empty;
}