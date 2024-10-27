// <copyright file="MappaGeneratorException.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Runtime.Serialization;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Exceptions;

/// <summary>
/// An exception occurred while generating the map methods.
/// </summary>
[Serializable]
public sealed class MappaGeneratorException
    : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaGeneratorException"/> class.
    /// </summary>
    public MappaGeneratorException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaGeneratorException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public MappaGeneratorException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaGeneratorException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="location">The location this exception references.</param>
    public MappaGeneratorException(string message, Location? location)
        : base(message)
    {
        this.Location = location;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaGeneratorException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public MappaGeneratorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaGeneratorException"/> class.
    /// </summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    private MappaGeneratorException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary>
    /// Gets the references location.
    /// </summary>
    internal Location? Location { get; }
}