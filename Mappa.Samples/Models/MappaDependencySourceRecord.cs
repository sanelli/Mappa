// <copyright file="MappaDependencySourceRecord.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model to use for mapping using the <see cref="Mappa.Dependency.Protobuf.MappaProtobufMapper"/>.
/// </summary>
/// <param name="TimeStamp">A timestamp.</param>
public sealed record MappaDependencySourceRecord(DateTime TimeStamp);