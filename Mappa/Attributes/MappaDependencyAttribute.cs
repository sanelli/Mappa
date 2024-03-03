// <copyright file="MappaDependencyAttribute.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Attributes;

/// <summary>
/// Specify that a property or a field can be used
/// as a source of mapping methods.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class MappaDependencyAttribute
    : Attribute;