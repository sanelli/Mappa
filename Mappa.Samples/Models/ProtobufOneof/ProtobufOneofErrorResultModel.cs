// <copyright file="ProtobufOneofErrorResultModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.ProtobufOneof;

/// <summary>
/// The POCO counterpart of <see cref="ProtobufOneofErrorPayload"/>.
/// </summary>
public sealed class ProtobufOneofErrorResultModel : ProtobufOneofResultModel
{
    /// <summary>
    /// Gets or sets an error code.
    /// </summary>
    public required int Code { get; set; }
}