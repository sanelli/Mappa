// <copyright file="MappaAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Specify the class represents a Mappa mapper.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class MappaAttribute
    : Attribute
{
}