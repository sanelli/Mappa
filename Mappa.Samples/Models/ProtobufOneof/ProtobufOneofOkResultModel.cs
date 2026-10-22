// <copyright file="ProtobufOneofOkResultModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.ProtobufOneof;

/// <summary>
/// The POCO counterpart of <see cref="ProtobufOneofOkPayload"/>.
/// </summary>
public sealed class ProtobufOneofOkResultModel : ProtobufOneofResultModel
{
    /// <summary>
    /// Gets or sets a value.
    /// </summary>
    public required string Value { get; set; }
}