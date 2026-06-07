// <copyright file="ReadOnlyTargetCollectionMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper to test mapping to target readonly target collections
/// properties.
/// </summary>
[Mappa]
public sealed partial class ReadOnlyTargetCollectionMapper
{
    /// <summary>
    /// Map from <see cref="SourceClassWithCollections"/> to <see cref="TargetClassWithCollections"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target mapped from source.</returns>
    public partial TargetClassWithCollections Map(SourceClassWithCollections source);

    /// <summary>
    /// Map from <see cref="SourceClassWithCollections"/> to <see cref="TargetClassWithPrivateSetterPropertyCollections"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target mapped from source.</returns>
    public partial TargetClassWithPrivateSetterPropertyCollections MapWithPrivateSetters(SourceClassWithCollections source);

    /// <summary>
    /// Map from <see cref="SourceClassWithCollections"/> to <see cref="TargetProtobufClassWithCollections"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target mapped from source.</returns>
    public partial TargetProtobufClassWithCollections MapToProtobuf(SourceClassWithCollections source);

    /// <summary>
    /// Map from <see cref="SourceClassWithSpecializedCollections"/> to <see cref="TargetClassWithSpecializedCollections"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target mapped from source.</returns>
    public partial TargetClassWithSpecializedCollections MapSpecializedCollections(SourceClassWithSpecializedCollections source);
}