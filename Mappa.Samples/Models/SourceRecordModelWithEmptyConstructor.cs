// <copyright file="SourceRecordModelWithEmptyConstructor.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A source record with a few parameters
/// and an empty constructor.
/// </summary>
/// <param name="ParamA">An integer property.</param>
/// <param name="ParamB">An enumeration property.</param>
public sealed record SourceRecordModelWithEmptyConstructor(int ParamA, CountingValues ParamB)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SourceRecordModelWithEmptyConstructor"/> class.
    /// </summary>
    public SourceRecordModelWithEmptyConstructor()
        : this(-1, CountingValues.One)
    {
    }
}