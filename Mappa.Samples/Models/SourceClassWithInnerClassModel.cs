// <copyright file="SourceClassWithInnerClassModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A class with an inner class as property.
/// </summary>
public sealed class SourceClassWithInnerClassModel
{
    /// <summary>
    /// Gets or sets the inner model.
    /// </summary>
    public SourceClassModel InnerModel { get; set; } = new();
}