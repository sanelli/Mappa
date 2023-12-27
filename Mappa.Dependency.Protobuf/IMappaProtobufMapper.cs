// <copyright file="IMappaProtobufMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Google.Protobuf.WellKnownTypes;

namespace Mappa.Dependency.Protobuf;

/// <summary>
/// Describe the method available that can be used to
/// map to and from gRPC types.
/// </summary>
public interface IMappaProtobufMapper
{
    /// <summary>
    /// Map from <see cref="Timestamp"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to be converted.</param>
    /// <returns>The corresponding date.</returns>
    DateTime MapFromTimestampToDateTime(Timestamp timestamp);

    /// <summary>
    /// Map from <see cref="Timestamp"/> to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to be converted.</param>
    /// <returns>The corresponding date.</returns>
    DateTimeOffset MapFromTimestampToDateTimeOffset(Timestamp timestamp);

    /// <summary>
    /// Map from <see cref="Timestamp"/> to <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to be converted.</param>
    /// <returns>The corresponding date.</returns>
    DateOnly MapFromTimestampToDateOnly(Timestamp timestamp);

    /// <summary>
    /// Map from <see cref="Timestamp"/> to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="timestamp">The timestamp to be converted.</param>
    /// <returns>The corresponding date.</returns>
    TimeOnly MapFromTimestampToTimeOnly(Timestamp timestamp);
}