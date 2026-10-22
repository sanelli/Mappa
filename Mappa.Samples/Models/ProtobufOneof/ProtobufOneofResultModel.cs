// <copyright file="ProtobufOneofResultModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.ProtobufOneof;

/// <summary>
/// A base class for the POCO hierarchy mapped to and from <see cref="ProtobufOneofResult"/>.
/// </summary>
public abstract class ProtobufOneofResultModel
{
    /// <summary>
    /// Gets or sets a description.
    /// </summary>
    public required string Description { get; set; }
}