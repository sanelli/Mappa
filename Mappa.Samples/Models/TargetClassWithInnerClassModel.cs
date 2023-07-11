// <copyright file="TargetClassWithInnerClassModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A class with an inner class as property.
/// </summary>
public sealed class TargetClassWithInnerClassModel
{
    /// <summary>
    /// Gets or sets the inner model.
    /// </summary>
    public required TargetClassModel InnerModel { get; set; } = new();
}