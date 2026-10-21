// <copyright file="BsonWellKnownTypesMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

using MongoDB.Bson;

namespace Mappa.Samples;

/// <summary>
/// Mapper showcasing <see cref="MappaSettingsAttribute.BsonWellKnownTypes"/> for
/// <see cref="ObjectId"/> and <see cref="Decimal128"/>.
/// </summary>
[Mappa]
[MappaSettings(BsonWellKnownTypes = BooleanSetting.Enable)]
public sealed partial class BsonWellKnownTypesMapper
{
    /// <summary>
    /// Map <see cref="string"/> to <see cref="ObjectId"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped object id.</returns>
    public partial ObjectId MapStringToObjectId(string source);

    /// <summary>
    /// Map <see cref="ObjectId"/> to <see cref="string"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped string.</returns>
    public partial string MapObjectIdToString(ObjectId source);

    /// <summary>
    /// Map <see cref="byte"/> array to <see cref="ObjectId"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped object id.</returns>
    public partial ObjectId MapBytesToObjectId(byte[] source);

    /// <summary>
    /// Map <see cref="ObjectId"/> to <see cref="byte"/> array.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped bytes.</returns>
    public partial byte[] MapObjectIdToBytes(ObjectId source);

    /// <summary>
    /// Map <see cref="Decimal128"/> to <see cref="decimal"/>.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The mapped decimal.</returns>
    public partial decimal MapDecimal128ToDecimal(Decimal128 source);
}