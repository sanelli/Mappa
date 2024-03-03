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
    {
        ArgumentNullException.ThrowIfNull(timestamp);
        return timestamp.ToDateTime();
    }

    /// <inheritdoc />
    public DateTime? MapFromNullableTimestampToNullableDateTime(Timestamp? timestamp)
        => timestamp?.ToDateTime();

    /// <inheritdoc />
    public DateTimeOffset MapFromTimestampToDateTimeOffset(Timestamp timestamp)
    {
        ArgumentNullException.ThrowIfNull(timestamp);
        return timestamp.ToDateTimeOffset();
    }

    /// <inheritdoc />
    public DateTimeOffset? MapFromNullableTimestampToNullableDateTimeOffset(Timestamp? timestamp)
        => timestamp?.ToDateTime();

    /// <inheritdoc />
    public DateOnly MapFromTimestampToDateOnly(Timestamp timestamp)
    {
        ArgumentNullException.ThrowIfNull(timestamp);
        return DateOnly.FromDateTime(timestamp.ToDateTime());
    }

    /// <inheritdoc />
    public DateOnly? MapFromNullableTimestampToNullableDateOnly(Timestamp? timestamp)
        => timestamp is null ? null : DateOnly.FromDateTime(timestamp.ToDateTime());

    /// <inheritdoc />
    public TimeOnly MapFromTimestampToTimeOnly(Timestamp timestamp)
    {
        ArgumentNullException.ThrowIfNull(timestamp);
        return TimeOnly.FromDateTime(timestamp.ToDateTime());
    }

    /// <inheritdoc />
    public TimeOnly? MapFromNullableTimestampToNullableTimeOnly(Timestamp? timestamp)
        => timestamp is null ? null : TimeOnly.FromDateTime(timestamp.ToDateTime());

    /// <inheritdoc/>
    public Timestamp MapFromDateTimeToTimestamp(DateTime datetime)
        => Timestamp.FromDateTime(datetime);

    /// <inheritdoc />
    public Timestamp? MapFromNullableDateTimeToNullableTimestamp(DateTime? datetime)
        => datetime is null ? null : Timestamp.FromDateTime(datetime.Value);

    /// <inheritdoc/>
    public Timestamp MapFromDateTimeOffsetToTimestamp(DateTimeOffset datetime)
        => Timestamp.FromDateTimeOffset(datetime.ToUniversalTime());

    /// <inheritdoc />
    public Timestamp? MapFromNullableDateTimeOffsetToNullableTimestamp(DateTimeOffset? datetime)
        => datetime is null ? null : Timestamp.FromDateTimeOffset(datetime.Value);

    /// <inheritdoc/>
    public Timestamp MapFromDateOnlyToTimestamp(DateOnly dateOnly)
        => Timestamp.FromDateTime(dateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).ToUniversalTime());

    /// <inheritdoc />
    public Timestamp? MapFromNullableDateOnlyToNullableTimestamp(DateOnly? dateOnly)
        => dateOnly is null ? null : Timestamp.FromDateTime(dateOnly.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).ToUniversalTime());

    /// <inheritdoc />
    public TimeSpan MapFromDurationToTimeSpan(Duration duration)
    {
        ArgumentNullException.ThrowIfNull(duration);
        return duration.ToTimeSpan();
    }

    /// <inheritdoc />
    public TimeSpan? MapFromNullableDurationToNullableTimeSpan(Duration? duration)
        => duration?.ToTimeSpan();

    /// <inheritdoc />
    public Duration MapFromTimeSpanToDuration(TimeSpan timespan)
        => Duration.FromTimeSpan(timespan);

    /// <inheritdoc />
    public Duration? MapFromNullableTimeSpanToNullableDuration(TimeSpan? timespan)
        => timespan is null ? null : Duration.FromTimeSpan(timespan.Value);
}