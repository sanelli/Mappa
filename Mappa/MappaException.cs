// <copyright file="MappaException.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa;

/// <summary>
/// Exception thrown by Mappa at runtime when a mapping cannot proceed,
/// such as when the maximum runtime mapping depth has been exceeded.
/// </summary>
public class MappaException
    : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaException"/> class.
    /// </summary>
    public MappaException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MappaException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public MappaException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}