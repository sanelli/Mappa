// <copyright file="MappaAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Attribute used to mark the classes for which
/// <see cref="Mappa"/> mapper methods should be generated.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class MappaAttribute
    : Attribute;