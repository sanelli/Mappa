// <copyright file="MapFieldsPropertyTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model with properties for field-to-property mapping in <see cref="MapFieldsMapper"/>.
/// </summary>
public sealed class MapFieldsPropertyTargetModel
{
    /// <summary>
    /// Gets or sets the mapped name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the mapped age.
    /// </summary>
    public int Age { get; set; }
}