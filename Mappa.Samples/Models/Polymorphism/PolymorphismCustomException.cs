// <copyright file="PolymorphismCustomException.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models.Polymorphism;

/// <summary>
/// Custom exception.
/// </summary>
[Serializable]
#pragma warning disable S3925
public sealed class PolymorphismCustomException
#pragma warning restore S3925
    : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PolymorphismCustomException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public PolymorphismCustomException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolymorphismCustomException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PolymorphismCustomException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolymorphismCustomException"/> class.
    /// </summary>
    public PolymorphismCustomException()
    {
    }
}