// <copyright file="BsonWellKnownTypesMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using MongoDB.Bson;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="BsonWellKnownTypesMapper"/>.
/// </summary>
internal static class BsonWellKnownTypesMapperRunner
{
    /// <summary>
    /// Runs map methods on <see cref="BsonWellKnownTypesMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(BsonWellKnownTypesMapper));
        var mapper = new BsonWellKnownTypesMapper();
        var objectId = ObjectId.Empty;

        report.RecordInvocation(
            nameof(BsonWellKnownTypesMapper.MapObjectIdToString),
            "ObjectId",
            "string",
            objectId,
            mapper.MapObjectIdToString(objectId));
        report.RecordInvocation(
            nameof(BsonWellKnownTypesMapper.MapStringToObjectId),
            "string",
            "ObjectId",
            objectId.ToString(),
            mapper.MapStringToObjectId(objectId.ToString()));
        report.RecordInvocation(
            nameof(BsonWellKnownTypesMapper.MapObjectIdToBytes),
            "ObjectId",
            "byte[]",
            objectId,
            mapper.MapObjectIdToBytes(objectId));
        report.RecordInvocation(
            nameof(BsonWellKnownTypesMapper.MapDecimal128ToDecimal),
            "Decimal128",
            "decimal",
            new Decimal128(1.25m),
            mapper.MapDecimal128ToDecimal(new Decimal128(1.25m)));
    }
}