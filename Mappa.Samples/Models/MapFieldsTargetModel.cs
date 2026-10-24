// <copyright file="MapFieldsTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model with public instance fields for <see cref="MapFieldsMapper"/> samples.
/// </summary>
public sealed class MapFieldsTargetModel
{
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable SA1401 // Field should be private
#pragma warning disable S1104 // Fields should not be public
    /// <summary>
    /// The mapped name as a public field.
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    /// The mapped age as a public field.
    /// </summary>
    public int Age;
#pragma warning restore S1104
#pragma warning restore SA1401
#pragma warning restore CA1051
}