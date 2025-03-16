// <copyright file="IMappaProtobufMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Google.Protobuf.WellKnownTypes;

namespace Mappa.Dependency.Protobuf;

/// <summary>
/// Mapper methods for protobuf common types.
/// </summary>
public interface IMappaProtobufMapper
{
    /// <summary>
    /// Map from <see cref="Timestamp"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to be converted.</param>
    /// <returns>The corresponding date.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="timestamp"/> is <c>null</c>.</exception>
    DateTime MapFromTimestampToDateTime(Timestamp timestamp);

    /// <summary>
    /// Map from <see cref="Timestamp"/> to <see cref="Nullable{DateTime}"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to be converted.</param>
    /// <returns>The corresponding date.</returns>
    DateTime? MapFromNullableTimestampToNullableDateTime(Timestamp? timestamp);

    /// <summary>
    /// Map from <see cref="Timestamp"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to be converted.</param>
    /// <returns>The corresponding date.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="timestamp"/> is <c>null</c>.</exception>
    DateTimeOffset MapFromTimestampToDateTimeOffset(Timestamp timestamp);

    /// <summary>
    /// Map from <see cref="Timestamp"/> to <see cref="Nullable{DateTimeOffset}"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to be converted.</param>
    /// <returns>The corresponding date.</returns>
    DateTimeOffset? MapFromNullableTimestampToNullableDateTimeOffset(Timestamp? timestamp);

    #if NET6_0_OR_GREATER
    /// <summary>
    /// Map from <see cref="Timestamp"/> to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to be converted.</param>
    /// <returns>The corresponding date.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="timestamp"/> is <c>null</c>.</exception>
    DateOnly MapFromTimestampToDateOnly(Timestamp timestamp);

    /// <summary>
    /// Map from <see cref="Timestamp"/> to <see cref="Nullable{DateOnly}"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to be converted.</param>
    /// <returns>The corresponding date.</returns>
    DateOnly? MapFromNullableTimestampToNullableDateOnly(Timestamp? timestamp);

    /// <summary>
    /// Map from <see cref="Timestamp"/> to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to be converted.</param>
    /// <returns>The corresponding date.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="timestamp"/> is <c>null</c>.</exception>
    TimeOnly MapFromTimestampToTimeOnly(Timestamp timestamp);

    /// <summary>
    /// Map from <see cref="Timestamp"/> to <see cref="Nullable{TimeOnly}"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to be converted.</param>
    /// <returns>The corresponding date.</returns>
    TimeOnly? MapFromNullableTimestampToNullableTimeOnly(Timestamp? timestamp);
#endif

    /// <summary>
    /// Map from <see cref="DateTime"/> to <see cref="Timestamp"/>.
    /// </summary>
    /// <param name="datetime">The datetime to be converted.</param>
    /// <returns>The corresponding date.</returns>
    Timestamp MapFromDateTimeToTimestamp(DateTime datetime);

    /// <summary>
    /// Map from <see cref="Nullable{DateTime}"/> to <see cref="Timestamp"/>.
    /// </summary>
    /// <param name="datetime">The datetime to be converted.</param>
    /// <returns>The corresponding date.</returns>
    Timestamp? MapFromNullableDateTimeToNullableTimestamp(DateTime? datetime);

    /// <summary>
    /// Map from <see cref="DateTimeOffset"/> to <see cref="Timestamp"/>.
    /// </summary>
    /// <param name="datetime">The datetime to be converted.</param>
    /// <returns>The corresponding date.</returns>
    Timestamp MapFromDateTimeOffsetToTimestamp(DateTimeOffset datetime);

    /// <summary>
    /// Map from <see cref="Nullable{DateTimeOffset}"/> to <see cref="Timestamp"/>.
    /// </summary>
    /// <param name="datetime">The datetime to be converted.</param>
    /// <returns>The corresponding date.</returns>
    Timestamp? MapFromNullableDateTimeOffsetToNullableTimestamp(DateTimeOffset? datetime);

#if NET6_0_OR_GREATER
    /// <summary>
    /// Map from <see cref="DateOnly"/> to <see cref="Timestamp"/>.
    /// </summary>
    /// <param name="dateOnly">The date to be converted.</param>
    /// <returns>The corresponding date.</returns>
    Timestamp MapFromDateOnlyToTimestamp(DateOnly dateOnly);

    /// <summary>
    /// Map from <see cref="Nullable{DateOnly}"/> to <see cref="Timestamp"/>.
    /// </summary>
    /// <param name="dateOnly">The date to be converted.</param>
    /// <returns>The corresponding date.</returns>
    Timestamp? MapFromNullableDateOnlyToNullableTimestamp(DateOnly? dateOnly);
#endif

    /// <summary>
    /// Map from <see cref="Duration"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="duration">The duration to be converted.</param>
    /// <returns>The corresponding date.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="duration"/> is <c>null</c>.</exception>
    TimeSpan MapFromDurationToTimeSpan(Duration duration);

    /// <summary>
    /// Map from <see cref="Duration"/> to <see cref="Nullable{TimeSpan}"/>.
    /// </summary>
    /// <param name="duration">The duration to be converted.</param>
    /// <returns>The corresponding date.</returns>
    TimeSpan? MapFromNullableDurationToNullableTimeSpan(Duration? duration);

    /// <summary>
    /// Map from <see cref="TimeSpan"/> to <see cref="Duration"/>.
    /// </summary>
    /// <param name="timespan">The time-span to be converted.</param>
    /// <returns>The corresponding date.</returns>
    Duration MapFromTimeSpanToDuration(TimeSpan timespan);

    /// <summary>
    /// Map from <see cref="Nullable{TimeSpan}"/> to <see cref="Duration"/>.
    /// </summary>
    /// <param name="timespan">The time-span to be converted.</param>
    /// <returns>The corresponding date.</returns>
    Duration? MapFromNullableTimeSpanToNullableDuration(TimeSpan? timespan);
}