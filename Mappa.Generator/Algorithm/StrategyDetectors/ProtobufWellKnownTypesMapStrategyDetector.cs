// <copyright file="ProtobufWellKnownTypesMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for Protobuf well-known type strategies.
/// </summary>
internal sealed class ProtobufWellKnownTypesMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtobufWellKnownTypesMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation settings.</param>
    public ProtobufWellKnownTypesMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation)
    {
        this.context = context;
        this.compilation = compilation;
    }

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        if (this.context.MappaUserSettings.ProtobufWellKnownTypes is not BooleanSetting.Enable)
        {
            return false;
        }

        foreach (var candidate in this.GetProtobufMappingCandidates())
#pragma warning disable S3267 // Loops should be simplified using the "Where" LINQ method
        {
            if (!candidate.CanMap())
            {
                continue;
            }

            mapStrategy = candidate.CreateStrategy();
            return true;
        }
#pragma warning restore S3267 // Loops should be simplified using the "Where" LINQ method

        return false;
    }

    private static bool CanMapSourceToInt32Value(ITypeSymbol sourceType)
    {
        switch (sourceType.SpecialType)
        {
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
                return true;
            default:
                return false;
        }
    }

    private static bool CanMapSourceToInt64Value(ITypeSymbol sourceType)
    {
        switch (sourceType.SpecialType)
        {
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
                return true;
            default:
                return false;
        }
    }

    private static bool CanMapSourceToUInt32Value(ITypeSymbol sourceType)
    {
        switch (sourceType.SpecialType)
        {
            case SpecialType.System_Byte:
            case SpecialType.System_UInt16:
            case SpecialType.System_UInt32:
                return true;
            default:
                return false;
        }
    }

    private static bool CanMapSourceToUInt64Value(ITypeSymbol sourceType)
    {
        switch (sourceType.SpecialType)
        {
            case SpecialType.System_Byte:
            case SpecialType.System_UInt16:
            case SpecialType.System_UInt32:
            case SpecialType.System_UInt64:
                return true;
            default:
                return false;
        }
    }

    private static bool CanMapSourceToDoubleValue(ITypeSymbol sourceType)
    {
        switch (sourceType.SpecialType)
        {
            case SpecialType.System_Single:
            case SpecialType.System_Double:
                return true;
            default:
                return false;
        }
    }

    private IEnumerable<(Func<bool> CanMap, Func<MapStrategy> CreateStrategy)> GetProtobufMappingCandidates()
    {
        var targetType = this.context.TargetType;
        var sourceType = this.context.SourceType;
        yield return (this.CanMapTimestampToDateTime, () => new ProtobufTimestampToDateTimeMapStrategy(targetType, sourceType));
        yield return (this.CanMapTimestampToDateTimeOffset, () => new ProtobufTimestampToDateTimeOffsetMapStrategy(targetType, sourceType));
        yield return (this.CanMapTimestampToDateOnly, () => new ProtobufTimestampToDateOnlyMapStrategy(targetType, sourceType));
        yield return (this.CanMapTimestampToTimeOnly, () => new ProtobufTimestampToTimeOnlyMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateTimeToTimestamp, () => new DateTimeToProtobufTimestampMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateTimeOffsetToTimestamp, () => new DateTimeOffsetToProtobufTimestampMapStrategy(targetType, sourceType));
        yield return (this.CanMapDateOnlyToTimestamp, () => new DateOnlyToProtobufTimestampMapStrategy(targetType, sourceType));
        yield return (this.CanMapDurationToTimeSpan, () => new ProtobufDurationToTimeSpanMapStrategy(targetType, sourceType));
        yield return (this.CanMapTimeSpanToDuration, () => new TimeSpanToProtobufDurationMapStrategy(targetType, sourceType));
        yield return (this.CanMapStringToStringValue, () => new StringToProtobufStringValueMapStrategy(targetType, sourceType));
        yield return (this.CanMapStringValueToString, () => new ProtobufStringValueToStringMapStrategy(targetType, sourceType));
        yield return (this.CanMapBoolToBoolValue, () => new BoolToProtobufBoolValueMapStrategy(targetType, sourceType));
        yield return (this.CanMapBoolValueToBool, () => new ProtobufBoolValueToBoolMapStrategy(targetType, sourceType));
        yield return (this.CanMapByteArrayToBytesValue, () => new ByteArrayToProtobufBytesValueMapStrategy(targetType, sourceType));
        yield return (this.CanMapBytesValueToByteArray, () => new ProtobufBytesValueToByteArrayMapStrategy(targetType, sourceType));
        yield return (this.CanMapToInt32Value, () => new ToProtobufInt32ValueMapStrategy(targetType, sourceType));
        yield return (this.CanMapInt32ValueToInt32, () => new ProtobufInt32ValueToInt32MapStrategy(targetType, sourceType));
        yield return (this.CanMapInt32ValueToInt64, () => new ProtobufInt32ValueToInt64MapStrategy(targetType, sourceType));
        yield return (this.CanMapInt32ValueToUInt64, () => new ProtobufInt32ValueToUInt64MapStrategy(targetType, sourceType));
        yield return (this.CanMapToInt64Value, () => new ToProtobufInt64ValueMapStrategy(targetType, sourceType));
        yield return (this.CanMapInt64ValueToInt64, () => new ProtobufInt64ValueToInt64MapStrategy(targetType, sourceType));
        yield return (this.CanMapToUInt32Value, () => new ToProtobufUInt32ValueMapStrategy(targetType, sourceType));
        yield return (this.CanMapUInt32ValueToUInt32, () => new ProtobufUInt32ValueToUInt32MapStrategy(targetType, sourceType));
        yield return (this.CanMapUInt32ValueToInt64, () => new ProtobufUInt32ValueToInt64MapStrategy(targetType, sourceType));
        yield return (this.CanMapUInt32ValueToUInt64, () => new ProtobufUInt32ValueToUInt64MapStrategy(targetType, sourceType));
        yield return (this.CanMapToUInt64Value, () => new ToProtobufUInt64ValueMapStrategy(targetType, sourceType));
        yield return (this.CanMapUInt64ValueToUInt64, () => new ProtobufUInt64ValueToUInt64MapStrategy(targetType, sourceType));
        yield return (this.CanMapToDoubleValue, () => new ToProtobufDoubleValueMapStrategy(targetType, sourceType));
        yield return (this.CanMapDoubleValueToDouble, () => new ProtobufDoubleValueToDoubleMapStrategy(targetType, sourceType));
        yield return (this.CanMapToFloatValue, () => new ToProtobufFloatValueMapStrategy(targetType, sourceType));
        yield return (this.CanMapFloatValueToSingle, () => new ProtobufFloatValueToSingleMapStrategy(targetType, sourceType));
        yield return (this.CanMapFloatValueToDouble, () => new ProtobufFloatValueToDoubleMapStrategy(targetType, sourceType));
    }

    private bool CanMapTimestampToDateTime()
        => this.context.SourceType.IsProtobufTimestamp(this.compilation) && this.context.TargetType.IsDateTime();

    private bool CanMapTimestampToDateTimeOffset()
        => this.context.SourceType.IsProtobufTimestamp(this.compilation) && this.context.TargetType.IsDateTimeOffset(this.compilation);

    private bool CanMapTimestampToDateOnly()
        => this.context.SourceType.IsProtobufTimestamp(this.compilation) && this.context.TargetType.IsDateOnly(this.compilation);

    private bool CanMapTimestampToTimeOnly()
        => this.context.SourceType.IsProtobufTimestamp(this.compilation) && this.context.TargetType.IsTimeOnly(this.compilation);

    private bool CanMapDateTimeToTimestamp()
        => this.context.SourceType.IsDateTime() && this.context.TargetType.IsProtobufTimestamp(this.compilation);

    private bool CanMapDateTimeOffsetToTimestamp()
        => this.context.SourceType.IsDateTimeOffset(this.compilation) && this.context.TargetType.IsProtobufTimestamp(this.compilation);

    private bool CanMapDateOnlyToTimestamp()
        => this.context.SourceType.IsDateOnly(this.compilation) && this.context.TargetType.IsProtobufTimestamp(this.compilation);

    private bool CanMapDurationToTimeSpan()
        => this.context.SourceType.IsProtobufDuration(this.compilation) && this.context.TargetType.IsTimeSpan(this.compilation);

    private bool CanMapTimeSpanToDuration()
        => this.context.SourceType.IsTimeSpan(this.compilation) && this.context.TargetType.IsProtobufDuration(this.compilation);

    private bool CanMapStringToStringValue()
        => this.context.SourceType.IsString() && this.context.TargetType.IsProtobufStringValue(this.compilation);

    private bool CanMapStringValueToString()
        => this.context.SourceType.IsProtobufStringValue(this.compilation) && this.context.TargetType.IsString();

    private bool CanMapBoolToBoolValue()
        => this.context.SourceType.IsBoolean() && this.context.TargetType.IsProtobufBoolValue(this.compilation);

    private bool CanMapBoolValueToBool()
        => this.context.SourceType.IsProtobufBoolValue(this.compilation) && this.context.TargetType.IsBoolean();

    private bool CanMapByteArrayToBytesValue()
        => this.context.SourceType.IsArray()
           && this.context.SourceType.GetElementType().IsByte()
           && this.context.TargetType.IsProtobufBytesValue(this.compilation);

    private bool CanMapBytesValueToByteArray()
        => this.context.SourceType.IsProtobufBytesValue(this.compilation)
           && this.context.TargetType.IsArray()
           && this.context.TargetType.GetElementType().IsByte();

    private bool CanMapToInt32Value()
        => this.context.TargetType.IsProtobufInt32Value(this.compilation)
           && CanMapSourceToInt32Value(this.context.SourceType);

    private bool CanMapInt32ValueToInt32()
        => this.context.SourceType.IsProtobufInt32Value(this.compilation)
           && this.context.TargetType.SpecialType == SpecialType.System_Int32;

    private bool CanMapInt32ValueToInt64()
        => this.context.SourceType.IsProtobufInt32Value(this.compilation)
           && this.context.TargetType.SpecialType == SpecialType.System_Int64;

    private bool CanMapInt32ValueToUInt64()
        => this.context.SourceType.IsProtobufInt32Value(this.compilation)
           && this.context.TargetType.SpecialType == SpecialType.System_UInt64;

    private bool CanMapToInt64Value()
        => this.context.TargetType.IsProtobufInt64Value(this.compilation)
           && CanMapSourceToInt64Value(this.context.SourceType);

    private bool CanMapInt64ValueToInt64()
        => this.context.SourceType.IsProtobufInt64Value(this.compilation)
           && this.context.TargetType.SpecialType == SpecialType.System_Int64;

    private bool CanMapToUInt32Value()
        => this.context.TargetType.IsProtobufUInt32Value(this.compilation)
           && CanMapSourceToUInt32Value(this.context.SourceType);

    private bool CanMapUInt32ValueToUInt32()
        => this.context.SourceType.IsProtobufUInt32Value(this.compilation)
           && this.context.TargetType.SpecialType == SpecialType.System_UInt32;

    private bool CanMapUInt32ValueToInt64()
        => this.context.SourceType.IsProtobufUInt32Value(this.compilation)
           && this.context.TargetType.SpecialType == SpecialType.System_Int64;

    private bool CanMapUInt32ValueToUInt64()
        => this.context.SourceType.IsProtobufUInt32Value(this.compilation)
           && this.context.TargetType.SpecialType == SpecialType.System_UInt64;

    private bool CanMapToUInt64Value()
        => this.context.TargetType.IsProtobufUInt64Value(this.compilation)
           && CanMapSourceToUInt64Value(this.context.SourceType);

    private bool CanMapUInt64ValueToUInt64()
        => this.context.SourceType.IsProtobufUInt64Value(this.compilation)
           && this.context.TargetType.SpecialType == SpecialType.System_UInt64;

    private bool CanMapToDoubleValue()
        => this.context.TargetType.IsProtobufDoubleValue(this.compilation)
           && CanMapSourceToDoubleValue(this.context.SourceType);

    private bool CanMapDoubleValueToDouble()
        => this.context.SourceType.IsProtobufDoubleValue(this.compilation)
           && this.context.TargetType.IsDouble();

    private bool CanMapToFloatValue()
        => this.context.TargetType.IsProtobufFloatValue(this.compilation)
           && this.context.SourceType.SpecialType == SpecialType.System_Single;

    private bool CanMapFloatValueToSingle()
        => this.context.SourceType.IsProtobufFloatValue(this.compilation)
           && this.context.TargetType.SpecialType == SpecialType.System_Single;

    private bool CanMapFloatValueToDouble()
        => this.context.SourceType.IsProtobufFloatValue(this.compilation)
           && this.context.TargetType.IsDouble();
}