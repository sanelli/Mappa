// <copyright file="MappaIgnoreAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Attribute used to ignore some methods inside mapper classes
/// defined by the <see cref="MappaAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class MappaIgnoreAttribute
    : Attribute;