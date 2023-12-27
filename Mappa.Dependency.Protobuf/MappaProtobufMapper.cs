// <copyright file="MappaProtobufMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Google.Protobuf.WellKnownTypes;

namespace Mappa.Dependency.Protobuf;

/// <summary>
/// Implementation of <see cref="IMappaProtobufMapper"/>.
/// </summary>
public sealed class MappaProtobufMapper
    : IMappaProtobufMapper
{
    /// <inheritdoc />
    public DateTime MapFromTimestampToDateTime(Timestamp timestamp)
        => timestamp?.ToDateTime() ?? throw new ArgumentNullException(nameof(timestamp));

    /// <inheritdoc />
    public DateTimeOffset MapFromTimestampToDateTimeOffset(Timestamp timestamp)
        => timestamp?.ToDateTime() ?? throw new ArgumentNullException(nameof(timestamp));

    /// <inheritdoc />
    public DateOnly MapFromTimestampToDateOnly(Timestamp timestamp)
        => DateOnly.FromDateTime(timestamp?.ToDateTime() ?? throw new ArgumentNullException(nameof(timestamp)));

    /// <inheritdoc />
    public TimeOnly MapFromTimestampToTimeOnly(Timestamp timestamp)
        => TimeOnly.FromDateTime(timestamp?.ToDateTime() ?? throw new ArgumentNullException(nameof(timestamp)));
}