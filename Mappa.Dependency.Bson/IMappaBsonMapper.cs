// <copyright file="IMappaBsonMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using MongoDB.Bson;

namespace Mappa.Dependency.Bson;

/// <summary>
/// Mapper methods for Bson common types.
/// </summary>
public interface IMappaBsonMapper
{
    /// <summary>
    /// Map a <see cref="string"/> to an <see cref="ObjectId"/>.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <returns>The object identifier.</returns>
    ObjectId MapToObjectId(string source);

    /// <summary>
    /// Map a byte array to an <see cref="ObjectId"/>.
    /// </summary>
    /// <param name="source">The source array of bytes.</param>
    /// <returns>The object identifier.</returns>
    ObjectId MapToObjectId(byte[] source);

    /// <summary>
    /// Map a nullable <see cref="string"/> to a nullable <see cref="ObjectId"/>.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <returns>The object identifier representation, or <c>null</c> if the <paramref name="source"/> is <c>null</c> or empty.</returns>
    ObjectId? MapToNullableObjectId(string? source);

    /// <summary>
    /// Map an <see cref="ObjectId"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <returns>The object identifier.</returns>
    string MapToString(ObjectId source);

    /// <summary>
    /// Map a nullable <see cref="ObjectId"/> to a nullable <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source object identifier.</param>
    /// <returns>The string representation, or <c>null</c> if the <paramref name="source"/> is <c>null</c>.</returns>
    string? MapToNullableString(ObjectId? source);

    /// <summary>
    /// Map an <see cref="ObjectId"/> to an array of bytes.
    /// </summary>
    /// <param name="source">The object identifier.</param>
    /// <returns>The byte array representation of <paramref name="source"/>.</returns>
    byte[] MapToBytes(ObjectId source);
}