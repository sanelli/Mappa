// <copyright file="MappaBsonMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using MongoDB.Bson;

namespace Mappa.Dependency.Bson;

/// <summary>
/// Implementation of <see cref="IMappaBsonMapper"/>.
/// </summary>
public sealed class MappaBsonMapper
    : IMappaBsonMapper
{
    /// <inheritdoc />
    public ObjectId MapToObjectId(string source)
    {
#if NET6_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
#else
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
#endif

        return ObjectId.Parse(source);
    }

    /// <inheritdoc />
    public ObjectId MapToObjectId(byte[] source)
    {
        return new ObjectId(source);
    }

    /// <inheritdoc />
    public ObjectId? MapToNullableObjectId(string? source)
    {
        return source is null ? null : ObjectId.Parse(source);
    }

    /// <inheritdoc />
    public string MapToString(ObjectId source)
    {
        return source.ToString();
    }

    /// <inheritdoc />
    public string? MapToNullableString(ObjectId? source)
    {
        return source?.ToString();
    }

    /// <inheritdoc />
    public byte[] MapToBytes(ObjectId source)
    {
        return source.ToByteArray();
    }
}