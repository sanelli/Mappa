// <copyright file="SourceRecordModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A source record with a few parameters
/// and an empty constructor.
/// </summary>
/// <param name="ParamA">An integer property.</param>
/// <param name="ParamB">An enumeration property.</param>
public sealed record SourceRecordModel(int ParamA, CountingValues ParamB);