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

    /// <inheritdoc/>
    public Timestamp MapFromDateTimeToTimestamp(DateTime datetime)
        => Timestamp.FromDateTime(datetime.ToUniversalTime());

    /// <inheritdoc/>
    public Timestamp MapFromDateTimeOffsetToTimestamp(DateTimeOffset datetime)
        => Timestamp.FromDateTimeOffset(datetime.ToUniversalTime());

    /// <inheritdoc/>
    public Timestamp MapFromDateOnlyToTimestamp(DateOnly value)
        => Timestamp.FromDateTime(value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).ToUniversalTime());

    /// <inheritdoc />
    public TimeSpan MapFromDurationToTimeSpan(Duration duration)
        => duration?.ToTimeSpan() ?? throw new ArgumentNullException(nameof(duration));

    /// <inheritdoc />
    public Duration MapFromTimeSpanToDuration(TimeSpan timespan)
        => Duration.FromTimeSpan(timespan);
}