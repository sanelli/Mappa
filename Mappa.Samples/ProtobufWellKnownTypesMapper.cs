// <copyright file="ProtobufWellKnownTypesMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Google.Protobuf.WellKnownTypes;

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper showcasing <see cref="MappaSettingsAttribute.ProtobufWellKnownTypes"/> for
/// Timestamp/Duration and scalar wrapper types.
/// </summary>
[Mappa]
[MappaSettings(ProtobufWellKnownTypes = BooleanSetting.Enable)]
public sealed partial class ProtobufWellKnownTypesMapper
{
    /// <summary>
    /// Map from <see cref="MappaDependencySourceRecord"/> to <see cref="MappaDependencyTargetModel"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    public partial MappaDependencyTargetModel MapTimestamp(MappaDependencySourceRecord source);

    /// <summary>
    /// Map <see cref="DateTime"/> to <see cref="Timestamp"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped timestamp.</returns>
    public partial Timestamp MapDateTimeToTimestamp(DateTime source);

    /// <summary>
    /// Map <see cref="Timestamp"/> to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped date/time.</returns>
    public partial DateTime MapTimestampToDateTime(Timestamp source);

    /// <summary>
    /// Map <see cref="TimeSpan"/> to <see cref="Duration"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped duration.</returns>
    public partial Duration MapTimeSpanToDuration(TimeSpan source);

    /// <summary>
    /// Map <see cref="Duration"/> to <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped time span.</returns>
    public partial TimeSpan MapDurationToTimeSpan(Duration source);

    /// <summary>
    /// Map <see cref="string"/> to <see cref="StringValue"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped wrapper.</returns>
    public partial StringValue MapStringToStringValue(string source);

    /// <summary>
    /// Map <see cref="StringValue"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped string.</returns>
    public partial string MapStringValueToString(StringValue source);

    /// <summary>
    /// Map <see cref="bool"/> to <see cref="BoolValue"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped wrapper.</returns>
    public partial BoolValue MapBoolToBoolValue(bool source);

    /// <summary>
    /// Map <see cref="BoolValue"/> to <see cref="bool"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped boolean.</returns>
    public partial bool MapBoolValueToBool(BoolValue source);

    /// <summary>
    /// Map <see cref="byte"/> array to <see cref="BytesValue"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped wrapper.</returns>
    public partial BytesValue MapBytesToBytesValue(byte[] source);

    /// <summary>
    /// Map <see cref="BytesValue"/> to <see cref="byte"/> array.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped bytes.</returns>
    public partial byte[] MapBytesValueToBytes(BytesValue source);

    /// <summary>
    /// Map <see cref="int"/> to <see cref="Int32Value"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped wrapper.</returns>
    public partial Int32Value MapIntToInt32Value(int source);

    /// <summary>
    /// Map <see cref="Int32Value"/> to <see cref="int"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped integer.</returns>
    public partial int MapInt32ValueToInt(Int32Value source);

    /// <summary>
    /// Map <see cref="double"/> to <see cref="DoubleValue"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped wrapper.</returns>
    public partial DoubleValue MapDoubleToDoubleValue(double source);

    /// <summary>
    /// Map <see cref="DoubleValue"/> to <see cref="double"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped double.</returns>
    public partial double MapDoubleValueToDouble(DoubleValue source);
}