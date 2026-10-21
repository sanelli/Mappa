// <copyright file="MappaDependencySourceRecord.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source model used with <see cref="ProtobufWellKnownTypesMapper"/> for Timestamp mapping.
/// </summary>
/// <param name="TimeStamp">A timestamp.</param>
public sealed record MappaDependencySourceRecord(DateTime TimeStamp);